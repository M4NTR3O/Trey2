using MySqlConnector;

namespace Trey2.DB
{
    public interface IDatabaseFactory
    {
        MySqlConnection CreateConnection();
    }
}
