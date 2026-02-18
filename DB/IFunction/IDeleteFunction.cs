using MySqlConnector;

namespace Trey2.DB.IFunction
{
    public interface IDeleteFunction
    {
        // Удаление пользователя 
        public static async Task DeleteUser(ulong id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                await connection.OpenAsync();
                string query = $"DELETE FROM user WHERE user_id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id", MySqlDbType.UInt64).Value = id;
                    await command.ExecuteReaderAsync();
                }
                await connection.CloseAsync();
            }
        }
        // Удаление тренировки
        public static async Task DeleteTraining(ulong id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                await connection.OpenAsync();
                string query = $"DELETE FROM training WHERE training_id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id", MySqlDbType.UInt64).Value = id;
                    await command.ExecuteReaderAsync();
                }
                await connection.CloseAsync();
            }
        }
    }
}
