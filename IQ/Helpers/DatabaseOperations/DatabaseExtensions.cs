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
    }
}
