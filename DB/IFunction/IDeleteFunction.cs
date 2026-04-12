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
        public static async Task<bool> DeleteTraining(ulong id, ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                bool result;
                await connection.OpenAsync();
                string query = $"DELETE FROM training WHERE training_id = @id AND sportsman_id = @user_id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id", MySqlDbType.UInt64).Value = id;
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    result = await command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }


        public static async Task deleteRefresh(string refreshToken, IDatabaseFactory factory)
        {
            var hash = TokenHasher.Hash(refreshToken);
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"DELETE FROM refresh_tokens WHERE token_hash = @hash";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@hash", hash);
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }
    }
}
