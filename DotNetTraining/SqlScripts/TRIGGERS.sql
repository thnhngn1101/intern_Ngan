SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_UpdateMasterdataOld]
ON [dbo].[bpdrafts]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        PRINT 'Trigger execution started.';

        -- Declare variables for processing
        DECLARE @id UNIQUEIDENTIFIER, @UpdatedName NVARCHAR(255), 
                @ColumnName NVARCHAR(255), @UpdatedValue NVARCHAR(MAX), 
                @NewEntry NVARCHAR(MAX), @Description NVARCHAR(MAX);

        -- Mapping table for aliases
        DECLARE @ColumnMapping TABLE (Name NVARCHAR(255), Alias NVARCHAR(255));
        INSERT INTO @ColumnMapping (Name, Alias)
        VALUES 
                   ('Purchasing organization', 'PurchasingOrganization'),
        ('Purchasing organization', 'PurchasingOrganization_2'),
        ('Purchasing organization', 'PurchasingOrganization_3'),
        ('Payment Terms', 'PaymentTerms'),
        ('Payment Terms', 'PaymentTerms_2'),
        ('Payment Terms', 'PaymentTerms_3'),
        ('Payment Terms', 'PaymentTermSaleView'),
        ('Payment Terms', 'PaymentTermSaleView_2'),
        ('Payment Terms', 'PaymentTermSaleView_3'),
        ('Distr. Channel', 'DistrChannel'),
        ('Acct Assmt Grp Cust.', 'AcctAssmtGrpCust'),
        ('Acct Assmt Grp Cust.', 'AcctAssmtGrpCust_2'),
        ('Acct Assmt Grp Cust.', 'AcctAssmtGrpCust_3'),
        ('Sales Org', 'SalesOrg'),
        ('Sales Org', 'SalesOrg_2'),
        ('Sales Org', 'SalesOrg_3'),
        ('Delivering Plant', 'DeliveringPlant'),
        ('Delivering Plant', 'DeliveringPlant_2'),
        ('Delivering Plant', 'DeliveringPlant_3'),
        ('Incoterms', 'Incoterms'),
        ('Incoterms', 'Incoterms_2'),
        ('Incoterms', 'Incoterms_3'),
        ('Customer Group 2', 'CustomerGroup'),
        ('Customer Group 2', 'CustomerGroup_2'),
        ('Customer Group 2', 'CustomerGroup_3'),
        ('Cust.Pric.Procedure', 'CustPricProcedure'),
        ('Cust.Pric.Procedure', 'CustPricProcedure_2'),
        ('Cust.Pric.Procedure', 'CustPricProcedure_3'),
        ('Exchange Rate Type', 'ExchangeRateType'),
        ('Exchange Rate Type', 'ExchangeRateType_2'),
        ('Exchange Rate Type', 'ExchangeRateType_3'),
        ('Delivery Priority', 'DeliveryPriority'),
        ('Delivery Priority', 'DeliveryPriority_2'),
        ('Delivery Priority', 'DeliveryPriority_3'),
        ('Sales District', 'SalesDistrict'),
        ('Sales District', 'SalesDistrict_2'),
        ('Sales District', 'SalesDistrict_3'),
        ('Tax Number Category', 'TaxNumberCategory'),
        ('Shipping Conditions', 'ShippingCondition'),
        ('Shipping Conditions', 'ShippingConditions_2'),
        ('Shipping Conditions', 'ShippingConditions_3'),
        ('Price Group', 'PriceGroup'),
        ('Price Group', 'PriceGroup_2'),
        ('Price Group', 'PriceGroup_3'),
        ('Reconciliation Account', 'ReconciliationAccount'),
        ('Output Tax', 'OutputTax'),
        ('Output Tax', 'OutputTax_2'),
        ('Output Tax', 'OutputTax_3'),
        ('Customer Group 3', 'CustomerGroup3'),
        ('Customer Group 3', 'CustomerGroup3_2'),
        ('Customer Group 3', 'CustomerGroup3_3'),
        ('Region', 'Region'),
        ('Sales Group', 'SalesGroup'),
        ('Sales Group', 'SalesGroup_2'),
        ('Sales Group', 'SalesGroup_3'),
        ('Country Key', 'CountryKey'),
        ('Customer Group', 'CustomerGroup'),
        ('Customer Group', 'CustomerGroup_2'),
        ('Customer Group', 'CustomerGroup_3'),
        ('House Bank', 'HouseBank'),
        ('Division', 'Division')


        -- Cursor for processing rows in `inserted`
        DECLARE cur CURSOR FOR
        SELECT id FROM inserted;

        OPEN cur;

        FETCH NEXT FROM cur INTO @id;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            PRINT 'Processing row with ID: ' + CAST(@id AS NVARCHAR(50));

            -- Initialize MasterdataOld if null
            UPDATE bpdrafts
            SET MasterdataOld = ISNULL(MasterdataOld, '[]')
            WHERE id = @id;

            -- Cursor to iterate through column mappings
            DECLARE col_cursor CURSOR FOR
            SELECT DISTINCT Name, Alias FROM @ColumnMapping;

            OPEN col_cursor;

            FETCH NEXT FROM col_cursor INTO @UpdatedName, @ColumnName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                PRINT 'Checking column: ' + @ColumnName;

                -- Dynamically retrieve the column value from `inserted`
                SELECT @UpdatedValue = CASE 
                   WHEN @ColumnName = 'PurchasingOrganization' THEN (SELECT PurchasingOrganization FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'PaymentTerms' THEN (SELECT PaymentTerms FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'DistrChannel' THEN (SELECT DistrChannel FROM inserted WHERE id = @id)
                -- Add more columns as needed
                WHEN @ColumnName = 'AcctAssmtGrpCust' THEN (SELECT AcctAssmtGrpCust FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'SalesOrg' THEN (SELECT SalesOrg FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'DeliveringPlant' THEN (SELECT DeliveringPlant FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'Incoterms' THEN (SELECT Incoterms FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'CustomerGroup' THEN (SELECT CustomerGroup FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'CustPricProcedure' THEN (SELECT CustPricProcedure FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'ExchangeRateType' THEN (SELECT ExchangeRateType FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'DeliveryPriority' THEN (SELECT DeliveryPriority FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'SalesDistrict' THEN (SELECT SalesDistrict FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'TaxNumberCategory' THEN (SELECT TaxNumberCategory FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'ShippingCondition' THEN (SELECT ShippingConditions FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'PriceGroup' THEN (SELECT PriceGroup FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'ReconciliationAccount' THEN (SELECT ReconciliationAccount FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'OutputTax' THEN (SELECT OutputTax FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'CustomerGroup3' THEN (SELECT CustomerGroup3 FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'Region' THEN (SELECT Region FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'SalesGroup' THEN (SELECT SalesGroup FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'CountryKey' THEN (SELECT CountryKey FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'HouseBank' THEN (SELECT HouseBank FROM inserted WHERE id = @id)
                WHEN @ColumnName = 'Division' THEN (SELECT Division FROM inserted WHERE id = @id)
                    ELSE NULL
                END;

                -- Only process if the value is non-null and non-empty
                IF @UpdatedValue IS NOT NULL AND LEN(@UpdatedValue) > 0
                BEGIN
                    PRINT 'Updating column ' + @ColumnName + ' with value: ' + @UpdatedValue;

                    -- Fetch description from master_data
                    SELECT TOP 1 @Description = Description
                    FROM master_data
                    WHERE Name = @UpdatedName AND Value = @UpdatedValue;

                    -- Create JSON entry
                    SET @NewEntry = CONCAT(
                        '{"name": "', @UpdatedName, '", "value": "', @UpdatedValue, 
                        '", "description": "', ISNULL(@Description, 'No description'), '"}'
                    );

                    PRINT 'Appending to MasterdataOld: ' + @NewEntry;

                    -- Append to MasterdataOld
                    UPDATE bpdrafts
                    SET MasterdataOld = JSON_MODIFY(
                        ISNULL(MasterdataOld, '[]'),
                        'append $',
                        JSON_QUERY(@NewEntry)
                    )
                    WHERE id = @id;

                    PRINT 'Update completed for ID: ' + CAST(@id AS NVARCHAR(50));
                END
                ELSE
                BEGIN
                    PRINT 'No value for column: ' + @ColumnName + '. Skipping.';
                END;

                FETCH NEXT FROM col_cursor INTO @UpdatedName, @ColumnName;
            END;

            CLOSE col_cursor;
            DEALLOCATE col_cursor;

            FETCH NEXT FROM cur INTO @id;
        END;

        CLOSE cur;
        DEALLOCATE cur;

        PRINT 'Trigger execution completed successfully.';
    END TRY
    BEGIN CATCH
        -- Error logging to avoid rollback of the main transaction
        BEGIN TRY
            DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
            DECLARE @ErrorProcedure NVARCHAR(255) = ERROR_PROCEDURE();
            DECLARE @ErrorLine INT = ERROR_LINE();

            PRINT 'Error occurred: ' + @ErrorMessage;

            -- Insert error log (ensure TriggerErrorLog table exists)
            INSERT INTO TriggerErrorLog (ErrorMessage, ErrorProcedure, ErrorLine, LogTime)
            VALUES (@ErrorMessage, @ErrorProcedure, @ErrorLine, GETDATE());
        END TRY
        BEGIN CATCH
            PRINT 'Error while logging: ' + ERROR_MESSAGE();
        END CATCH;
    END CATCH
END;
GO
