    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using Microsoft.IdentityModel.Tokens;

    namespace Project_Software_API.Properties.Backend.SQL.sql_fetcher
    {
        public class DataFetcher
        {
            private readonly string connection_string;

            public DataFetcher(string connectionString)
            {
                connection_string = connectionString;
            }

            /// <summary>
            /// Fetches a list of doubles from the first column of the query.
            /// This method expects numeric columns (e.g., float/double/int).
            /// If it sees a string or unparseable value, it logs and skips that row.
            /// </summary>
            public List<double> FetchData(string query)
            {
                var result = new List<double>();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connection_string))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    object rawValue = reader.GetValue(0);

                                    double finalValue;
                                    if (rawValue is double dbVal)
                                    {
                                        // Already a double
                                        finalValue = dbVal;
                                    }
                                    else if (rawValue is int intVal)
                                    {
                                        // Convert int -> double
                                        finalValue = intVal;
                                    }
                                    else if (rawValue is string strVal)
                                    {
                                        // Attempt to parse string -> double
                                        if (!double.TryParse(strVal, out finalValue))
                                        {
                                            Console.WriteLine($"Could not parse '{strVal}' as double for query: {query}");
                                            continue; // skip
                                        }
                                    }
                                    else
                                    {
                                        // Some unexpected type
                                        Console.WriteLine(
                                            $"Unexpected type {rawValue.GetType()} in column 0. Skipping row...");
                                        continue; // skip
                                    }

                                    // Now finalValue is a valid double
                                    result.Add(finalValue);
                                }
                            }
                        }
                    }

                    if (!result.Any())
                    {
                        Console.WriteLine($"No data could be fetched for query: {query}");
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error in FetchData: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred in FetchData: {ex.Message}");
                }

                return result;
            }

            /// <summary>
            /// Fetches the first column as a list of strings.
            /// Useful for queries returning gateway IDs or text columns.
            /// </summary>
            public List<string> FetchStringList(string query)
            {
                var result = new List<string>();
                try
                {
                    using (SqlConnection connection = new SqlConnection(connection_string))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    // We'll read column 0 as a string
                                    object rawValue = reader.GetValue(0);

                                    if (rawValue is string s)
                                    {
                                        result.Add(s);
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            $"Column 0 is not a string (found {rawValue.GetType()}). Skipping row...");
                                    }
                                }
                            }
                        }
                    }

                    if (!result.Any())
                    {
                        Console.WriteLine($"No data could be fetched for query: {query}");
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error in FetchStringList: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred in FetchStringList: {ex.Message}");
                }

                return result;
            }

            /// <summary>
            /// Fetches a single string value from the first row, first column.
            /// (Unchanged from your original.)
            /// </summary>
            public string FetchString(string query)
            {
                string result = "";
                try
                {
                    using (SqlConnection connection = new SqlConnection(connection_string))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            if (reader.Read())
                            {
                                result = reader.GetString(0);
                            }

                            if (result.IsNullOrEmpty())
                                throw new Exception("No data could be fetched for: " + query);
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured in DataFetcher: ${e.Message}");
                }

                return result;
            }

            /// <summary>
            /// Fetches a single int value from the first row, first column.
            /// (Unchanged from your original.)
            /// </summary>
            public int FetchInt(string query)
            {
                int result = -1;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connection_string))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt32(0);
                            }

                            if (result == -1)
                                throw new Exception("No data could be fetched for: " + query);
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured in DataFetcher: ${e.Message}");
                }

                return result;
            }
        }
    }
