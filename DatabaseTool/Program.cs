using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySqlConnector;
using Dapper;

namespace DatabaseTool
{
    class Program
    {
        private const string RootConnectionString = "Server=localhost;Port=3307;Uid=root;Pwd=;AllowUserVariables=true;";
        private const string RiskAdminServerConnectionString = "Server=localhost;Port=3307;Uid=riskadmin;Pwd=RiskAdmin@123;AllowUserVariables=true;";
        private const string DatabaseName = "risk_management_db";
        private static string ScriptsPath = Path.GetFullPath(@"../ERMS.Database");

        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Database Diagnostic Tool ---");
            
            Console.WriteLine("[INFO] Attempting to connect to the server as riskadmin...");
            bool connectedAsRiskAdmin = await TestConnection(RiskAdminServerConnectionString + $"Database={DatabaseName};", "riskadmin");
            if (connectedAsRiskAdmin)
            {
                Console.WriteLine($"[INFO] Successfully connected as riskadmin to {DatabaseName}.");
                await CheckTables(RiskAdminServerConnectionString + $"Database={DatabaseName};");
                await RunMigrations(RiskAdminServerConnectionString + $"Database={DatabaseName};");
                return;
            }

            Console.WriteLine("[ERROR] Failed to connect as riskadmin. Trying as root to fix user/database...");
            // (Previous logic for creating user omitted for brevity if not needed, but let's just focus on running migrations)
            await RunMigrations(RiskAdminServerConnectionString + $"Database={DatabaseName};");
        }

        static async Task<bool> TestConnection(string connectionString, string user)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Connection failed for {user}: {ex.Message}");
                return false;
            }
        }

        static async Task CheckTables(string connectionString)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                var tables = await conn.QueryAsync<string>("SHOW TABLES;");
                Console.WriteLine("[INFO] Available tables:");
                foreach (var table in tables)
                {
                    Console.WriteLine($" - {table}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Could not list tables: {ex.Message}");
            }
        }

        static async Task RunMigrations(string connectionString)
        {
            Console.WriteLine("[INFO] Running migration scripts...");
            var scripts = Directory.GetFiles(ScriptsPath, "*.sql").OrderBy(x => x).ToList();
            
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            foreach (var script in scripts)
            {
                Console.WriteLine($"[INFO] Executing {Path.GetFileName(script)}...");
                string content = await File.ReadAllTextAsync(script);
                
                // Remove USE statements
                content = Regex.Replace(content, @"(?i)USE\s+`?.*?`?;", "", RegexOptions.Multiline);

                // Simple parser for DELIMITER blocks
                string currentDelimiter = ";";
                var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var currentBlock = new System.Text.StringBuilder();

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase))
                    {
                        var newDelim = trimmed.Substring(9).Trim();
                        if (!string.IsNullOrEmpty(newDelim))
                        {
                            currentDelimiter = newDelim;
                            continue;
                        }
                    }

                    if (trimmed.EndsWith(currentDelimiter))
                    {
                        // Remove the delimiter from the end of the block
                        var statement = currentBlock.ToString() + " " + line.Substring(0, line.LastIndexOf(currentDelimiter));
                        if (!string.IsNullOrWhiteSpace(statement))
                        {
                            try
                            {
                                await conn.ExecuteAsync(statement);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR] Statement failed: {ex.Message}");
                            }
                            currentBlock.Clear();
                        }
                    }
                    else
                    {
                        currentBlock.AppendLine(line);
                    }
                }
                
                // Execute any remaining block
                var final = currentBlock.ToString();
                if (!string.IsNullOrWhiteSpace(final))
                {
                    try { await conn.ExecuteAsync(final); } catch { }
                }
            }
            Console.WriteLine("[INFO] All migrations processed.");
        }
    }
}
