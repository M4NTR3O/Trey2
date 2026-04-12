using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Trey2.DB
{
    public class DatabaseFactory: IDatabaseFactory
    {
        private readonly string _connectionString;
        public DatabaseFactory(IConfiguration config)
        {
            var host = config["DB:HOST"];
            var port = config["DB:PORT"];
            var user = config["DB:USER"];
            var password = config["DB:PASSWORD"];
            var database = config["DB:NAME"];

            _connectionString = $"server={host};port={port};user={user};password={password};database={database};UseAffectedRows=true;";
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
