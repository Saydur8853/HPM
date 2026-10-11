using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public sealed class UserAdministrationService(IServiceScopeFactory scopeFactory)
{
    public async Task<UserAdminResult> CreateAsync(UserAdminCommand command, int? actorId, string actorName)
    {
        if (!RoleNames.All.Contains(command.Role)) return UserAdminResult.Fail("Select a valid role.");
        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();
        int? doctorProfileId;
        try { doctorProfileId = await ValidateDoctorProfileAsync(db, command); }
        catch (Exception ex) { return UserAdminResult.Fail(ex.Message); }
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
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
            await transaction.CommitAsync();
            return UserAdminResult.Ok(user.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return UserAdminResult.Fail(ex.Message);
        }
    }

    public async Task<UserAdminResult> UpdateAsync(UserAdminCommand command, int? actorId, string actorName)
    {
        if (!RoleNames.All.Contains(command.Role)) return UserAdminResult.Fail("Select a valid role.");
        if (actorId == command.ID && !command.IsActive) return UserAdminResult.Fail("You cannot deactivate your own account.");
        if (actorId == command.ID && command.Role != RoleNames.Administrator) return UserAdminResult.Fail("You cannot remove your own Administrator role.");
        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();
        int? doctorProfileId;
        try { doctorProfileId = await ValidateDoctorProfileAsync(db, command); }
        catch (Exception ex) { return UserAdminResult.Fail(ex.Message); }
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
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
            // Invalidate all existing cookies after role, status or password
            // changes. This makes deactivation effective for signed-in users.
            Ensure(await users.UpdateSecurityStampAsync(user));
            AddAudit(db, actorId, actorName, "Update", user.Id, $"User account; Role={command.Role}; DoctorProfileID={doctorProfileId}; Active={command.IsActive}; PasswordReset={passwordReset}");
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return UserAdminResult.Ok(user.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return UserAdminResult.Fail(ex.Message);
        }
    }

    public async Task<UserAdminResult> UnlockAsync(int userId, int? actorId, string actorName)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var user = await users.FindByIdAsync(userId.ToString()) ?? throw new InvalidOperationException("User account was not found.");
            Ensure(await users.SetLockoutEndDateAsync(user, null));
            Ensure(await users.ResetAccessFailedCountAsync(user));
            AddAudit(db, actorId, actorName, "Update", user.Id, "Account unlocked");
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return UserAdminResult.Ok(user.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return UserAdminResult.Fail(ex.Message);
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
