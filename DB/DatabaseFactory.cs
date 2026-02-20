using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Trey2.DB
{
    public class DatabaseFactory: IDatabaseFactory
    {
        private readonly string _connectionString;
        public DatabaseFactory(IConfiguration config)
        {
            var host = config["Database:Host"];
            var port = config["Database:Port"];
            var user = config["Database:User"];
            var password = config["Database:Password"];
            var database = config["Database:Name"];

            _connectionString = $"server={host};port={port};user={user};password={password};database={database};UseAffectedRows=true;";
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
