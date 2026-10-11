SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @Now datetime=GETDATE(), @Supplier int, @Medicine int, @Challan bigint, @Sub bigint, @Memo bigint;
    INSERT Generic_Group(Name) VALUES(N'UAT Generic'); DECLARE @Generic int=SCOPE_IDENTITY();
    INSERT Medicine_Type(Type) VALUES(N'UAT Tablet'); DECLARE @Type int=SCOPE_IDENTITY();
    INSERT Supplier_Pharmacy(Name,Amount,Trans_Date) VALUES(N'UAT TX Supplier',0,@Now); SET @Supplier=SCOPE_IDENTITY();
    INSERT Service(Name,Sale_Rate,Buy_Rate,Re_Order_Level,Category_ID,Type_ID,Generic_ID,Supplier_ID)
    VALUES(N'UAT TX Medicine',15,10,5,3,@Type,@Generic,@Supplier); SET @Medicine=SCOPE_IDENTITY();

    INSERT Medicine_Receiving_Challan(Supplier,Invoice_No,Invoice_Date,Is_Posted,Total,Discount,VAT,Store_ID,Entered_By,Trans_Date)
    VALUES(@Supplier,N'UAT-INV',@Now,1,1000,0,0,1,1,@Now); SET @Challan=SCOPE_IDENTITY();
    INSERT Medicine_Receiving_Challan_Sub(Challan_No,Service_ID,SLNO,Total,Buy_Rate,Quantity,Discount,VAT,Expire_Date,Batch_No,Sale_Rate)
    VALUES(@Challan,@Medicine,1,1000,10,100,0,0,DATEADD(year,1,CAST(@Now AS date)),N'UAT-B1',15); SET @Sub=SCOPE_IDENTITY();
    INSERT Medicine_Store_Balance(Service_ID,Medicine_Store_ID,Balance,Expire_Date,Medicine_Receiving_Challan_Sub_ID,Batch_No)
    VALUES(@Medicine,1,100,DATEADD(year,1,CAST(@Now AS date)),@Sub,N'UAT-B1');
    INSERT Supplier_Ledger(Supplier_ID,Challan_No,Debit,Credit,Balance,Trans_Date) VALUES(@Supplier,@Challan,0,1000,1000,@Now);

    INSERT Cash_Memo(Name,Patient_Type,Total,Discount,Entered_By,Trans_Date,Is_Package,Is_Credit,Buffer_Store)
    VALUES(N'UAT Customer',N'1',30,0,1,@Now,0,0,1); SET @Memo=SCOPE_IDENTITY();
    INSERT Cash_Memo_Sub(Bill_No,Service_ID,SLNO,Rate,Quantity,Buy_Rate) VALUES(@Memo,@Medicine,1,15,2,10);
    UPDATE Medicine_Store_Balance SET Balance=Balance-2 WHERE Service_ID=@Medicine AND Medicine_Store_ID=1 AND Batch_No=N'UAT-B1';
    INSERT Central_Cash_Collection(Amount,Module_Flag,Head_ID,Entered_By,Trans_Date,Trans_Date_CL,Reference_ID) VALUES(30,4,4,1,@Now,@Now,@Memo);

    IF NOT EXISTS(SELECT 1 FROM Medicine_Store_Balance WHERE Service_ID=@Medicine AND Balance=98) THROW 51000,'Stock deduction failed.',1;
    IF NOT EXISTS(SELECT 1 FROM Supplier_Ledger WHERE Challan_No=@Challan AND Balance=1000) THROW 51000,'Supplier ledger failed.',1;
    IF NOT EXISTS(SELECT 1 FROM Central_Cash_Collection WHERE Reference_ID=@Memo AND Module_Flag=4 AND Head_ID=4) THROW 51000,'Cash collection failed.',1;
    SELECT N'PASS' Result,@Challan Challan_ID,@Memo Cash_Memo_ID;
    ROLLBACK;
END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
IF EXISTS(SELECT 1 FROM Service WHERE Name=N'UAT TX Medicine') THROW 51000,'Rollback failed.',1;
SELECT N'PASS' Rollback_Result;
