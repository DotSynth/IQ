using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DatabaseOperations
{
    public class DatabaseExtensions
    {
        public static NpgsqlConnection? con;
        public static bool ConnectToDb(string ConnectionString)
        {
            bool Connected;
            try
            {
                con = new NpgsqlConnection(
                            connectionString: ConnectionString);
                con.Open();
                Connected = true;
            }
            catch
            {
                Connected = false;
            }
            return Connected;
        }

        public static string GetCurrentUserRole()
        {
            // SQL query to get current user role
            string query = "SELECT current_role;";

            using (NpgsqlCommand command = new NpgsqlCommand(query, con))
            {
                // Execute the query and retrieve the result
                string? userRole = command.ExecuteScalar()?.ToString();

                if (!string.IsNullOrEmpty(userRole))
                {

                    if (checkUserStatus(userRole))
                    {
                        string userIsAdmin = "Admin";
                        return userIsAdmin;
                    }
                    else
                    {
                        return "Branch";
                    }
                }
                else
                {
                    return "noRoleAttached";
                }
            }
        }

        static bool checkUserStatus(string userRole)
        {
            string queryUserStatus = "SELECT usesuper FROM pg_user WHERE usename = @username";
            using var queryUserStatusCommand = new NpgsqlCommand(queryUserStatus, con);
            queryUserStatusCommand.Parameters.AddWithValue("username", userRole);
            bool isSuperUser = (bool)queryUserStatusCommand.ExecuteScalar();
            return isSuperUser;
        }

        public static bool TriggerDbMassAction_Branch()
        {
            bool isCompleted;
            try
            {

                string createBranchInventoryTable = "CREATE TABLE IF NOT EXISTS BranchInventory (ModelID VARCHAR(255) UNIQUE PRIMARY KEY NOT NULL, BrandID VARCHAR(255) NOT NULL, Description VARCHAR(255) NOT NULL, QuantityInStock INT NOT NULL, UnitPrice FLOAT NOT NULL, TotalWorth FLOAT NOT NULL);";
                using var createBranchInventoryTableCommand = new NpgsqlCommand(createBranchInventoryTable, con);
                createBranchInventoryTableCommand.ExecuteScalar();

                string createSalesTable = "CREATE TABLE IF NOT EXISTS Sales (InvoiceID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantitySold INT NOT NULL, SellingPrice FLOAT NOT NULL, SoldTo VARCHAR(255) NOT NULL, CustomerContactInfo VARCHAR(255) NOT NULL, SoldOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createSalesTableCommand = new NpgsqlCommand(createSalesTable, con);
                createSalesTableCommand.ExecuteScalar();

                string createPurchasesTable = "CREATE TABLE IF NOT EXISTS Purchases (InvoiceID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityBought INT NOT NULL, BuyingPrice FLOAT NOT NULL, PurchasedFrom VARCHAR(255) NOT NULL, SupplierContactInfo VARCHAR(255) NOT NULL, PurchasedOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createPurchasesTableCommand = new NpgsqlCommand(createPurchasesTable, con);
                createPurchasesTableCommand.ExecuteScalar();

                string createTransferInwardsTable = "CREATE TABLE IF NOT EXISTS TransferInwards (TransferID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityTransferred INT NOT NULL, TransferredFrom VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, TransferredOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createTransferInwardsTableCommand = new NpgsqlCommand(createTransferInwardsTable, con);
                createTransferInwardsTableCommand.ExecuteScalar();

                string createTransferOutwardsTable = "CREATE TABLE IF NOT EXISTS TransferOutwards  (TransferID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityTransferred INT NOT NULL, TransferredTo VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, TransferredOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createTransferOutwardsTableCommand = new NpgsqlCommand(createTransferOutwardsTable, con);
                createTransferOutwardsTableCommand.ExecuteScalar();

                string createReturnInwardsTable = "CREATE TABLE IF NOT EXISTS ReturnInwards (ReturnID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityReturned INT NOT NULL, ReturnedFrom VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL,   ReturnedOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createReturnInwardsTableCommand = new NpgsqlCommand(createReturnInwardsTable, con);
                createReturnInwardsTableCommand.ExecuteScalar();

                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS ReturnOutwards (ReturnID INT UNIQUE PRIMARY KEY NOT NULL, ModelID VARCHAR(255) NOT NULL, BrandID VARCHAR(255) NOT NULL, QuantityReturned INT NOT NULL, ReturnedTo VARCHAR(255) NOT NULL, SignedBy VARCHAR(255) NOT NULL, ReturnedOn TIMESTAMP NOT NULL, FOREIGN KEY (ModelID) REFERENCES BranchInventory (ModelID));";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                createReturnOutwardsTableCommand.ExecuteScalar();

                string createCommitHistoryTable = "CREATE TABLE IF NOT EXISTS CommitHistory (CommitID INT UNIQUE PRIMARY KEY NOT NULL, CommitDescription VARCHAR(255) NOT NULL, CommitDate TIMESTAMP NOT NULL, ApprovalStatus VARCHAR(255) NOT NULL,   ApprovalDate TIMESTAMP);";
                using var createCommitHistoryTableCommand = new NpgsqlCommand(createCommitHistoryTable, con);
                createCommitHistoryTableCommand.ExecuteScalar();

                isCompleted = true;
            }
            catch
            {
                isCompleted = false;
            }

            return isCompleted;
        }

        public static bool TriggerDbMassAction_Admin()
        {
            bool isCompleted;
            try
            {
                /*
                string createTransferOutwardsTable = "CREATE TABLE IF NOT EXISTS TransferOutwards (\r\n    TransferID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityTransferred INT NOT NULL,\r\n    TransferredTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    TransferredOn TIMESTAMP NOT NULL\r\n);";
                using var createTransferOutwardsTableCommand = new NpgsqlCommand(createTransferOutwardsTable, con);
                string createReturnInwardsTable = "CREATE TABLE IF NOT EXISTS ReturnInwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedFrom VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnInwardsTableCommand = new NpgsqlCommand(createReturnInwardsTable, con);
                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS ReturnOutwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) FORIEGN KEY NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                string createWarehouseInventoryTable = "CREATE TABLE IF NOT EXISTS WarehouseInventory (\r\n    ModelID VARCHAR(255) UNIQUE NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    Description VARCHAR(255) NOT NULL,\r\n    QuantityInStock INT NOT NULL,\r\n    UnitPrice FLOAT NOT NULL,\r\n    TotalWorth FLOAT NOT NULL,\r\n);";
                using var createWarehouseInventoryTableCommand = new NpgsqlCommand(createWarehouseInventoryTable, con);
                */

                isCompleted = true;
            }
            catch
            {
                isCompleted = false;
            }

            return isCompleted;
        }

        public static bool TriggerDbMassAction_Warehouse()
        {
            bool isCompleted;
            try
            {
                
                string createReturnOutwardsTable = "CREATE TABLE IF NOT EXISTS ReturnOutwards (\r\n    ReturnID INT UNIQUE NOT NULL,\r\n    ModelID VARCHAR(255) FORIEGN KEY NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    QuantityReturned INT NOT NULL,\r\n    ReturnedTo VARCHAR(255) NOT NULL,\r\n    SignedBy VARCHAR(255) NOT NULL,\r\n    ReturnedOn TIMESTAMP NOT NULL\r\n);";
                using var createReturnOutwardsTableCommand = new NpgsqlCommand(createReturnOutwardsTable, con);
                string createWarehouseInventoryTable = "CREATE TABLE IF NOT EXISTS WarehouseInventory (\r\n    ModelID VARCHAR(255) UNIQUE NOT NULL,\r\n    BrandID VARCHAR(255) NOT NULL,\r\n    Description VARCHAR(255) NOT NULL,\r\n    QuantityInStock INT NOT NULL,\r\n    UnitPrice FLOAT NOT NULL,\r\n    TotalWorth FLOAT NOT NULL,\r\n);";
                using var createWarehouseInventoryTableCommand = new NpgsqlCommand(createWarehouseInventoryTable, con);
                

                isCompleted = true;
            }
            catch
            {
                isCompleted = false;
            }

            return isCompleted;
        }
    }
}
