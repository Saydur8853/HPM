using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public sealed class UserAdministrationService(IServiceScopeFactory scopeFactory)
{
    private static async Task ValidateActorAsync(IServiceProvider services, int? actorId, string role, int? targetId = null)
    {
        if (actorId is null) throw new InvalidOperationException("An authenticated administrator is required.");
        var users = services.GetRequiredService<UserManager<AppUser>>();
        var actor = await users.FindByIdAsync(actorId.Value.ToString()) ?? throw new InvalidOperationException("Administrator account was not found.");
        if (!actor.IsActive || !await users.IsInRoleAsync(actor, RoleNames.Administrator)) throw new InvalidOperationException("Administrator access is required.");
        if (role is RoleNames.AppOwner or RoleNames.HospitalOwner or RoleNames.Patient) throw new InvalidOperationException("This account type cannot be created here.");
        var hospitalOwner = await users.IsInRoleAsync(actor, RoleNames.HospitalOwner);
        if (hospitalOwner && role == RoleNames.Administrator) throw new InvalidOperationException("Hospital Owner cannot create another administrator.");
        if (hospitalOwner)
        {
            var enabled = await services.GetRequiredService<HospitalLicenseService>().EnabledModulesAsync();
            if (!HospitalLicenseService.StaffRoleAvailable(role, enabled)) throw new InvalidOperationException("This role is not covered by your licensed modules.");
        }
        if (targetId is int id)
        {
            var target = await users.FindByIdAsync(id.ToString()) ?? throw new InvalidOperationException("User account was not found.");
            var roles = await users.GetRolesAsync(target);
            if (roles.Contains(RoleNames.AppOwner) || roles.Contains(RoleNames.HospitalOwner) || (hospitalOwner && roles.Contains(RoleNames.Administrator)))
                throw new InvalidOperationException("This protected account cannot be managed here.");
        }
    }

    public async Task<UserAdminResult> CreateAsync(UserAdminCommand command, int? actorId, string actorName)
    {
        if (!RoleNames.All.Contains(command.Role)) return UserAdminResult.Fail("Select a valid role.");
        try
        {
            var userId = await ResilientTransaction.ExecuteScopedAsync(
                scopeFactory,
                async (services, db) =>
                {
                    await ValidateActorAsync(services, actorId, command.Role);
                    var users = services.GetRequiredService<UserManager<AppUser>>();
                    var doctorProfileId = await ValidateDoctorProfileAsync(db, command);
                    var user = new AppUser
                    {
                        UserName = command.UserName.Trim(), Email = command.Email?.Trim(), DisplayName = command.DisplayName.Trim(),
                        EmailConfirmed = true, IsActive = true, MustChangePassword = true, CreatedAt = DateTime.UtcNow,
                        DoctorProfileID = doctorProfileId
                    };
                    Ensure(await users.CreateAsync(user, command.Password));
                    Ensure(await users.AddToRoleAsync(user, command.Role));
                    AddAudit(db, actorId, actorName, "Create", user.Id, $"User account; Role={command.Role}; DoctorProfileID={doctorProfileId}; TemporaryPassword=True");
                    await db.SaveChangesAsync();
                    return user.Id;
                });
            return UserAdminResult.Ok(userId);
        }
        catch (Exception ex)
        {
            return UserAdminResult.Fail(ex.GetBaseException().Message);
        }
    }

    public async Task<UserAdminResult> UpdateAsync(UserAdminCommand command, int? actorId, string actorName)
    {
        if (!RoleNames.All.Contains(command.Role)) return UserAdminResult.Fail("Select a valid role.");
        if (actorId == command.ID && !command.IsActive) return UserAdminResult.Fail("You cannot deactivate your own account.");
        if (actorId == command.ID && command.Role != RoleNames.Administrator) return UserAdminResult.Fail("You cannot remove your own Administrator role.");
        try
        {
            var userId = await ResilientTransaction.ExecuteScopedAsync(
                scopeFactory,
                async (services, db) =>
                {
                    await ValidateActorAsync(services, actorId, command.Role, command.ID);
                    var users = services.GetRequiredService<UserManager<AppUser>>();
                    var doctorProfileId = await ValidateDoctorProfileAsync(db, command);
                    var user = await users.FindByIdAsync(command.ID.ToString()) ?? throw new InvalidOperationException("User account was not found.");
                    user.DisplayName = command.DisplayName.Trim();
                    user.Email = command.Email?.Trim();
                    user.IsActive = command.IsActive;
                    user.DoctorProfileID = doctorProfileId;
                    Ensure(await users.UpdateAsync(user));
                    var existingRoles = await users.GetRolesAsync(user);
                    if (existingRoles.Count > 0) Ensure(await users.RemoveFromRolesAsync(user, existingRoles));
                    Ensure(await users.AddToRoleAsync(user, command.Role));
                    var passwordReset = !string.IsNullOrWhiteSpace(command.Password);
                    if (passwordReset)
                    {
                        var token = await users.GeneratePasswordResetTokenAsync(user);
                        Ensure(await users.ResetPasswordAsync(user, token, command.Password));
                        user.MustChangePassword = true;
                        Ensure(await users.UpdateAsync(user));
                    }
                    Ensure(await users.UpdateSecurityStampAsync(user));
                    AddAudit(db, actorId, actorName, "Update", user.Id, $"User account; Role={command.Role}; DoctorProfileID={doctorProfileId}; Active={command.IsActive}; PasswordReset={passwordReset}");
                    await db.SaveChangesAsync();
                    return user.Id;
                });
            return UserAdminResult.Ok(userId);
        }
        catch (Exception ex)
        {
            return UserAdminResult.Fail(ex.GetBaseException().Message);
        }
    }

    public async Task<UserAdminResult> UnlockAsync(int userId, int? actorId, string actorName)
    {
        try
        {
            var unlockedUserId = await ResilientTransaction.ExecuteScopedAsync(
                scopeFactory,
                async (services, db) =>
                {
                    var users = services.GetRequiredService<UserManager<AppUser>>();
                    var user = await users.FindByIdAsync(userId.ToString()) ?? throw new InvalidOperationException("User account was not found.");
                    var role = (await users.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
                    await ValidateActorAsync(services, actorId, role, userId);
                    Ensure(await users.SetLockoutEndDateAsync(user, null));
                    Ensure(await users.ResetAccessFailedCountAsync(user));
                    AddAudit(db, actorId, actorName, "Update", user.Id, "Account unlocked");
                    await db.SaveChangesAsync();
                    return user.Id;
                });
            return UserAdminResult.Ok(unlockedUserId);
        }
        catch (Exception ex)
        {
            return UserAdminResult.Fail(ex.GetBaseException().Message);
        }
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded) throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));
    }

    private static async Task<int?> ValidateDoctorProfileAsync(HpmDbContext db, UserAdminCommand command)
    {
        if (command.Role != RoleNames.Doctor) return null;
        if (!command.DoctorProfileID.HasValue || command.DoctorProfileID <= 0)
            throw new InvalidOperationException("A Doctors Profile must be selected for a Doctor user.");
        if (!await db.Doctors.AnyAsync(x => x.ID == command.DoctorProfileID.Value))
            throw new InvalidOperationException("The selected Doctors Profile was not found.");
        if (await db.Users.AnyAsync(x => x.DoctorProfileID == command.DoctorProfileID && x.Id != command.ID))
            throw new InvalidOperationException("The selected Doctors Profile is already linked to another user.");
        return command.DoctorProfileID;
    }

    private static void AddAudit(HpmDbContext db, int? actorId, string actorName, string action, int entityId, string detail) =>
        db.AuditLogs.Add(new AuditLog { UserID = actorId, UserName = actorName, Action = action, EntityName = "AppUser", EntityID = entityId.ToString(), ChangedFields = detail, EventTime = DateTime.UtcNow });
}

public sealed record UserAdminCommand(int ID, string DisplayName, string UserName, string? Email, string Role, string Password, bool IsActive, int? DoctorProfileID = null);

public sealed record UserAdminResult(bool Succeeded, int? UserId, string Error)
{
    public static UserAdminResult Ok(int userId) => new(true, userId, string.Empty);
    public static UserAdminResult Fail(string error) => new(false, null, error);
}
