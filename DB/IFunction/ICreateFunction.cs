using Trey2.Models;
using MySqlConnector;
using System.Linq.Expressions;
using System.Transactions;
using Trey2.ViewModel;

namespace Trey2.DB.IFunction
{
    public interface ICreateFunction
    {
        public static async Task createUser(Models.User _user, IDatabaseFactory factory)
        {
            long newId;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO user 
                            (user_surname, user_name, user_fathername, user_email, user_nickname, password, user_birthday, user_gender, user_height, user_weight, user_only_coach, user_hr_rest, user_hr_max, user_pace_max, like_coach) 
                            VALUES 
                            (@Surname, @Name, @Fathername, @Email, @Nickname, @Password, @Birthday, @Gender, @Height, @Weight, @Only_Coach, @Hr_Rest, @Hr_Max, @Pace_Max, @LikeCoach);";
                try
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Surname", _user.user_surname);
                        command.Parameters.AddWithValue("@Name", _user.user_name);
                        command.Parameters.AddWithValue("@Fathername", (object?)_user.user_fathername ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", _user.user_email);
                        command.Parameters.AddWithValue("@Nickname", _user.user_nickname.ToLower());
                        command.Parameters.AddWithValue("@Password", PasswordHasher.Hash(_user.password));
                        command.Parameters.Add("@Birthday", MySqlDbType.Date).Value = _user.user_birthday;
                        command.Parameters.AddWithValue("@Gender", _user.user_gender);
                        command.Parameters.AddWithValue("@Height", _user.user_height);
                        command.Parameters.AddWithValue("@Weight", _user.user_weight);
                        command.Parameters.AddWithValue("@Only_Coach", _user.user_only_coach);
                        command.Parameters.AddWithValue("@Hr_Rest", _user.user_hr_rest);
                        command.Parameters.AddWithValue("@Hr_Max", _user.user_hr_max);
                        command.Parameters.AddWithValue("@Pace_Max", (object?)_user.user_pace_max ?? DBNull.Value);
                        command.Parameters.AddWithValue("@LikeCoach", _user.like_coach);
                        await command.ExecuteNonQueryAsync();
                        newId = command.LastInsertedId;
                    }
                    await connection.CloseAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

        public static async Task createTrackpoint(ulong id, Models.Trackpoint trackpoint, uint i, MySqlConnection connection, MySqlTransaction transaction)
        {
            long newId;
            string query = @"INSERT INTO trackpoint 
                    (trackpoint_id, training_id, duration_point, distance, latitude, longitude, heart_rate, speed, cadence, power, altitude) 
                    VALUES 
                    (@Trackpoint_id, @Training, @Duration_Point, @Distance, @Latitude, @Longitude, @Heart_rate, @Speed, @Cadence, @Power, @Altitude);";
            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.Add("Trackpoint_id", MySqlDbType.UInt32).Value = i;
                command.Parameters.AddWithValue("@Training", id);
                command.Parameters.Add("@Duration_Point", MySqlDbType.DateTime).Value = trackpoint.duration_point;
                command.Parameters.AddWithValue("@Distance", (object?)trackpoint.distance ?? DBNull.Value);
                command.Parameters.AddWithValue("@Latitude", (object?)trackpoint.latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Longitude", (object?)trackpoint.longitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Heart_rate", (object?)trackpoint.heart_rate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Speed", (object?)trackpoint.speed ?? DBNull.Value);
                command.Parameters.AddWithValue("@Cadence", (object?)trackpoint.cadence ?? DBNull.Value);
                command.Parameters.AddWithValue("@Power", (object?)trackpoint.power ?? DBNull.Value);
                command.Parameters.AddWithValue("@Altitude", (object?)trackpoint.altitude ?? DBNull.Value);
                await command.ExecuteNonQueryAsync();
                newId = command.LastInsertedId;
            }
        }

        public static async Task createTrainingLap(ulong id, Models.TrainingLap lap, ushort i, MySqlConnection? connection, MySqlTransaction transaction)
        {
            long newId;
            string query = @"INSERT INTO training_lap 
                        (training_lap_id, training_id, lap_number, lap_caption, start_time, timestamp, duration, distance,
                        avg_heart_rate, max_heart_rate, min_heart_rate, avg_speed, max_speed, avg_running_cadence,
                        max_running_cadence, avg_power, max_power, total_ascent, total_descent) 
                        VALUES 
                        (@Lap_id, @Training_id, @Lap_number, @Lap_caption, @Start_time, @Timestamp, @Duration, @Distance,
                        @Avg_heart_rate, @Max_heart_rate, @Min_heart_rate, @Avg_speed, @Max_speed, @Avg_running_cadence,
                        @Max_running_cadence, @Avg_power, @Max_power, @Total_ascent, @Total_descent);";
            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.Add("Lap_id", MySqlDbType.UInt16).Value = i;
                command.Parameters.AddWithValue("@Training_id", id);
                command.Parameters.AddWithValue("@Lap_number", lap.lap_number);
                command.Parameters.AddWithValue("@Lap_caption", (object?)lap.lap_caption ?? DBNull.Value);
                command.Parameters.Add("@Start_time", MySqlDbType.DateTime).Value = lap.start_time;
                command.Parameters.Add("@Timestamp", MySqlDbType.DateTime).Value = lap.timestamp;
                command.Parameters.AddWithValue("@Duration", lap.duration);
                command.Parameters.AddWithValue("@Distance", (object?)lap.distance ?? DBNull.Value);
                command.Parameters.AddWithValue("@Avg_heart_rate", (object?)lap.avg_heart_rate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Max_heart_rate", (object?)lap.max_heart_rate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Min_heart_rate", (object?)lap.min_heart_rate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Avg_speed", (object?)lap.avg_speed ?? DBNull.Value);
                command.Parameters.AddWithValue("@Max_speed", (object?)lap.max_speed ?? DBNull.Value);
                command.Parameters.AddWithValue("@Avg_running_cadence", (object?)lap.avg_running_cadence ?? DBNull.Value);
                command.Parameters.AddWithValue("@Max_running_cadence", (object?)lap.max_running_cadence ?? DBNull.Value);
                command.Parameters.AddWithValue("@Avg_power", (object?)lap.avg_power ?? DBNull.Value);
                command.Parameters.AddWithValue("@Max_power", (object?)lap.max_power ?? DBNull.Value);
                command.Parameters.AddWithValue("@Total_ascent", (object?)lap.total_ascent ?? DBNull.Value);
                command.Parameters.AddWithValue("@Total_descent", (object?)lap.total_descent ?? DBNull.Value);
                await command.ExecuteNonQueryAsync();
                newId = command.LastInsertedId;
            }
        }

        public static async Task<ulong> createTraining(Models.Training training, IDatabaseFactory factory)
        {
            ulong newId = 0;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    string query = @"INSERT INTO training 
                            (sportsman_id, sport_id, type_training_id, name_training, time_start, local_timestamp,
                            duration, distance, avg_heart_rate, max_heart_rate, min_heart_rate, avg_speed, max_speed,
                            avg_running_cadence, max_running_cadence, avg_power, max_power, start_position_lat,
                            start_position_long, total_ascent, total_descent, max_altitude, calories, is_gps, stress_score,
                            feeling_score, comment_sportsman, coach_creator, comment_coach, status_training_id, description) 
                            VALUES 
                            (@Sportsman, @Sport, @Type_training, @Name_training, @Time_start, @Local_timestamp,
                            @Duration, @Distance, @Avg_heart_rate, @Max_heart_rate, @Min_heart_rate, @Avg_speed, @Max_speed,
                            @Avg_running_cadence, @Max_running_cadence, @Avg_power, @Max_power, @Start_position_lat,
                            @Start_position_long, @Total_ascent, @Total_descent, @Max_altitude, @Calories, @Is_gps, @Stress_score,
                            @Feeling_score, @Comment_sportsman, @Coach_creator, @Comment_coach, @Status_training, @Description);";
                    using (var command = new MySqlCommand(query, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Sportsman", training.sportsman_id);
                        command.Parameters.AddWithValue("@Sport", training.sport_id);
                        command.Parameters.AddWithValue("@Type_training", training.type_training_id);
                        command.Parameters.AddWithValue("@Name_training", (object?)training.name_training ?? DBNull.Value);
                        command.Parameters.Add("@Time_start", MySqlDbType.DateTime).Value = training.time_start;
                        command.Parameters.Add("@Local_timestamp", MySqlDbType.DateTime).Value = training.local_timestamp;
                        command.Parameters.AddWithValue("@Duration", training.duration);
                        command.Parameters.AddWithValue("@Distance", (object?)training.distance ?? DBNull.Value);
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
                        command.Parameters.AddWithValue("@Calories", (object?)training.calories ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Is_gps", training.is_gps);
                        command.Parameters.AddWithValue("@Stress_score", (object?)training.stress_score ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Feeling_score", (object?)training.feeling_score ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Comment_sportsman", (object?)training.comment_sportsman ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Coach_creator", (object?)training.coach_creator ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Comment_coach", (object?)training.comment_coach ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Status_training", (object?)training.status_training ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Description", (object?)training.description ?? DBNull.Value);
                        await command.ExecuteNonQueryAsync();
                        newId = (ulong)command.LastInsertedId;
                    }

                    if (training.trackpoints is not null)
                    {
                        uint i = 1;
                        foreach (Trackpoint tp in training.trackpoints)
                        {
                            await createTrackpoint(newId, tp, i, connection, transaction);
                            i++;
                        }
                    }
                    if (training.laps is not null)
                    {
                        ushort i = 1;
                        foreach (TrainingLap lap in training.laps)
                        {
                            await createTrainingLap(newId, lap, i, connection, transaction);
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
                return newId;
            }
        }

        public static async Task<ulong> createTrainingWithoutFile(TrainingCreate training, IDatabaseFactory factory)
        {
            ulong newId = 0;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO training 
                        (sportsman_id, sport_id, type_training_id, name_training, time_start, local_timestamp,
                        duration, distance, avg_heart_rate, max_heart_rate, min_heart_rate, avg_speed, max_speed,
                        avg_running_cadence, max_running_cadence, avg_power, max_power, start_position_lat,
                        start_position_long, total_ascent, total_descent, max_altitude, calories, is_gps, stress_score,
                        feeling_score, comment_sportsman, coach_creator, comment_coach, status_training_id, description) 
                        VALUES 
                        (@Sportsman, @Sport, @Type_training, @Name_training, @Time_start, @Local_timestamp,
                        @Duration, @Distance, @Avg_heart_rate, @Max_heart_rate, @Min_heart_rate, @Avg_speed, @Max_speed,
                        @Avg_running_cadence, @Max_running_cadence, @Avg_power, @Max_power, @Start_position_lat,
                        @Start_position_long, @Total_ascent, @Total_descent, @Max_altitude, @Calories, @Is_gps, @Stress_score,
                        @Feeling_score, @Comment_sportsman, @Coach_creator, @Comment_coach, @Status_training, @Description);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Sportsman", training.sportsman_id);
                    command.Parameters.AddWithValue("@Sport", training.sport_id);
                    command.Parameters.AddWithValue("@Type_training", training.type_training_id);
                    command.Parameters.AddWithValue("@Name_training", (object?)training.name_training ?? DBNull.Value);
                    command.Parameters.Add("@Time_start", MySqlDbType.DateTime).Value = training.time_start;
                    command.Parameters.Add("@Local_timestamp", MySqlDbType.DateTime).Value = training.local_timestamp;
                    command.Parameters.AddWithValue("@Duration", training.duration);
                    command.Parameters.AddWithValue("@Distance", (object?)training.distance ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_heart_rate", (object?)training.avg_heart_rate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Max_heart_rate", DBNull.Value);
                    command.Parameters.AddWithValue("@Min_heart_rate", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_speed", (object?)training.avg_speed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Max_speed", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_running_cadence", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_running_cadence", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_power", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_power", DBNull.Value);
                    command.Parameters.AddWithValue("@Start_position_lat", DBNull.Value);
                    command.Parameters.AddWithValue("@Start_position_long", DBNull.Value);
                    command.Parameters.AddWithValue("@Total_ascent", DBNull.Value);
                    command.Parameters.AddWithValue("@Total_descent", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_altitude", DBNull.Value);
                    command.Parameters.AddWithValue("@Calories", (object?)training.calories ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Is_gps", false);
                    command.Parameters.AddWithValue("@Stress_score", (object?)training.stress_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Feeling_score", (object?)training.feeling_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comment_sportsman", (object?)training.comment_sportsman ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Coach_creator", (object?)training.coach_creator ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comment_coach", (object?)training.comment_coach ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status_training", (object?)training.status_training ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object?)training.description ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                    newId = (ulong)command.LastInsertedId;
                }
                await connection.CloseAsync();
            }
            return newId;
        }

        public static async Task SaveRefreshToken(ulong userId, string refreshToken, IDatabaseFactory factory)
        {
            var hash = TokenHasher.Hash(refreshToken);

            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"
            INSERT INTO refresh_tokens(user_id, token_hash, expires_at)
            VALUES (@uid, @hash, @exp)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@hash", hash);
                    command.Parameters.AddWithValue("@exp", DateTime.UtcNow.AddDays(7));

                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
            
        }

        public static async Task<ulong> createPlannedTraining(TrainingCreate training, IDatabaseFactory factory)
        {
            ulong newId = 0;
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO training 
                        (sportsman_id, sport_id, type_training_id, name_training, time_start, local_timestamp,
                        duration, distance, avg_heart_rate, max_heart_rate, min_heart_rate, avg_speed, max_speed,
                        avg_running_cadence, max_running_cadence, avg_power, max_power, start_position_lat,
                        start_position_long, total_ascent, total_descent, max_altitude, calories, is_gps, stress_score,
                        feeling_score, comment_sportsman, coach_creator, comment_coach, status_training_id, description) 
                        VALUES 
                        (@Sportsman, @Sport, @Type_training, @Name_training, @Time_start, @Local_timestamp,
                        @Duration, @Distance, @Avg_heart_rate, @Max_heart_rate, @Min_heart_rate, @Avg_speed, @Max_speed,
                        @Avg_running_cadence, @Max_running_cadence, @Avg_power, @Max_power, @Start_position_lat,
                        @Start_position_long, @Total_ascent, @Total_descent, @Max_altitude, @Calories, @Is_gps, @Stress_score,
                        @Feeling_score, @Comment_sportsman, @Coach_creator, @Comment_coach, @Status_training, @Description);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Sportsman", training.sportsman_id);
                    command.Parameters.AddWithValue("@Sport", training.sport_id);
                    command.Parameters.AddWithValue("@Type_training", training.type_training_id);
                    command.Parameters.AddWithValue("@Name_training", (object?)training.name_training ?? DBNull.Value);
                    command.Parameters.Add("@Time_start", MySqlDbType.DateTime).Value = training.time_start;
                    command.Parameters.Add("@Local_timestamp", MySqlDbType.DateTime).Value = training.local_timestamp;
                    command.Parameters.AddWithValue("@Duration", training.duration);
                    command.Parameters.AddWithValue("@Distance", (object?)training.distance ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_heart_rate", (object?)training.avg_heart_rate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Max_heart_rate", DBNull.Value);
                    command.Parameters.AddWithValue("@Min_heart_rate", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_speed", (object?)training.avg_speed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Max_speed", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_running_cadence", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_running_cadence", DBNull.Value);
                    command.Parameters.AddWithValue("@Avg_power", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_power", DBNull.Value);
                    command.Parameters.AddWithValue("@Start_position_lat", DBNull.Value);
                    command.Parameters.AddWithValue("@Start_position_long", DBNull.Value);
                    command.Parameters.AddWithValue("@Total_ascent", DBNull.Value);
                    command.Parameters.AddWithValue("@Total_descent", DBNull.Value);
                    command.Parameters.AddWithValue("@Max_altitude", DBNull.Value);
                    command.Parameters.AddWithValue("@Calories", (object?)training.calories ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Is_gps", false);
                    command.Parameters.AddWithValue("@Stress_score", (object?)training.stress_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Feeling_score", (object?)training.feeling_score ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comment_sportsman", (object?)training.comment_sportsman ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Coach_creator", (object?)training.coach_creator ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comment_coach", (object?)training.comment_coach ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status_training", (object?)training.status_training ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object?)training.description ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                    newId = (ulong)command.LastInsertedId;
                }
                await connection.CloseAsync();
            }
            return newId;
        }
    }
}
