using StudentManagement.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentManagement.Services
{
    /// <summary>
    /// Service for MS SQL Server database operations
    /// Converts PostgreSQL functions to C# methods
    /// </summary>
    public class SqlServerService
    {
        // TODO: Replace with your actual SQL Server connection string
        private readonly string _connectionString = "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;";

        /// <summary>
        /// Constructor - can be updated to accept connection string from configuration
        /// </summary>
        public SqlServerService()
        {
            // Connection string can be injected via constructor if needed
        }

        /// <summary>
        /// Constructor with connection string parameter
        /// </summary>
        public SqlServerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ========================================
        // CONVERTED FUNCTIONS FROM POSTGRESQL
        // ========================================
        // Add your converted functions below
        // Each PostgreSQL function will be converted to a C# method here

        /// <summary>
        /// DEC2HEX: Convert decimal to hexadecimal
        /// Equivalent to PostgreSQL function DEC2HEX
        /// </summary>
        /// <param name="decimalValue">Integer value to convert</param>
        /// <param name="digits">Optional number of digits for padding (default: null)</param>
        /// <returns>Hexadecimal string in uppercase, or null if input is null</returns>
        public string? DEC2HEX(int? decimalValue, int? digits = null)
        {
            // Handle null input
            if (decimalValue == null)
            {
                return null;
            }

            // Convert to hexadecimal and make uppercase
            string hexValue = Convert.ToString(decimalValue.Value, 16).ToUpper();

            // If digits specified, pad with zeros on the left
            if (digits.HasValue)
            {
                hexValue = hexValue.PadLeft(digits.Value, '0');
            }

            return hexValue;
        }

        /// <summary>
        /// CODE_FUNC: Get ASCII value of first character
        /// Equivalent to PostgreSQL function CODE_FUNC
        /// </summary>
        /// <param name="inputText">Input text string</param>
        /// <returns>ASCII value of first character, or null if input is null/empty</returns>
        public int? CODE_FUNC(string? inputText)
        {
            // Handle null or empty input
            if (string.IsNullOrEmpty(inputText))
            {
                return null;
            }

            // Get ASCII value of first character
            return (int)inputText[0];
        }

        /// <summary>
        /// SUBSTITUTE: Replace occurrences of text
        /// Equivalent to PostgreSQL function SUBSTITUTE
        /// </summary>
        /// <param name="inputText">Input text string</param>
        /// <param name="oldText">Text to find and replace</param>
        /// <param name="newText">Text to replace with</param>
        /// <param name="instanceNum">Optional: specific occurrence to replace (1-based index). If null, replaces all occurrences</param>
        /// <returns>Modified text with replacements, or empty string if input/oldText is null</returns>
        public string SUBSTITUTE(string? inputText, string? oldText, string? newText, int? instanceNum = null)
        {
            // Handle null input or old_text
            if (inputText == null || oldText == null)
            {
                return string.Empty;
            }

            // Treat null new_text as empty string
            newText ??= string.Empty;

            // If instance_num is null, replace all occurrences
            if (instanceNum == null)
            {
                return inputText.Replace(oldText, newText);
            }

            // Replace specific instance
            int occurrence = 0;
            int startIndex = 0;

            while (startIndex < inputText.Length)
            {
                int foundIndex = inputText.IndexOf(oldText, startIndex, StringComparison.Ordinal);

                if (foundIndex == -1)
                {
                    // No more occurrences found
                    break;
                }

                occurrence++;

                if (occurrence == instanceNum.Value)
                {
                    // Found the specific instance to replace
                    return inputText.Substring(0, foundIndex) + newText + inputText.Substring(foundIndex + oldText.Length);
                }

                // Move past this occurrence
                startIndex = foundIndex + oldText.Length;
            }

            // Instance number not found, return original text
            return inputText;
        }

        /// <summary>
        /// TRIM_FUNC: Remove leading and trailing spaces
        /// Equivalent to PostgreSQL function TRIM_FUNC
        /// </summary>
        /// <param name="inputText">Input text string</param>
        /// <returns>Trimmed text, or null if input is null</returns>
        public string? TRIM_FUNC(string? inputText)
        {
            // Handle null input
            if (inputText == null)
            {
                return null;
            }

            // Trim leading and trailing whitespace
            return inputText.Trim();
        }

        /// <summary>
        /// ISBLANK: Check if cell is blank/null
        /// Equivalent to PostgreSQL function ISBLANK
        /// </summary>
        /// <param name="inputValue">Input value to check</param>
        /// <returns>True if value is null, empty, or consists only of whitespace; false otherwise</returns>
        public bool ISBLANK(string? inputValue)
        {
            // Check if null, empty, or only whitespace
            return string.IsNullOrWhiteSpace(inputValue);
        }

        // ========================================
        // HELPER METHODS
        // ========================================

        /// <summary>
        /// Tests if database connection is working
        /// </summary>
        /// <returns>True if connection successful, false otherwise</returns>
        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="sql">SQL command to execute</param>
        /// <param name="parameters">Parameters for the SQL command</param>
        /// <returns>Number of rows affected</returns>
        protected int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a scalar SQL command (returns single value)
        /// </summary>
        /// <param name="sql">SQL command to execute</param>
        /// <param name="parameters">Parameters for the SQL command</param>
        /// <returns>Single value result</returns>
        protected object? ExecuteScalar(string sql, Dictionary<string, object>? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a DataTable
        /// </summary>
        /// <param name="sql">SQL query to execute</param>
        /// <param name="parameters">Parameters for the SQL query</param>
        /// <returns>DataTable with results</returns>
        protected DataTable ExecuteQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
    }
}
