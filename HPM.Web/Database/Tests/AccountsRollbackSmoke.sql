SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @Voucher bigint, @Reversal bigint, @Stamp nvarchar(36)=REPLACE(CONVERT(nvarchar(36),NEWID()),N'-',N'');
    DECLARE @VoucherNo nvarchar(20)=N'UAT-'+LEFT(@Stamp,16);
    INSERT Accounts_Voucher(Voucher_No,Voucher_Type,Voucher_Date,Narration,Is_Posted,Status,Entered_By,Trans_Date,Approved_By,Approved_At)
    VALUES(@VoucherNo,N'Expense',CAST(GETDATE() AS date),N'Rollback smoke test',1,N'Approved',1,GETDATE(),1,GETDATE()); SET @Voucher=SCOPE_IDENTITY();
    INSERT Accounts_Voucher_Line(Voucher_ID,Account_ID,Debit,Credit) VALUES(@Voucher,5,500,0),(@Voucher,1,0,500);
    IF (SELECT SUM(Debit)-SUM(Credit) FROM Accounts_Voucher_Line WHERE Voucher_ID=@Voucher)<>0 THROW 51000,'Voucher is not balanced.',1;
    INSERT Accounts_Voucher(Voucher_No,Voucher_Type,Voucher_Date,Narration,Reference,Is_Posted,Status,Entered_By,Trans_Date,Approved_By,Approved_At,Reversal_Of_Voucher_ID)
    VALUES(N'R'+LEFT(@Stamp,19),N'Reversal',CAST(GETDATE() AS date),N'Reversal smoke test',N'REV-UAT',1,N'Approved',1,GETDATE(),1,GETDATE(),@Voucher); SET @Reversal=SCOPE_IDENTITY();
    INSERT Accounts_Voucher_Line(Voucher_ID,Account_ID,Debit,Credit) VALUES(@Reversal,5,0,500),(@Reversal,1,500,0);
    UPDATE Accounts_Voucher SET Status=N'Cancelled',Cancelled_By=1,Cancelled_At=GETDATE(),Cancellation_Reason=N'UAT' WHERE ID=@Voucher;
    IF (SELECT SUM(Debit)-SUM(Credit) FROM Accounts_Voucher_Line WHERE Voucher_ID IN(@Voucher,@Reversal))<>0 THROW 51000,'Reversal is not balanced.',1;
    INSERT Cashier_Handover(Business_Date,Cashier_ID,System_Amount,Handed_Over_Amount,Variance,Notes,Trans_Date,Is_Locked,Closed_At) VALUES(CAST(GETDATE() AS date),-999,500,500,0,N'UAT',GETDATE(),1,GETDATE());
    INSERT Financial_Period_Lock(From_Date,To_Date,Reason,Is_Active,Locked_By,Locked_At) VALUES(CAST(GETDATE() AS date),CAST(GETDATE() AS date),N'UAT',1,1,GETDATE());
    INSERT Financial_Reversal(Entity_Type,Entity_ID,Amount,Reason,Reversal_Voucher_ID,Reversed_By,Reversed_At) VALUES(N'UAT',-999,500,N'UAT',@Reversal,1,GETDATE());
    SELECT N'PASS' Result,@Voucher Voucher_ID,@Reversal Reversal_ID;
    ROLLBACK;
END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
IF EXISTS(SELECT 1 FROM Accounts_Voucher WHERE Voucher_No=@VoucherNo) THROW 51000,'Rollback failed.',1;
IF EXISTS(SELECT 1 FROM Financial_Reversal WHERE Entity_Type=N'UAT' AND Entity_ID=-999) THROW 51000,'Reversal rollback failed.',1;
SELECT N'PASS' Rollback_Result;
