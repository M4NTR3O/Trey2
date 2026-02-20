using MySqlConnector;

namespace Trey2.DB.IFunction
{
    public interface IDeleteFunction
    {
        // Удаление пользователя 
        public static async Task<bool> DeleteUser(ulong id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                bool result;
                await connection.OpenAsync();
                string query = $"DELETE FROM user WHERE user_id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id", MySqlDbType.UInt64).Value = id;
                    result = await command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }
        // Удаление тренировки
        public static async Task<bool> DeleteTraining(ulong id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                bool result;
                await connection.OpenAsync();
                string query = $"DELETE FROM training WHERE training_id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id", MySqlDbType.UInt64).Value = id;
                    result = await command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }
    }
}
