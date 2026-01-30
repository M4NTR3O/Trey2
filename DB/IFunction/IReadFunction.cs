using MySqlConnector;
using System.Data;
using Trey2.Models;

namespace Trey2.DB.IFunction
{
    public class IReadFunction
    {
        public static async Task<User?> GetUser(ulong id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                User? user = null;
                await connection.OpenAsync();
                string query = $"SELECT `user_id`, `user_surname`, `user_name`, `user_fathername`, `user_email`, `user_nickname`, `user_birthday`, `user_gender`, `user_height`, `user_weight`, `user_only_coach`, `user_hr_rest`, `user_hr_max`, `user_pace_max` FROM `user` WHERE `user_id`={id};";
                using (var command = new MySqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            id = ulong.Parse(reader[0].ToString()!),
                            user_surname = reader[1].ToString()!,
                            user_name = reader[2].ToString()!,
                            user_fathername = reader[3] != DBNull.Value ? reader[3].ToString() : null,
                            user_email = reader[4].ToString()!,
                            user_nickname = reader[5].ToString()!,
                            user_birthday = DateOnly.FromDateTime(DateTime.Parse(reader[6].ToString()!)),
                            user_gender = bool.Parse(reader[7].ToString()!),
                            user_height = float.Parse(reader[8].ToString()!),
                            user_weight = float.Parse(reader[9].ToString()!),
                            user_only_coach = bool.Parse(reader[10].ToString()!),
                            user_hr_rest = byte.Parse(reader[11].ToString()!),
                            user_hr_max = byte.Parse(reader[12].ToString()!),
                            user_pace_max = float.Parse(reader[13].ToString()!)
                        };
                    }
                }
                await connection.CloseAsync();
                return user;
            }

        }

        public static async Task<(ulong, bool)> GetAuthEmail(AuthRequest request, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT user_id, user_email, password FROM `user` WHERE user_email = @login;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", request.Login);
                    var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        bool valid = PasswordHasher.Verify(request.Password, reader[2].ToString()!);
                        if (valid /*reader[2].ToString() == password*/)
                        {
                            ulong id = Convert.ToUInt64(reader[0].ToString()!);
                            await connection.CloseAsync();
                            return (id, true);
                        }
                        else
                        {
                            await connection.CloseAsync();
                            return (0, true);
                        }
                    }
                    else
                    {
                        await connection.CloseAsync();
                        return (0, false);
                    }
                }
            }
        }

        public static async Task<Training> GetFullTraining(ulong id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using(var connection = database.CreateNewConnection())
            {
                Training? training = null;
                await connection.OpenAsync();
                string query = $"SELECT всё";
                using (var command = new MySqlCommand(query, connection))
                {
                    training = new Training {
                    
                    };
                }
                await connection.CloseAsync();
                //get trackpoints
                //get laps
                return training;
            }
        }

        public static async Task<Training?> GetMidTraining(ulong id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                Training? training = null;
                await connection.OpenAsync();
                string query = $"SELECT `training_id`, `sportsmen_id`, `sport_id`, `type_training_id`, `name_training`, `time_start`, " +
                    $"`duration`, `distance`, `avg_speed`, `total_ascent`, `calories`, `is_gps`, `local_timestamp` FROM `training` WHERE `training_id` = {id};";
                using (var command = new MySqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var dt = reader.GetDateTime("time_start");
                        Console.WriteLine(dt.Kind);
                        training = new Training
                        {
                            id = ulong.Parse(reader[0].ToString()!),
                            sportsmen_id = ulong.Parse(reader[1].ToString()!),
                            sport_id = byte.Parse(reader[2].ToString()!),
                            type_training_id = byte.Parse(reader[3].ToString()!),
                            name_training = reader[4] == DBNull.Value ? null: reader[4].ToString(),
                            time_start = reader.GetDateTime("time_start"),
                            duration = float.Parse(reader[6].ToString()!),
                            distance = reader[7] != DBNull.Value ? float.Parse(reader[7].ToString()!): null,
                            avg_speed = reader[8] != DBNull.Value ? float.Parse(reader[8].ToString()!) : null,
                            total_ascent = reader[9] != DBNull.Value ? ushort.Parse(reader[9].ToString()!) : null,
                            calories = reader[10] != DBNull.Value ? ushort.Parse(reader[10].ToString()!) : null,
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            is_gps = reader.GetBoolean("is_gps")
                        };
                    }
                }
                await connection.CloseAsync();
                if (training is not null && training.is_gps)
                {
                    training.trackpoints = await GetLowTrackpoint(id, factory);
                }
                return training;
            }
        }

        public static async Task<List<Trackpoint>> GetLowTrackpoint(ulong training_id, IDatabaseFactory factory)
        {
            var database = factory.Create();
            using (var connection = database.CreateNewConnection())
            {
                List<Trackpoint> trackpoints = new List<Trackpoint>();
                await connection.OpenAsync();
                string query = $"SELECT `duration_point`, `latitude`, `longitude` FROM `trackpoint` WHERE `training_id` = {training_id};";
                using (var command = new MySqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        Trackpoint tp = new Trackpoint
                        {
                            duration_point = reader.GetDateTime("duration_point"),
                            latitude = reader[1] != DBNull.Value ? float.Parse(reader[1].ToString()!): null,
                            longitude = reader[2] != DBNull.Value ? float.Parse(reader[2].ToString()!): null                            
                        };
                        if (tp is not null)
                        {
                            trackpoints.Add(tp);
                        }
                    }
                }
                await connection.CloseAsync();
                return trackpoints;
            }
        }
    }
}
