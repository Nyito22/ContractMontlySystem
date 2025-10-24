using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;

namespace ContractMontlySystem.Models
{
    public class sql_connection
    {

       public string connection_string = @"Server=(localdb)\ContractClaimSystem;Database=Monthly_Claim_System;";

        public void createUserTable()
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection_string))
                {
                    connect.Open();

                    string Sqlquery = @"CREATE TABLE Staff(
                                        EmployeeId INT IDENTITY(1000,10) PRIMARY KEY NOT NULL,
                                        FullNames VARCHAR (100) NOT NULL,
                                        Email VARCHAR(50) NOT NULL,
                                        Phone_Number VARCHAR(15) NOT NULL,
                                        Password VARCHAR (50) NOT NULL,
                                        Role VARCHAR(50) NOT NULL);";

                    using (SqlCommand command = new SqlCommand(Sqlquery,connect))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Table is created");
                    }
                    connect.Close();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }


        public void Store_into_Table(string Fullnames, string phoneNumber, string email, string password, string role)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    // Creating a parameterized query to avoid SQL injection
                    string query = @"INSERT INTO Staff (FullNames, Email, Phone_Number, Password, Role) 
                             VALUES (@Fullnames, @Email, @PhoneNumber, @Password, @Role)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Adding parameters to the query
                        command.Parameters.AddWithValue("@Fullnames", Fullnames);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Role", role);

                        // Executing the query
                        command.ExecuteNonQuery();
                        Console.WriteLine("Info stored successfully.");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }


        //
        public bool LogInUser(string email, string password, string role)
        {
            bool found = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    // Query to check if there's a user with the given email, password, and role
                    string query = @"SELECT COUNT(*) 
                             FROM Staff 
                             WHERE Email = @Email 
                             AND Password = @Password 
                             AND Role = @Role";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Role", role);

                        // Execute the query and check if any rows were returned (i.e., user found)
                        int userCount = (int)command.ExecuteScalar();

                        // If userCount > 0, the user exists, so log in is successful
                        if (userCount > 0)
                        {
                            found = true;
                            Console.WriteLine("Login successful!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid login credentials.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return found;
        }



        public void createClaimsTable()
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection_string))
                {
                    connect.Open();
                    string Sqlquery = @"CREATE TABLE Claim (
    ClaimId INT IDENTITY(1,1) PRIMARY KEY,        
    LectureName VARCHAR(100) NOT NULL,            
    LectureId VARCHAR(50) NOT NULL,            
    ModuleName VARCHAR(100) NOT NULL,              
    ClaimFrom DATE NOT NULL,                      
    ClaimTo DATE NOT NULL,                        
    HourlyWage DECIMAL(10, 2) NOT NULL,           
    SessionHours INT NOT NULL,                   
    SupportingDocs VARCHAR(255) NOT NULL,       
    DateSubmitted DATETIME DEFAULT GETDATE(),
    Claim_Status VARCHAR(50) DEFAULT 'Pending'
);
";
                    using (SqlCommand command = new SqlCommand(Sqlquery, connect))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Table is created");
                    }
                    connect.Close();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
        public bool StoreClaimIntoTable(
     string lectureName,
     string lectureId,
     string moduleName,
     DateTime claimFrom,
     DateTime claimTo,
     decimal hourlyWage,
     int sessionHours,
     string filePath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    decimal totalAmount = hourlyWage * sessionHours;

                    string query = @"INSERT INTO Claim 
                (LectureName, LectureId, ModuleName, ClaimFrom, ClaimTo, HourlyWage, SessionHours, TotalAmount, SupportingDocs)
                VALUES 
                (@LectureName, @LectureId, @ModuleName, @ClaimFrom, @ClaimTo, @HourlyWage, @SessionHours, @TotalAmount, @SupportingDocs)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LectureName", lectureName);
                        command.Parameters.AddWithValue("@LectureId", lectureId);
                        command.Parameters.AddWithValue("@ModuleName", moduleName);
                        command.Parameters.AddWithValue("@ClaimFrom", claimFrom);
                        command.Parameters.AddWithValue("@ClaimTo", claimTo);
                        command.Parameters.AddWithValue("@HourlyWage", hourlyWage);
                        command.Parameters.AddWithValue("@SessionHours", sessionHours);
                        command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        command.Parameters.AddWithValue("@SupportingDocs", string.IsNullOrEmpty(filePath) ? DBNull.Value : (object)filePath);

                        Console.WriteLine($"Saving claim to DB: {lectureName}, {lectureId}, {moduleName}, {claimFrom}-{claimTo}, {hourlyWage}, {sessionHours}, FilePath={filePath}");
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine("Rows inserted: " + rowsAffected);

                        if (rowsAffected == 0)
                            Console.WriteLine("Insert failed: check your database and column constraints.");

                        return rowsAffected > 0;


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error storing claim: " + ex.Message);
                return false;
            }
        }


        public List<Claims> GetAllClaims()
        {
            List<Claims> claimsList = new List<Claims>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    string query = "SELECT * FROM Claim";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Claims claim = new Claims
                            {
                                ClaimId = reader["ClaimId"].ToString(),
                                LectureName = reader["LectureName"].ToString(),
                                LectureId = reader["LectureId"].ToString(),
                                ModuleName = reader["ModuleName"].ToString(),
                                ClaimFrom = reader.IsDBNull(reader.GetOrdinal("ClaimFrom")) ? DateTime.MinValue : Convert.ToDateTime(reader["ClaimFrom"]),
                                ClaimTo = reader.IsDBNull(reader.GetOrdinal("ClaimTo")) ? DateTime.MinValue : Convert.ToDateTime(reader["ClaimTo"]),
                                HourlyWage = reader.IsDBNull(reader.GetOrdinal("HourlyWage")) ? 0 : Convert.ToDecimal(reader["HourlyWage"]),
                                SessionHours = reader.IsDBNull(reader.GetOrdinal("SessionHours")) ? 0 : Convert.ToInt32(reader["SessionHours"]),
                                TotalAmount = (reader.IsDBNull(reader.GetOrdinal("HourlyWage")) || reader.IsDBNull(reader.GetOrdinal("SessionHours")))
                                                ? 0
                                                : Convert.ToDecimal(reader["HourlyWage"]) * Convert.ToInt32(reader["SessionHours"]),
                                SupportingDocsPath = reader["SupportingDocs"].ToString(),
                                Claim_Status = reader["Claim_Status"].ToString()
                            };
                            claimsList.Add(claim);
                        }
                    }
                }
                Console.WriteLine("Total claims found: " + claimsList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving claims: " + ex.Message);
            }

            return claimsList;
        }
        public bool UpdateClaimStatuses(Dictionary<string, string> UpdatedStatuses)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();

                    foreach (var item in UpdatedStatuses)
                    {
                        string claimId = item.Key;
                        string newStatus = item.Value;

                        if (!string.IsNullOrEmpty(newStatus))
                        {
                            string query = "UPDATE Claim SET Claim_Status = @Status WHERE ClaimId = @ClaimId";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@ClaimId", claimId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating claim statuses: " + ex.Message);
                return false;
            }
        }


        public bool UpdateFinalStatuses(Dictionary<string, string> finalStatuses)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();

                    foreach (var item in finalStatuses)
                    {
                        string claimId = item.Key;
                        string newStatus = item.Value;

                        if (!string.IsNullOrEmpty(newStatus))
                        {
                            // Update FinalStatus column for AM approval
                            string query = "UPDATE Claim SET FinalStatus = @Status WHERE ClaimId = @ClaimId";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@ClaimId", claimId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating final statuses: " + ex.Message);
                return false;
            }
        }


    }
}




    

