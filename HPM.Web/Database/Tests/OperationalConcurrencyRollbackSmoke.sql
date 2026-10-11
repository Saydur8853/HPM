SET NOCOUNT ON;
SET XACT_ABORT OFF;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @LockResult int;
    EXEC @LockResult = sys.sp_getapplock
        @Resource = 'HPM:ConcurrencySmoke',
        @LockMode = 'Exclusive',
        @LockOwner = 'Transaction',
        @LockTimeout = 5000;
    IF @LockResult < 0 THROW 51000, 'Application lock could not be acquired.', 1;

    INSERT INTO Doctor_Appointment
        (Doctor_ID, Appt_SLNO, Appt_Date, Name, Entered_By, Trans_Date)
    VALUES
        (999999, 1, '2099-01-01', 'Concurrency Test', 1, GETDATE());

    DECLARE @DuplicateAppointmentRejected bit = 0;
    BEGIN TRY
        INSERT INTO Doctor_Appointment
            (Doctor_ID, Appt_SLNO, Appt_Date, Name, Entered_By, Trans_Date)
        VALUES
            (999999, 1, '2099-01-01', 'Duplicate Test', 1, GETDATE());
    END TRY
    BEGIN CATCH
        SET @DuplicateAppointmentRejected = 1;
    END CATCH;
    IF @DuplicateAppointmentRejected = 0
        THROW 51001, 'Duplicate appointment serial was not rejected.', 1;

    INSERT INTO Prescription (Name, Visit_No, Visit_Date, Type)
    VALUES ('Concurrency Test', 1, '2099-01-01', 'OPD');

    DECLARE @DuplicatePrescriptionRejected bit = 0;
    BEGIN TRY
        INSERT INTO Prescription (Name, Visit_No, Visit_Date, Type)
        VALUES ('Duplicate Prescription', 1, '2099-01-01', 'OPD');
    END TRY
    BEGIN CATCH
        SET @DuplicatePrescriptionRejected = 1;
    END CATCH;
    IF @DuplicatePrescriptionRejected = 0
        THROW 51002, 'Duplicate prescription visit serial was not rejected.', 1;

    INSERT INTO Bed_Cabin (Name, Category_ID, Is_Allocated)
    VALUES ('TST-CONCURRENCY-1', 1, 0);
    DECLARE @BedID1 int = SCOPE_IDENTITY();
    INSERT INTO Bed_Cabin (Name, Category_ID, Is_Allocated)
    VALUES ('TST-CONCURRENCY-2', 1, 0);
    DECLARE @BedID2 int = SCOPE_IDENTITY();

    INSERT INTO Bed_Cabin_Transection
        (Bed_Cabin_ID, Reg_No, Rate, Start_Date)
    VALUES
        (@BedID1, 'TSTREG001', 1000, GETDATE());

    DECLARE @DuplicateOpenBedRejected bit = 0;
    BEGIN TRY
        INSERT INTO Bed_Cabin_Transection
            (Bed_Cabin_ID, Reg_No, Rate, Start_Date)
        VALUES
            (@BedID1, 'TSTREG002', 1000, GETDATE());
    END TRY
    BEGIN CATCH
        SET @DuplicateOpenBedRejected = 1;
    END CATCH;
    IF @DuplicateOpenBedRejected = 0
        THROW 51003, 'A second open movement for one bed was not rejected.', 1;

    DECLARE @DuplicateOpenPatientRejected bit = 0;
    BEGIN TRY
        INSERT INTO Bed_Cabin_Transection
            (Bed_Cabin_ID, Reg_No, Rate, Start_Date)
        VALUES
            (@BedID2, 'TSTREG001', 1000, GETDATE());
    END TRY
    BEGIN CATCH
        SET @DuplicateOpenPatientRejected = 1;
    END CATCH;
    IF @DuplicateOpenPatientRejected = 0
        THROW 51004, 'A patient received two open bed movements.', 1;

    ROLLBACK TRANSACTION;
    SELECT 'PASS' AS Result,
           @DuplicateAppointmentRejected AS AppointmentGuard,
           @DuplicatePrescriptionRejected AS PrescriptionGuard,
           @DuplicateOpenBedRejected AS BedGuard,
           @DuplicateOpenPatientRejected AS PatientBedGuard;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
