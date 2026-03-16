using MySqlConnector;
using System.ComponentModel;
using System.Data;
using System.Reflection.PortableExecutable;
using Trey2.Models;
using Trey2.ViewModel;
using Trey2.ViewModels;

namespace Trey2.DB.IFunction
{
    public class IReadFunction
    {
        public static async Task<User?> GetUser(ulong user_id, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                User? user = null;
                await connection.OpenAsync();
                string query = @"SELECT `user_id`, `user_surname`, `user_name`, `user_fathername`, `user_email`," +
                    @" `user_nickname`, `user_birthday`, `user_gender`, `user_height`, `user_weight`, `user_only_coach`," +
                    @" `user_hr_rest`, `user_hr_max`, `user_pace_max` FROM `user` WHERE `user_id`=@user_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = user_id;
                    var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            id = reader.GetUInt64("user_id"),
                            user_surname = reader.GetString("user_surname"),
                            user_name = reader.GetString("user_name"),
                            user_fathername = reader.IsDBNull("user_fathername") ? null : reader.GetString("user_fathername"),
                            user_email = reader.GetString("user_email"),
                            user_nickname = reader.GetString("user_nickname"),
                            user_birthday = reader.GetDateOnly("user_birthday"),
                            user_gender = reader.GetBoolean("user_gender"),
                            user_height = reader.GetFloat("user_height"),
                            user_weight = reader.GetFloat("user_weight"),
                            user_only_coach = reader.GetBoolean("user_only_coach"),
                            user_hr_rest = reader.GetByte("user_hr_rest"),
                            user_hr_max = reader.GetByte("user_hr_max"),
                            user_pace_max = reader.IsDBNull("user_pace_max") ? null : reader.GetFloat("user_pace_max")
                        };
                    }
                    else
                    {
                        user = null;
                    }
                }
                await connection.CloseAsync();
                return user;
            }
        }

        public static async Task<(bool flag, User? user)> GetAuth(AuthRequest request, IDatabaseFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT `user_id`, `user_surname`, `user_name`, `user_fathername`, `user_email`," +
                    @" `user_nickname`, `user_birthday`, `user_gender`, `user_height`, `user_weight`, `user_only_coach`," +
                    @" `user_hr_rest`, `user_hr_max`, `user_pace_max`, `password` FROM `user` WHERE user_email = @login OR user_nickname = @login;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", request.Login);
                    var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        bool valid = PasswordHasher.Verify(request.Password, reader.GetString("password"));
                        if (!valid)
                        {
                            await connection.CloseAsync();
                            return (false, null);
                        }
                        User user = new User
                        {
                            id = reader.GetUInt64("user_id"),
                            user_surname = reader.GetString("user_surname"),
                            user_name = reader.GetString("user_name"),
                            user_fathername = reader.IsDBNull("user_fathername") ? null: reader.GetString("user_fathername"),
                            user_email = reader.GetString("user_email"),
                            user_nickname = reader.GetString("user_nickname"),
                            user_birthday = reader.GetDateOnly("user_birthday"),
                            user_gender = reader.GetBoolean("user_gender"),
                            user_height = reader.GetFloat("user_height"),
                            user_weight = reader.GetFloat("user_weight"),
                            user_only_coach = reader.GetBoolean("user_only_coach"),
                            user_hr_rest = reader.GetByte("user_hr_rest"),
                            user_hr_max = reader.GetByte("user_hr_max"),
                            user_pace_max = reader.IsDBNull("user_pace_max") ? null : reader.GetFloat("user_pace_max")
                        };
                        await connection.CloseAsync();
                        return (true, user);
                    }
                    else
                    {
                        await connection.CloseAsync();
                        return (false, null);
                    }
                }
            }
        }

        public static async Task<(ViewTrainingFull?, List<TrackpointGraph>)> GetFullTraining(ulong training_id, IDatabaseFactory factory)
        {
            
            using(var connection = factory.CreateConnection())
            {
                ViewTrainingFull? training = null;
                List<TrackpointGraph> result = new();
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `avg_speed`, `total_ascent`, `calories`, `is_gps`, `local_timestamp`, " +
                    @"`avg_heart_rate`, `max_heart_rate`, `min_heart_rate`, `max_speed`, `avg_running_cadence`," +
                    @"`max_running_cadence`, `avg_power`, `max_power`, `start_position_lat`, `start_position_long`," +
                    @"`total_descent`, `max_altitude`, `stress_score`, `feeling_score`, `comment_sportsman`," +
                    @"`coach_creator`, `comment_coach`, S.`status_caption` as status_caption FROM `training` AS T" +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id" +
                    @" INNER JOIN type_training AS TT ON TT.type_training_id = T.type_training_id" +
                    @" INNER JOIN sport AS SP ON SP.sport_id = T.sport_id" +
                    @" WHERE `training_id` = @training_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        training = new ViewTrainingFull
                        {
                            id = reader.GetUInt64("training_id"),
                            sportsman_id = reader.GetUInt64("sportsman_id"),
                            //Поправить после проверки
                            sport = reader.GetString("sport"),
                            type_training = reader.GetString("type_training"),
                            name_training = reader.IsDBNull("name_training") ? null : reader.GetString("name_training"),
                            time_start = reader.GetDateTime("time_start"),
                            duration = reader.GetFloat("duration"),
                            distance = reader.IsDBNull("distance") ? null : reader.GetFloat("distance"),
                            avg_speed = reader.IsDBNull("avg_speed") ? null : reader.GetFloat("avg_speed"),
                            total_ascent = reader.IsDBNull("total_ascent") ? null : reader.GetUInt16("total_ascent"),
                            calories = reader.IsDBNull("calories") ? null : reader.GetUInt16("calories"),
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            is_gps = reader.GetBoolean("is_gps"),
                            avg_heart_rate = reader.IsDBNull("avg_heart_rate") ? null : reader.GetByte("avg_heart_rate"),
                            max_heart_rate = reader.IsDBNull("max_heart_rate") ? null : reader.GetByte("max_heart_rate"),
                            min_heart_rate = reader.IsDBNull("min_heart_rate") ? null : reader.GetByte("min_heart_rate"),
                            max_speed = reader.IsDBNull("max_speed") ? null : reader.GetFloat("max_speed"),
                            avg_running_cadence = reader.IsDBNull("avg_running_cadence") ? null : reader.GetUInt16("avg_running_cadence"),
                            max_running_cadence = reader.IsDBNull("max_running_cadence") ? null : reader.GetUInt16("max_running_cadence"),
                            avg_power = reader.IsDBNull("avg_power") ? null : reader.GetUInt16("avg_power"),
                            max_power = reader.IsDBNull("max_power") ? null : reader.GetUInt16("max_power"),
                            start_position_lat = reader.IsDBNull("start_position_lat") ? null : reader.GetFloat("start_position_lat"),
                            start_position_long = reader.IsDBNull("start_position_long") ? null : reader.GetFloat("start_position_long"),
                            total_descent = reader.IsDBNull("total_descent") ? null : reader.GetUInt16("total_descent"),
                            max_altitude = reader.IsDBNull("max_altitude") ? null : reader.GetInt16("max_altitude"),
                            stress_score = reader.IsDBNull("stress_score") ? null : reader.GetByte("stress_score"),
                            feeling_score = reader.IsDBNull("feeling_score") ? null : reader.GetByte("feeling_score"),
                            comment_sportsman = reader.IsDBNull("comment_sportsman") ? null : reader.GetString("comment_sportsman"),
                            coach_creator = reader.IsDBNull("coach_creator") ? null : reader.GetUInt64("coach_creator"),
                            comment_coach = reader.IsDBNull("comment_coach") ? null : reader.GetString("comment_coach"),
                            status_training = reader.IsDBNull("status_caption") ? null : reader.GetString("status_caption")
                        };
                    }
                    await reader.CloseAsync();
                }
                
                
                if (training is not null)
                {
                    result = await GetFullTrackpoint(training_id, connection);
                    training.laps = await GetLapsTraining(training_id, connection);
                }
                await connection.CloseAsync();
                return (training, result);
            }
        }

        /*public static async Task<List<Altitudes>> GetAltitudes(ulong training_id, MySqlConnection? connection)
        {
            List<Altitudes> trackpoints = new List<Altitudes>();
            string query = @"SELECT `duration_point`, `altitude` FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull("altitude"))
                    {
                        Altitudes tp = new Altitudes
                        {
                            timestamp = reader.GetDateTime("duration_point"),
                            altitude = reader.GetFloat("altitude")
                        };
                        if (tp is not null)
                        {
                            trackpoints.Add(tp);
                        }
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }*/

        /*public static async Task<List<HeartRates>> GetHeartRates(ulong training_id, MySqlConnection? connection)
        {
            List<HeartRates> trackpoints = new List<HeartRates>();
            string query = @"SELECT `duration_point`, `heart_rate` FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull("heart_rate"))
                    {
                        HeartRates tp = new HeartRates
                        {
                            timestamp = reader.GetDateTime("duration_point"),
                            heart_rate = reader.GetByte("heart_rate")
                        };
                        if (tp is not null)
                        {
                            trackpoints.Add(tp);
                        }
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }*/

        /*public static async Task<List<Paces>> GetPaces(ulong training_id, MySqlConnection? connection)
        {
            List<Paces> trackpoints = new List<Paces>();
            string query = @"SELECT `duration_point`, `speed` FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull("speed"))
                    {
                        Paces tp = new Paces
                        {
                            timestamp = reader.GetDateTime("duration_point"),
                            pace = reader.GetFloat("speed")
                        };
                        if (tp is not null)
                        {
                            trackpoints.Add(tp);
                        }
                  
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }*/

        /*public static async Task<List<Powers>> GetPowers(ulong training_id, MySqlConnection? connection)
        {
            List<Powers> trackpoints = new List<Powers>();
            string query = @"SELECT `duration_point`, `power` FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull("power"))
                    {
                        Powers tp = new Powers
                        {
                            timestamp = reader.GetDateTime("duration_point"),
                            power = reader.GetUInt16("power")
                        };
                        if (tp is not null)
                        {
                            trackpoints.Add(tp);
                        }
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }*/

        public static async Task<ViewTrainingMid?> GetMidTraining(ulong training_id, IDatabaseFactory factory)
        {
            
            using (var connection = factory.CreateConnection())
            {
                ViewTrainingMid? training = null;
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `avg_speed`, `total_ascent`, `calories`, `is_gps`, `local_timestamp`, S.`status_caption` as status_caption FROM `training` AS T" +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id" +
                    @" INNER JOIN type_training AS TT ON TT.type_training_id = T.type_training_id" +
                    @" INNER JOIN sport AS SP ON SP.sport_id = T.sport_id" +
                    @" WHERE `training_id` = @training_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var dt = reader.GetDateTime("time_start");
                        Console.WriteLine(dt.Kind);
                        training = new ViewTrainingMid
                        {
                            id = reader.GetUInt64("training_id"),
                            sportsman_id = reader.GetUInt64("sportsman_id"),
                            sport = reader.GetString("sport"),
                            type_training = reader.GetString("type_training"),
                            name_training = reader.IsDBNull("name_training") ? null : reader.GetString("name_training"),
                            time_start = reader.GetDateTime("time_start"),
                            duration = reader.GetFloat("duration"),
                            distance = reader.IsDBNull("distance") ? null : reader.GetFloat("distance"),
                            avg_speed = reader.IsDBNull("avg_speed") ? null : reader.GetFloat("avg_speed"),
                            total_ascent = reader.IsDBNull("total_ascent") ? null : reader.GetUInt16("total_ascent"),
                            calories = reader.IsDBNull("calories") ? null : reader.GetUInt16("calories"),
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            is_gps = reader.GetBoolean("is_gps"),
                            status_training = reader.GetString("status_caption")
                        };
                    }
                    await reader.CloseAsync();
                }
                if (training is not null && training.is_gps)
                {
                    training.trackpoints = await GetLowTrackpoint(training_id, connection);

                }
                await connection.CloseAsync();
                return training;
            }
        }

        public static async Task<List<TrackpointView>> GetLowTrackpoint(ulong training_id, MySqlConnection? connection)
        {
            
            List<TrackpointView> trackpoints = new List<TrackpointView>();
            string query = @"SELECT `duration_point`, `latitude`, `longitude` FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    TrackpointView tp = new TrackpointView
                    {
                        duration_point = reader.GetDateTime("duration_point"),
                        latitude = reader.IsDBNull("latitude") ? null : reader.GetFloat("latitude"),
                        longitude = reader.IsDBNull("longitude") ? null : reader.GetFloat("longitude")                            
                    };
                    if (tp is not null)
                    {
                        trackpoints.Add(tp);
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }

        public static async Task<List<TrackpointGraph>> GetFullTrackpoint(ulong training_id, MySqlConnection? connection)
        {
            List<TrackpointGraph> trackpoints = new List<TrackpointGraph>();
            string query = @"SELECT `duration_point`, `latitude`, `longitude`," +
                @"`distance`, `heart_rate`, `speed`, `cadence`, `power`, `altitude`" +
                @" FROM `trackpoint` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    TrackpointGraph tp = new TrackpointGraph
                    {
                        TimeStartDt = reader.GetDateTime("duration_point"),
                        latitude = reader.IsDBNull("latitude") ? null : reader.GetFloat("latitude"),
                        longitude = reader.IsDBNull("longitude") ? null : reader.GetFloat("longitude"),
                        distance = reader.IsDBNull("distance") ? null : reader.GetFloat("distance"),
                        heart_rate = reader.IsDBNull("heart_rate") ? null : reader.GetByte("heart_rate"),
                        speed = reader.IsDBNull("speed") ? null : reader.GetFloat("speed"),
                        cadence = reader.IsDBNull("cadence") ? null : reader.GetUInt16("cadence"),
                        power = reader.IsDBNull("power") ? null :reader.GetUInt16("power"),
                        altitude = reader.IsDBNull("altitude") ? null : reader.GetFloat("altitude")
                    };
                    if (tp is not null)
                    {
                        trackpoints.Add(tp);
                    }
                }
                await reader.CloseAsync();
            }
            return trackpoints;
        }

        public static async Task<List<TrainingLap>> GetLapsTraining(ulong training_id, MySqlConnection? connection)
        {
            List<TrainingLap> laps = new List<TrainingLap>();
            string query = $"SELECT `lap_number`, `lap_caption`, `start_time`," +
                $"`timestamp`, `duration`, `distance`, `avg_heart_rate`, `max_heart_rate`," +
                $"`min_heart_rate`, `max_speed`, `avg_running_cadence`,`max_running_cadence`," +
                $" `avg_power`, `max_power`, `total_ascent`, `total_descent`" +
                $" FROM `training_lap` WHERE `training_id` = @training_id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    TrainingLap lap = new TrainingLap
                    {
                        lap_number = reader.GetInt32("lap_number"),
                        lap_caption = reader.IsDBNull("lap_caption")? null : reader.GetString("lap_caption"),
                        start_time = reader.GetDateTime("start_time"),
                        timestamp = reader.GetDateTime("timestamp"),
                        duration = reader.GetFloat("duration"),
                        distance = reader.IsDBNull("distance") ? null : reader.GetFloat("distance"),
                        avg_heart_rate = reader.IsDBNull("avg_heart_rate") ? null : reader.GetByte("avg_heart_rate"),
                        max_heart_rate = reader.IsDBNull("max_heart_rate") ? null : reader.GetByte("max_heart_rate"),
                        min_heart_rate = reader.IsDBNull("min_heart_rate") ? null : reader.GetByte("min_heart_rate"),
                        max_speed = reader.IsDBNull("max_speed") ? null : reader.GetFloat("max_speed"),
                        avg_running_cadence = reader.IsDBNull("avg_running_cadence") ? null : reader.GetUInt16("avg_running_cadence"),
                        max_running_cadence = reader.IsDBNull("max_running_cadence") ? null : reader.GetUInt16("max_running_cadence"),
                        avg_power = reader.IsDBNull("avg_power") ? null : reader.GetUInt16("avg_power"),
                        max_power = reader.IsDBNull("max_power") ? null : reader.GetUInt16("max_power"),
                        total_ascent = reader.IsDBNull("total_ascent") ? null : reader.GetUInt16("total_ascent"),
                        total_descent = reader.IsDBNull("total_descent") ? null : reader.GetUInt16("total_descent"),
                    };
                    if (lap is not null)
                    {
                        laps.Add(lap);
                    }
                }
                await reader.CloseAsync();
            }
            return laps;
        }

        public static async Task<List<LowTraining>>GetPeriodTrainings(ulong sportsman_id, DateTime start, DateTime end, IDatabaseFactory factory)
        {
            
            end = end.AddDays(1);
            using(var connection = factory.CreateConnection())
            {
                List<LowTraining> trainings = new List<LowTraining>();
                await connection.OpenAsync();
                string query = @"SELECT training_id, name_training, sport_id, duration, distance, local_timestamp, S.status_caption as status_caption FROM training AS T " +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id " +
                    @"WHERE sportsman_id = @sportsman_id AND time_start >= @start AND time_start < @end";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("@sportsman_id", MySqlDbType.UInt64).Value = sportsman_id;
                    command.Parameters.Add("@start", MySqlDbType.DateTime).Value = start;
                    command.Parameters.Add("@end", MySqlDbType.DateTime).Value = end;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        LowTraining training = new LowTraining
                        {
                            id = reader.GetUInt64("training_id"),
                            sport_id = reader.GetByte("sport_id"),
                            name_training = reader.IsDBNull("name_training") ? null : reader.GetString("name_training"),
                            duration = reader.GetFloat("duration"),
                            distance = reader.IsDBNull("distance") ? null : reader.GetFloat("distance"),
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            status_training = reader.GetString("status_caption")
                        };
                        if (training is not null)
                        {
                            trainings.Add(training);
                        }
                    }
                }
                await connection.CloseAsync();
                return trainings;
            }
        }

        public static async Task<List<Sport>> GetSports(IDatabaseFactory factory)
        {
            List<Sport> sports = new List<Sport>();
            
            using (var connecton = factory.CreateConnection())
            {
                await connecton.OpenAsync();
                string query = "SELECT `sport_id`, `sport_name` FROM `sport`";
                using (var command = new MySqlCommand(query, connecton))
                {
                    var reader = command.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        Sport sport = new Sport
                        {
                            id = reader.GetByte("sport_id"),
                            sport_name = reader.GetString("sport_name")
                        };
                        if (sport is not null)
                        {
                            sports.Add(sport);
                        }
                    }
                }
            }
            return sports;
        }

        public static async Task<List<TypeTraining>> GetTypesTraining(IDatabaseFactory factory)
        {
            List<TypeTraining> types = new List<TypeTraining>();
            
            using (var connecton = factory.CreateConnection())
            {
                await connecton.OpenAsync();
                string query = "SELECT `type_training_id`, `type_training_name` FROM `type_training`";
                using (var command = new MySqlCommand(query, connecton))
                {
                    var reader = command.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        TypeTraining type = new TypeTraining
                        {
                            id = reader.GetByte("type_training_id"),
                            type_training_name = reader.GetString("type_training_name")
                        };
                        if (type is not null)
                        {
                            types.Add(type);
                        }
                    }
                }
            }
            return types;
        }

        public static async Task<List<UserZones>> GetAllUserZones(ulong id_user, IDatabaseFactory factory)
        {
            List<UserZones> userZones = new List<UserZones>();
            
            using(var connection = factory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT `zone_id`, `zone_number`, `low_limit`, `high_limit`, `type_zone` FROM `user_zones` WHERE `user_id` = @User";
                using(var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("@User", MySqlDbType.UInt64).Value = id_user;
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        UserZones user_zone = new UserZones
                        {
                            user_id = id_user,
                            id = reader.GetUInt64("zone_id"),
                            zone_number = reader.GetByte("zone_number"),
                            low_limit = reader.GetDouble("low_limit"),
                            high_limit = reader.GetDouble("high_limit"),
                            type_zone = reader.GetByte("type_zone")
                        };
                        if (user_zone is not null)
                        {
                            userZones.Add(user_zone);
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return userZones;
        }
    }
}
