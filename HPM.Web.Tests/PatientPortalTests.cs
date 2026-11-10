using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Tests;

public sealed class PatientPortalTests
{
    [Fact]
    public void Patient_contact_fingerprint_is_stable_without_storing_raw_phone_in_claim()
    {
        var fingerprint = AppUserClaimsPrincipalFactory.ContactFingerprint(" 01700000000 ");
        Assert.Equal(fingerprint, AppUserClaimsPrincipalFactory.ContactFingerprint("01700000000"));
        Assert.NotEqual(fingerprint, AppUserClaimsPrincipalFactory.ContactFingerprint("01700000001"));
        Assert.DoesNotContain("01700000000", fingerprint);
        Assert.Contains(RoleNames.Patient, RoleNames.All);
    }

    [Fact]
    public void Portal_consent_and_query_indexes_are_in_model()
    {
        var options = new DbContextOptionsBuilder<HpmDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PrimeHpmModelOnly;Trusted_Connection=True")
            .Options;
        using var db = new HpmDbContext(options);
        var consent = db.Model.FindEntityType(typeof(PatientPortalConsent))!;
        Assert.Contains(consent.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(x => x.Name).SequenceEqual([nameof(PatientPortalConsent.PatientID), nameof(PatientPortalConsent.Version)]));
        Assert.NotNull(db.Model.FindEntityType(typeof(PatientPortalQuery)));
    }
}
