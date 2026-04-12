using MySqlConnector;
using Trey2.Models;

namespace Trey2.DB.IFunction
{
    public interface IUpdateFunction
    {
        //Update User
        public static async Task UpdateUser(User user, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(@"UPDATE `user` SET user_surname = @Surname, user_name = @Name,
                                                          user_fathername = @Fathername, user_height = @Height, user_gender = @Gender,
                                                          user_weight = @Weight, user_birthday = @Birthday, user_hr_rest = @HR_Rest, user_hr_max = @HR_Max, 
                                                          user_pace_max = @Pace_Max, user_only_coach = @Only_Coach WHERE user_id = @Id;", connection))
                {
                    command.Parameters.Add("@Surname", MySqlDbType.String).Value = user.user_surname;
                    command.Parameters.Add("@Name", MySqlDbType.String).Value = user.user_name;
                    command.Parameters.Add("@Fathername", MySqlDbType.String).Value = user.user_fathername;
                    command.Parameters.Add("@Height", MySqlDbType.Float).Value = user.user_height;
                    command.Parameters.Add("@Weight", MySqlDbType.Float).Value = user.user_weight;
                    command.Parameters.Add("@Birthday", MySqlDbType.Date).Value = user.user_birthday;
                    command.Parameters.Add("@HR_Rest", MySqlDbType.UByte).Value = user.user_hr_rest;
                    command.Parameters.Add("@HR_Max", MySqlDbType.UByte).Value = user.user_hr_max;
                    command.Parameters.Add("@Gender", MySqlDbType.Bool).Value = user.user_gender;
                    command.Parameters.Add("@Pace_Max", MySqlDbType.Float).Value = user.user_pace_max;
                    command.Parameters.Add("@Only_Coach", MySqlDbType.Bool).Value = user.user_only_coach;
                    command.Parameters.Add("@Id", MySqlDbType.UInt64).Value = user.id;

                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }

        //Update Training
        public static async Task UpdateTraining(Training training, ulong user_id, IDatabaseFactory factory)
        {
            training.avg_speed = training.distance is not null? training.distance / training.duration: null;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(@"UPDATE `training` SET sport_id = @Sport, type_training_id = @Type_Training,
                                                          name_training = @Name, duration = @Duration, distance = @Distance,
                                                          avg_speed = @Speed, calories = @Calories, stress_score = @Stress_score, feeling_score = @Feeling_score,
                                                          comment_sportsman = @Comment WHERE training_id = @Id AND sportsman_id = @user_id;", connection))
                {
                    command.Parameters.Add("@Sport", MySqlDbType.UByte).Value = training.sport_id;
                    command.Parameters.Add("@Type_Training", MySqlDbType.UByte).Value = training.type_training_id;
                    command.Parameters.Add("@Name", MySqlDbType.String).Value = (object?)training.name_training ?? DBNull.Value;
                    command.Parameters.Add("@Duration", MySqlDbType.Float).Value = training.duration;
                    command.Parameters.Add("@Distance", MySqlDbType.Float).Value = (object?)training.distance ?? DBNull.Value;
                    command.Parameters.Add("@Speed", MySqlDbType.Float).Value = (object?)training.avg_speed ?? DBNull.Value;
                    command.Parameters.Add("@Calories", MySqlDbType.UInt16).Value = training.calories;
                    command.Parameters.Add("@Stress_score", MySqlDbType.UByte).Value = training.stress_score;
                    command.Parameters.Add("@Feeling_score", MySqlDbType.UByte).Value = training.feeling_score;
                    command.Parameters.Add("@Comment", MySqlDbType.String).Value = (object?)training.comment_sportsman ?? DBNull.Value;
                    command.Parameters.Add("@Id", MySqlDbType.UInt64).Value = training.id;
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }
    }
}
