using MySqlConnector;
using Trey2.Models;
using Trey2.ViewModel;

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
                                                          user_pace_max = @Pace_Max, user_only_coach = @Only_Coach, like_coach = @Like_Coach WHERE user_id = @Id;", connection))
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
                    command.Parameters.Add("@Like_Coach", MySqlDbType.Bool).Value = user.like_coach;

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

        //Convert planned training to completed
        public static async Task UpdatePlanToCompleteTraining(Training training, ulong user_id, IDatabaseFactory factory)
        {
            training.avg_speed = training.distance is not null ? training.distance / training.duration : null;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    string query = @"UPDATE `training` SET 
                                                          duration = @Duration, distance = @Distance,
                                                          avg_speed = @Speed, calories = @Calories, stress_score = @Stress_score, feeling_score = @Feeling_score,
                                                          comment_sportsman = @Comment, status_training_id = 1, avg_heart_rate = @Avg_heart_rate, max_heart_rate = @Max_heart_rate, 
                                                          min_heart_rate = @Min_heart_rate, avg_speed = @Avg_speed, max_speed = @Max_speed, avg_running_cadence = @Avg_running_cadence,
                                                          max_running_cadence = @Max_running_cadence, avg_power = @Avg_power, max_power = @Max_power, start_position_lat = @Start_position_lat,
                                                          start_position_long = @Start_position_long, total_ascent = @Total_ascent, total_descent = @Total_descent, max_altitude = @Max_altitude, 
                                                          is_gps = @Is_gps WHERE training_id = @Id AND sportsman_id = @user_id AND status_training_id = 2;";
                    using (var command = new MySqlCommand(query, connection, transaction))
                    {
                        command.Parameters.Add("@Duration", MySqlDbType.Float).Value = training.duration;
                        command.Parameters.Add("@Distance", MySqlDbType.Float).Value = (object?)training.distance ?? DBNull.Value;
                        command.Parameters.Add("@Speed", MySqlDbType.Float).Value = (object?)training.avg_speed ?? DBNull.Value;
                        command.Parameters.Add("@Calories", MySqlDbType.UInt16).Value = training.calories;
                        command.Parameters.Add("@Stress_score", MySqlDbType.UByte).Value = training.stress_score;
                        command.Parameters.Add("@Feeling_score", MySqlDbType.UByte).Value = training.feeling_score;
                        command.Parameters.AddWithValue("@Avg_heart_rate", (object?)training.avg_heart_rate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Max_heart_rate", (object?)training.max_heart_rate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Min_heart_rate", (object?)training.min_heart_rate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Avg_speed", (object?)training.avg_speed ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Max_speed", (object?)training.max_speed ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Avg_running_cadence", (object?)training.avg_running_cadence ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Max_running_cadence", (object?)training.max_running_cadence ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Avg_power", (object?)training.avg_power ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Max_power", (object?)training.max_power ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Start_position_lat", (object?)training.start_position_lat ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Start_position_long", (object?)training.start_position_long ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Total_ascent", (object?)training.total_ascent ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Total_descent", (object?)training.total_descent ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Max_altitude", (object?)training.max_altitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Is_gps", training.is_gps);
                        command.Parameters.Add("@Comment", MySqlDbType.String).Value = (object?)training.comment_sportsman ?? DBNull.Value;
                        command.Parameters.Add("@Id", MySqlDbType.UInt64).Value = training.id;
                        command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                        int row = await command.ExecuteNonQueryAsync();
                        if (row == 0)
                        {
                            
                            throw new Exception("No execute row");
                        }
                    }

                    if (training.trackpoints is not null)
                    {
                        uint i = 1;
                        foreach (Trackpoint tp in training.trackpoints)
                        {
                            await ICreateFunction.createTrackpoint(training.id, tp, i, connection, transaction);
                            i++;
                        }
                    }
                    if (training.laps is not null)
                    {
                        ushort i = 1;
                        foreach (TrainingLap lap in training.laps)
                        {
                            await ICreateFunction.createTrainingLap(training.id, lap, i, connection, transaction);
                            i++;
                        }
                    }
                    await transaction.CommitAsync();

                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                


                await connection.CloseAsync();
            }
        }

        public static async Task UpdateTrainingWithoutFile(UpdatePlanTrainingWithoutFile training, ulong id_user, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"UPDATE training SET
                        time_start = @Time_start, 
                        local_timestamp = @Local_timestamp,
                        duration = @Duration, 
                        distance = @Distance, 
                        avg_heart_rate = @Avg_heart_rate, 
                        avg_speed = @Avg_speed, 
                        calories = @Calories, 
                        stress_score = @Stress_score,
                        feeling_score = @Feeling_score, 
                        comment_sportsman = @Comment_sportsman, 
                        status_training_id = 1 
                        WHERE training_id = @Training_id AND sportsman_id = @Sportsman AND status_training_id = 2;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Training_id", training.id);
                    command.Parameters.AddWithValue("@Sportsman", id_user);
                    command.Parameters.Add("@Time_start", MySqlDbType.DateTime).Value = training.time_start;
                    command.Parameters.Add("@Local_timestamp", MySqlDbType.DateTime).Value = training.local_timestamp;
                    command.Parameters.AddWithValue("@Duration", training.duration);
                    command.Parameters.AddWithValue("@Distance", (object?)training.distance ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_heart_rate", (object?)training.avg_heart_rate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_speed", (object?)training.avg_speed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Calories", (object?)training.calories ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Stress_score", (object?)training.stress_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Feeling_score", (object?)training.feeling_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comment_sportsman", (object?)training.comment_sportsman ?? DBNull.Value);
                    int row = await command.ExecuteNonQueryAsync();
                    if (row == 0)
                    {
                        await connection.CloseAsync();
                        throw new Exception("No execute row");
                    };
                }
                await connection.CloseAsync();
            }
        }
    }
}
