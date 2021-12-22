using System;
using System.Data.SqlClient;

namespace TryCRUD
{
    class Program
    {
        private static string ConnectionString = @"Data Source=ANGELO\SQLEXPRESS;Initial Catalog=mydatabase;Integrated Security=True";
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("*****************************\n" +
                                  " C: \t~ Create an entry  ~\n" +
                                  " R: \t~ Read all entries ~\n" +
                                  " R2: \t~ Read entry by ID ~\n" +
                                  " U: \t~ Update an entry  ~\n" +
                                  " D: \t~ Delete an entry  ~\n" +
                                  "*****************************");
                Console.Write(" Please choose an option: ");

                string userInput = "";
                userInput = Console.ReadLine();
                Console.WriteLine();

                switch (userInput.ToUpper())
                {
                    case "C":
                        CreateEntry();
                        Console.WriteLine("\nEntry created");
                        break;

                    case "R":
                        ReadEntry();
                        Console.WriteLine("\nAll entries read.");
                        break;

                    case "R2":
                        ReadById();
                        Console.WriteLine("Read by ID.");
                        break;

                    case "U":
                        UpdateEntry();
                        Console.WriteLine("Entry updated");
                        break;

                    case "D":
                        DeleteEntry();
                        break;

                    default:
                        Console.WriteLine($"Invalid input: {userInput}.");
                        Console.WriteLine("Please choose a valid option\n");
                        break;
                }
            } while (true);
        }

        static void CreateEntry()
        {
            Console.Write("Country name: ");
            var countryName = $"N\'{Console.ReadLine()}\'";
            Console.Write("Active status 0/1: ");
            var activeStatus = Convert.ToInt32(Console.ReadLine());

            var queryString = $"INSERT INTO tb_country (country, active)\n" +
                                 $"VALUES({countryName}, {activeStatus})";

            try
            {
                SqlConnection con = new(ConnectionString);
                SqlCommand cmd = new(queryString, con);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }

        static void ReadEntry()
        {
            string queryString = "SELECT *\n" +
                                 "FROM tb_country";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    Console.WriteLine("{0,-5} {1,-20} {2,-5:N1}\n", "ID", "Country", "Active");
                    while (rdr.Read())
                    {
                        Console.WriteLine("{0,-5} {1,-20} {2,-5:N1}", rdr[0], rdr[1], rdr[2]);
                        
                        /*
                        Country country = new Country()
                        {
                            Id = Convert.ToInt32(rdr[0]),
                            Name = Convert.ToString(rdr[1]),
                            Active = Convert.ToBoolean(rdr[2])
                        };
                        Console.WriteLine("{0,-5} {1,-20} {2,-5:N1}", country.Id, country.Name, country.Active);
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void ReadById()
        {
            //-----Prompts user for entry ID-----//
            Console.Write("ID of country to read: ");
            string id = Console.ReadLine();

            while (!IdExists(id))
            {
                PrintIds();
                Console.Write("\nNo country for provided id.\n" +
                                  "Enter a new id: ");
                id = Console.ReadLine();
            }

            Console.WriteLine();
            PrintDetailsById(id);
        }

        private static bool IdExists(string id)
        {
            // Immediately returns false if provides non-number ID
            bool isNumber = int.TryParse(id, out int idNum);
            if (!isNumber)
            {
                return false;
            }

            string queryString = $"SELECT *\n" +
                                 $"FROM tb_country\n" +
                                 $"WHERE id={idNum}";   //could have also put id instead of idNum but after check is prolly bette r

            try
            {
                    using SqlConnection con = new SqlConnection(ConnectionString);
                
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        return true; // gets first entry of that specified id (Note: id's are unique)
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private static void PrintIds()
        {
            string queryString = "SELECT id\n" +
                                 "FROM tb_country";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    Console.WriteLine(" {0,-5}\n", "ID");
                    while (rdr.Read())
                    {
                        Console.WriteLine(" {0,-5}", rdr[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void PrintDetailsById(string id)
        {
            string queryString = $"SELECT *\n" +
                                 $"FROM tb_country\n" +
                                 $"WHERE id={id}";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if(rdr.Read())
                    {
                        Console.WriteLine($" ID: {rdr[0]}\n" +
                                          $" Country: {rdr[1]}\n" +
                                          $" Active: {rdr[2]}\n");
                    }
                    else
                    {
                        Console.WriteLine("No entry for provided ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void UpdateEntry()
        {
            Console.Write("Name of country to update: ");
            string country = Console.ReadLine();
            Console.WriteLine();
            string id = GetIdByCountry(country);

            do
            {
                if (!id.Equals(""))
                {
                    break;
                }
                Console.WriteLine();
                PrintCountries();
                Console.Write("\nPlease enter another country: ");
                country = Console.ReadLine();
                id = GetIdByCountry(country);
            } while (true);

            Console.WriteLine("Current entry values:\n");
            PrintDetailsById(id);

            Console.Write("Enter new country name: ");
            string newCountry = $"{Console.ReadLine()}";
            Console.Write("Enter new active status 0/1: ");
            int newActive = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            string queryString = $"UPDATE tb_country SET country=N'{newCountry}', active={newActive} WHERE id={id}";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void PrintCountries()
        {
            string queryString = "SELECT country\n" +
                                 "FROM tb_country";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    Console.WriteLine(" {0,-5}\n", "Country");
                    while (rdr.Read())
                    {
                        Console.WriteLine(" {0,-5}", rdr[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetIdByCountry(string countryName)
        {
            string queryString = $"SELECT id, country, active FROM tb_country WHERE country='{countryName}'";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        return rdr[0].ToString();   // gets first entry of that specified country name
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.WriteLine("SORRY: No entry for provided Country.");
            return "";
        }

        static void DeleteEntry()
        {
            Console.Write("Name of country to delete: ");
            string country = Console.ReadLine();
            Console.WriteLine();
            string id = GetIdByCountry(country);

            do
            {
                if (!id.Equals(""))
                {
                    break;
                }
                Console.WriteLine();
                PrintCountries();
                Console.Write("\nPlease enter another country: ");
                country = Console.ReadLine();
                id = GetIdByCountry(country);
            } while (true);

            Console.WriteLine("Current entry values:\n");
            PrintDetailsById(id);

            Console.Write("Are you sure you want to delete? Y/N: ");
            string checkIfSure = "";
            checkIfSure = Console.ReadLine();

            while (true)
            {
                if (checkIfSure.ToUpper().Equals("Y"))
                {
                    // perform deletion on first entry of specified country
                    string queryString = $"DELETE FROM tb_country WHERE id={id}";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(ConnectionString))
                        {
                            SqlCommand cmd = new SqlCommand(queryString, con);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    Console.WriteLine("\nEntry deleted");
                    return;
                }

                if (checkIfSure.ToUpper().Equals("N"))
                {
                    Console.WriteLine("\nCancelled deletion.");
                    return;
                }

                Console.Write("Please enter a valid option Y/N: ");
                checkIfSure = Console.ReadLine();
            }
        }
    }
}