using HPM.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class MigrationVerificationTests
{
    private readonly ITestOutputHelper _output;

    public MigrationVerificationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Verify_All_Migrated_Data_Integrity()
    {
        var connStr = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=60";
        var services = new ServiceCollection();
        services.AddDbContextFactory<HpmDbContext>(options => options.UseSqlServer(connStr));
        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IDbContextFactory<HpmDbContext>>();

        await using var db = await factory.CreateDbContextAsync();

        // 1. Check Patients
        var patientCount = await db.Registrations.CountAsync();
        _output.WriteLine($"[PATIENTS] Total Registered Patients: {patientCount:N0}");
        Assert.True(patientCount > 15000, "Patients should exceed 15,000");

        var samplePatient = await db.Registrations.OrderByDescending(p => p.ID).FirstOrDefaultAsync();
        Assert.NotNull(samplePatient);
        _output.WriteLine($"  Sample Patient: ID={samplePatient.ID}, PatientID={samplePatient.PatientID}, Name={samplePatient.Name}, Contact={samplePatient.Contact}");

        // 2. Check Doctors
        var doctorCount = await db.Doctors.CountAsync();
        _output.WriteLine($"[DOCTORS] Total Doctor Profiles: {doctorCount:N0}");
        Assert.True(doctorCount > 5000, "Doctors should exceed 5,000");

        // 3. Check Admissions & Beds
        var admissionCount = await db.PatientAdmissions.CountAsync();
        var bedCount = await db.BedCabins.CountAsync();
        var occupiedBeds = await db.BedCabins.CountAsync(b => b.Status == "Occupied");
        _output.WriteLine($"[IPD] Admissions: {admissionCount:N0} | Total Beds: {bedCount:N0} | Active Occupied Beds: {occupiedBeds:N0}");
        Assert.True(admissionCount > 13000, "Admissions should exceed 13,000");
        Assert.True(bedCount > 500, "Beds should exceed 500");

        // 4. Check Services & Categories
        var serviceCount = await db.Services.CountAsync();
        var categoryCount = await db.ServiceCategories.CountAsync();
        _output.WriteLine($"[MASTERS] Services: {serviceCount:N0} | Categories: {categoryCount:N0}");
        Assert.True(serviceCount > 7000, "Services should exceed 7,000");

        // 5. Check OPD Tickets & Money Receipts
        var opdCount = await db.OPDTickets.CountAsync();
        var ipdReceiptCount = await db.IpdMoneyReceipts.CountAsync();
        var patientTransCount = await db.PatientTransactions.CountAsync();
        _output.WriteLine($"[OPERATIONS] OPD Tickets: {opdCount:N0} | Money Receipts: {ipdReceiptCount:N0} | Patient Transactions: {patientTransCount:N0}");
        Assert.True(opdCount > 25000, "OPD Tickets should exceed 25,000");
        Assert.True(ipdReceiptCount > 35000, "Money Receipts should exceed 35,000");
        Assert.True(patientTransCount > 500000, "Patient Transactions should exceed 500,000");

        // 6. Check Pharmacy & Stock
        var stockCount = await db.MedicineStoreBalances.CountAsync();
        var challanCount = await db.MedicineReceivingChallans.CountAsync();
        var challanItems = await db.MedicineReceivingChallanItems.CountAsync();
        _output.WriteLine($"[PHARMACY] Store Balances: {stockCount:N0} | Challans: {challanCount:N0} | Challan Items: {challanItems:N0}");
        Assert.True(stockCount > 25000, "Medicine stocks should exceed 25,000");
        Assert.True(challanCount > 9000, "Challans should exceed 9,000");
        Assert.True(challanItems > 38000, "Challan items should exceed 38,000");

        _output.WriteLine("\n=== ALL MIGRATION INTEGRITY CHECKS PASSED 100% SUCCESSFULLY ===");
    }
}
