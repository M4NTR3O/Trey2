using BCrypt.Net;
using MySqlConnector;

namespace Trey2.DB.IFunction
{
    public interface IPatchFunction
    {
        //Patch Lap Caption
        public static async Task<bool> PatchLapCaption(int id_lap, ulong id_training, string caption_lap, IDatabaseFactory factory)
        {
            bool result;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();

                string update_query = @"UPDATE training_lap SET lap_caption = @caption WHERE training_lap_id = @id_lap AND training_id = @id_training;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id_lap", MySqlDbType.UInt32).Value = id_lap;
                    update_command.Parameters.Add("@id_training", MySqlDbType.UInt64).Value = id_training;
                    update_command.Parameters.Add("@caption", MySqlDbType.String).Value = caption_lap;
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch email
        public static async Task<sbyte> PatchUserEmail(ulong id, string email, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                sbyte result;
                string update_query = @"UPDATE user SET user_email = @email WHERE user_id = @id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@email", MySqlDbType.String).Value = email;
                    try
                    {
                        result = (sbyte)Convert.ToInt32(await update_command.ExecuteNonQueryAsync() > 0);
                    }
                    catch (MySqlException ex) when (ex.Number == 1062)
                    {
                        result = -1;
                    }
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch nickname
        public static async Task<sbyte> PatchUserNickname(ulong id, string nickname, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                sbyte result;
                string update_query = @"UPDATE user SET user_nickname = @nickname WHERE user_id = @id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@nickname", MySqlDbType.String).Value = nickname;
                    try
                    {
                        result = (sbyte)Convert.ToInt32(await update_command.ExecuteNonQueryAsync() > 0);
                    }
                    catch (MySqlException ex) when (ex.Number == 1062)
                    {
                        result = -1;
                    }
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch Password
        public static async Task<bool> PatchPassword(ulong id, string last_password, string new_password, IDatabaseFactory factory)
        {
            bool result;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();

                string select_query = @"SELECT password FROM user WHERE user_id=@Id";
                using (var select_command = new MySqlCommand(select_query, connection))
                {
                    select_command.Parameters.Add("@Id", MySqlDbType.UInt64).Value = id;
                    var reader = await select_command.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                    {
                        await connection.CloseAsync();
                        return false;
                    }
                    string hash_password = reader.GetString("password");
                    if (!BCrypt.Net.BCrypt.Verify(last_password, hash_password))
                    {
                        await connection.CloseAsync();
                        return false;
                    }
                    await reader.CloseAsync();
                }

                string update_query = @"UPDATE user SET password = @password WHERE user_id = @id;";
                using(var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@password", MySqlDbType.String).Value = BCrypt.Net.BCrypt.HashPassword(new_password);
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch Training Name
        public static async Task<bool> PatchNameTraining(ulong id, string name_training, ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                bool result;
                string update_query = @"UPDATE training SET name_training = @name WHERE training_id = @id AND sportsman_id = @user_id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@name", MySqlDbType.String).Value = name_training;
                    update_command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch Training Stress Score
        public static async Task<bool> PatchStressScoreTraining(ulong id, byte stress_score, ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                bool result;
                string update_query = @"UPDATE training SET stress_score = @score WHERE training_id = @id AND sportsman_id = @user_id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@score", MySqlDbType.UByte).Value = stress_score;
                    update_command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch Training Feeling Score
        public static async Task<bool> PatchFeelingScoreTraining(ulong id, byte feeling_score, ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                bool result;
                string update_query = @"UPDATE training SET feeling_score = @score WHERE training_id = @id AND sportsman_id = @user_id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@score", MySqlDbType.UByte).Value = feeling_score;
                    update_command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }

        //Patch Sportsmen comment
        public static async Task<bool> PatchCommentSportsmanTraining(ulong id, string comment_sportsman, ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                bool result;
                string update_query = @"UPDATE training SET comment_sportsman = @comment WHERE training_id = @id AND sportsman_id = @user_id;";
                using (var update_command = new MySqlCommand(update_query, connection))
                {
                    update_command.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    update_command.Parameters.Add("@comment", MySqlDbType.String).Value = comment_sportsman;
                    update_command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    result = await update_command.ExecuteNonQueryAsync() > 0;
                }
                await connection.CloseAsync();
                return result;
            }
        }
    }
}
