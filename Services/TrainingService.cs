using MySqlConnector;
using Trey2.DB;
using Trey2.Services.Interfaces;
using Trey2.ViewModel.Report;
using Trey2.ViewModel;
using System.Data;
using Trey2.ViewModels;
using Trey2.Models;

namespace Trey2.Services
{
    public class TrainingService: ITrainingService
    {
        private readonly IDatabaseFactory _databaseFactory;

        public TrainingService(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }
        public async Task<ReportPage?> GetReportPage(ulong id_sportsman, DateTime start, DateTime end, string bin)
        {
            end.AddDays(1);
            string? query2 = GetQueryBin(bin);
            if (query2 == null)
            {
                return null;
            }
            ReportPage result;
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                //1 запрос
                string query1 = @"SELECT COUNT(*) AS session, SUM(distance) AS total_distance, SUM(duration) AS total_duration, " +
                              @"SUM(calories) AS total_calories, (SUM(distance)/SUM(duration)) AS avg_pace, " +
                              @"SUM(total_ascent) AS total_ascent, SUM(total_descent) AS total_descent FROM training " +
                              @"WHERE sportsman_id = @sportsman_id AND local_timestamp >= @start_date AND local_timestamp < @end_date AND status_training_id = 1;";
                ReportOverview overview = new ReportOverview();
                using (var command = new MySqlCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@sportsman_id", MySqlDbType.UInt64).Value = id_sportsman;
                    command.Parameters.Add("@start_date", MySqlDbType.DateTime).Value = start;
                    command.Parameters.Add("@end_date", MySqlDbType.DateTime).Value = end;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        overview = new ReportOverview
                        {
                            total_trainings = reader.IsDBNull("session") ? 0 : reader.GetUInt32("session"),
                            total_calories = reader.IsDBNull("total_calories") ? 0 : reader.GetInt32("total_calories"),
                            total_distance = reader.IsDBNull("total_distance") ? null : reader.GetDouble("total_distance"),
                            total_duration = reader.IsDBNull("total_duration") ? 0 : reader.GetDouble("total_duration"),
                            avg_pace = reader.IsDBNull("avg_pace") ? null : reader.GetDouble("avg_pace"),
                            total_ascent = reader.IsDBNull("total_ascent") ? null : reader.GetInt32("total_ascent"),
                            total_descent = reader.IsDBNull("total_descent") ? null : reader.GetInt32("total_descent"),
                        };
                    }
                    await reader.CloseAsync();
                }
                List<ReportBin> bins = new List<ReportBin>();
                using (var command = new MySqlCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@id_user", MySqlDbType.UInt64).Value = id_sportsman;
                    command.Parameters.Add("@start", MySqlDbType.DateTime).Value = start;
                    command.Parameters.Add("@end", MySqlDbType.DateTime).Value = end;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ReportBin bin_report = new ReportBin
                        {
                            bin_period = reader.GetValue("bin_period").ToString(),
                            total_trainings = reader.IsDBNull("session") ? 0 : reader.GetUInt32("session"),
                            total_distance = reader.IsDBNull("total_distance") ? null : reader.GetDouble("total_distance"),
                            total_duration = reader.IsDBNull("total_duration") ? 0 : reader.GetDouble("total_duration"),
                            avg_pace = reader.IsDBNull("avg_pace") ? null : reader.GetDouble("avg_pace"),
                            total_ascent = reader.IsDBNull("total_ascent") ? null : reader.GetInt32("total_ascent"),
                            total_descent = reader.IsDBNull("total_descent") ? null : reader.GetInt32("total_descent"),
                            max_speed = reader.IsDBNull("max_speed") ? null : reader.GetDouble("max_speed"),
                            avg_heart_rate = reader.IsDBNull("avg_heart_rate") ? null : reader.GetByte("avg_heart_rate")
                        };
                        if (bin_report != null)
                        {
                            bins.Add(bin_report);
                        }
                    }
                    await reader.CloseAsync();
                }
                List<ReportPie> report_type_pies = new List<ReportPie>();
                //3 запрос
                string query3 = @"SELECT TT.type_training_name AS caption, COUNT(*) AS count_session FROM training AS T INNER JOIN type_training AS TT ON T.type_training_id = TT.type_training_id " +
                                @"WHERE sportsman_id = @user_id AND local_timestamp >= @start_date AND local_timestamp < @end_date AND status_training_id = 1 GROUP BY T.type_training_id ORDER BY T.type_training_id;";
                using (var command = new MySqlCommand(query3, connection))
                {
                    command.Parameters.AddWithValue("@user_id", MySqlDbType.UInt64).Value = id_sportsman;
                    command.Parameters.Add("@start_date", MySqlDbType.DateTime).Value = start;
                    command.Parameters.Add("@end_date", MySqlDbType.DateTime).Value = end;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ReportPie pie = new ReportPie
                        {
                            caption = reader.GetString("caption"),
                            value = reader.GetInt32("count_session")
                        };
                        if (pie != null)
                        {
                            report_type_pies.Add(pie);
                        }
                    }
                    await reader.CloseAsync();
                }
                List<ReportPie> sport_session = new List<ReportPie>();
                List<ReportPie> sport_distance = new List<ReportPie>();
                List<ReportPie> sport_duration = new List<ReportPie>();
                //4 запрос
                string query4 = @"SELECT S.sport_name AS caption, COUNT(*) AS count_session, SUM(distance) AS distance, SUM(duration) AS duration FROM training AS T INNER JOIN sport AS S ON T.sport_id = S.sport_id " +
                                @"WHERE sportsman_id = @user_id AND local_timestamp >= @start_date AND local_timestamp < @end_date AND status_training_id = 1 GROUP BY T.sport_id ORDER BY T.sport_id;";
                using (var command = new MySqlCommand(query4, connection))
                {
                    command.Parameters.AddWithValue("@user_id", MySqlDbType.UInt64).Value = id_sportsman;
                    command.Parameters.Add("@start_date", MySqlDbType.DateTime).Value = start;
                    command.Parameters.Add("@end_date", MySqlDbType.DateTime).Value = end;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ReportPie pie_session = new ReportPie
                        {
                            caption = reader.GetString("caption"),
                            value = reader.GetInt32("count_session")
                        };
                        ReportPie pie_distance = new ReportPie
                        {
                            caption = reader.GetString("caption"),
                            value = reader.IsDBNull("distance") ? null : reader.GetDouble("distance")
                        };
                        ReportPie pie_duration = new ReportPie
                        {
                            caption = reader.GetString("caption"),
                            value = reader.GetDouble("duration")
                        };
                        if (pie_session != null)
                        {
                            sport_session.Add(pie_session);
                        }
                        if (pie_distance != null)
                        {
                            sport_distance.Add(pie_distance);
                        }
                        if (pie_duration != null)
                        {
                            sport_duration.Add(pie_duration);
                        }
                    }
                    await reader.CloseAsync();
                }
                result = new ReportPage
                {
                    overview = overview,
                    reportBins = bins,
                    typePie = report_type_pies,
                    sportSessionPie = sport_session,
                    sportDistancePie = sport_distance,
                    sportDurationPie = sport_duration
                };
                await connection.CloseAsync();
            }
            return result;
        }

        private string? GetQueryBin(string flag)
        {
            string? query;
            if (flag == "day")
            {
                query = @"SELECT DATE(local_timestamp) AS bin_period, COUNT(*) AS `session`, SUM(`distance`) AS total_distance, " +
                    @"SUM(duration) AS total_duration, (SUM(`distance`)/SUM(duration)) AS avg_pace, MAX(max_speed) AS max_speed, " +
                    @"AVG(avg_heart_rate) AS avg_heart_rate, SUM(total_ascent) AS total_ascent, SUM(total_descent) AS total_descent " +
                    @"FROM training WHERE sportsman_id = @id_user AND local_timestamp >= @start AND local_timestamp < @end AND status_training_id = 1 GROUP BY DATE(local_timestamp) ORDER BY bin_period;";
            }
            else if (flag == "week")
            {
                query = @"SELECT CONCAT(LEFT(YEARWEEK(local_timestamp, 1), 4), '-', RIGHT(YEARWEEK(local_timestamp, 1), 2)) AS bin_period, COUNT(*) AS `session`, SUM(`distance`) AS total_distance, " +
                    @"SUM(duration) AS total_duration, (SUM(`distance`)/SUM(duration)) AS avg_pace, MAX(max_speed) AS max_speed, " +
                    @"AVG(avg_heart_rate) AS avg_heart_rate, SUM(total_ascent) AS total_ascent, SUM(total_descent) AS total_descent " +
                    @"FROM training WHERE sportsman_id = @id_user AND local_timestamp >= @start AND local_timestamp < @end AND status_training_id = 1 GROUP BY CONCAT(LEFT(YEARWEEK(local_timestamp, 1), 4), '-', RIGHT(YEARWEEK(local_timestamp, 1), 2)) ORDER BY bin_period;";
            }
            else if (flag == "month")
            {
                query = @"SELECT DATE_FORMAT(local_timestamp, '%Y-%m') AS bin_period, COUNT(*) AS `session`, SUM(`distance`) AS total_distance, " +
                    @"SUM(duration) AS total_duration, (SUM(`distance`)/SUM(duration)) AS avg_pace, MAX(max_speed) AS max_speed, " +
                    @"AVG(avg_heart_rate) AS avg_heart_rate, SUM(total_ascent) AS total_ascent, SUM(total_descent) AS total_descent " +
                    @"FROM training WHERE sportsman_id = @id_user AND local_timestamp >= @start AND local_timestamp < @end AND status_training_id = 1 GROUP BY DATE_FORMAT(local_timestamp, '%Y-%m') ORDER BY bin_period;";
            }
            else
            {
                query = null;
            }
            return query;
        }

        public async Task<ViewTrainingMid?> GetMidTraining(ulong training_id, ulong id_sportsman)
        {
            using (var connection = _databaseFactory.CreateConnection())
            {
                ViewTrainingMid? training = null;
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `avg_speed`, `total_ascent`, `calories`, `is_gps`, `local_timestamp`, `description`, S.`status_caption` as status_caption FROM `training` AS T" +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id" +
                    @" INNER JOIN type_training AS TT ON TT.type_training_id = T.type_training_id" +
                    @" INNER JOIN sport AS SP ON SP.sport_id = T.sport_id" +
                    @" WHERE `training_id` = @training_id AND `sportsman_id` = @user_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = id_sportsman;
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
                            status_training = reader.GetString("status_caption"),
                            description = reader.IsDBNull("description") ? null : reader.GetString("description")
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

        private async Task<List<TrackpointView>> GetLowTrackpoint(ulong training_id, MySqlConnection? connection)
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

        public async Task<(ViewTrainingFull?, List<TrackpointGraph>)> GetFullTraining(ulong training_id, ulong id_user)
        {

            using (var connection = _databaseFactory.CreateConnection())
            {
                ViewTrainingFull? training = null;
                List<TrackpointGraph> result = new();
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `avg_speed`, `total_ascent`, `calories`, `is_gps`, `local_timestamp`, " +
                    @"`avg_heart_rate`, `max_heart_rate`, `min_heart_rate`, `max_speed`, `avg_running_cadence`," +
                    @"`max_running_cadence`, `avg_power`, `max_power`, `start_position_lat`, `start_position_long`," +
                    @"`total_descent`, `max_altitude`, `stress_score`, `feeling_score`, `comment_sportsman`," +
                    @"`coach_creator`, `comment_coach`, `description`, S.`status_caption` as status_caption FROM `training` AS T" +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id" +
                    @" INNER JOIN type_training AS TT ON TT.type_training_id = T.type_training_id" +
                    @" INNER JOIN sport AS SP ON SP.sport_id = T.sport_id" +
                    @" WHERE `training_id` = @training_id AND `sportsman_id` = @user_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("training_id", MySqlDbType.UInt64).Value = training_id;
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = id_user;
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
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
                            status_training = reader.IsDBNull("status_caption") ? null : reader.GetString("status_caption"),
                            description = reader.IsDBNull("description") ? null : reader.GetString("description")
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


        public async Task<ViewTrainingFull?> GetTrainingCreateWithoutFiles(ulong training_id)
        {

            using (var connection = _databaseFactory.CreateConnection())
            {
                ViewTrainingFull? training = null;
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `avg_speed`, `calories`, `is_gps`, `local_timestamp`, " +
                    @"`avg_heart_rate`, `stress_score`, `feeling_score`, `comment_sportsman`," +
                    @"`coach_creator`, `comment_coach`, `description`, S.`status_caption` as status_caption FROM `training` AS T" +
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
                            calories = reader.IsDBNull("calories") ? null : reader.GetUInt16("calories"),
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            is_gps = reader.GetBoolean("is_gps"),
                            avg_heart_rate = reader.IsDBNull("avg_heart_rate") ? null : reader.GetByte("avg_heart_rate"),
                            stress_score = reader.IsDBNull("stress_score") ? null : reader.GetByte("stress_score"),
                            feeling_score = reader.IsDBNull("feeling_score") ? null : reader.GetByte("feeling_score"),
                            comment_sportsman = reader.IsDBNull("comment_sportsman") ? null : reader.GetString("comment_sportsman"),
                            coach_creator = reader.IsDBNull("coach_creator") ? null : reader.GetUInt64("coach_creator"),
                            comment_coach = reader.IsDBNull("comment_coach") ? null : reader.GetString("comment_coach"),
                            status_training = reader.IsDBNull("status_caption") ? null : reader.GetString("status_caption"),
                            description = reader.IsDBNull("description") ? null : reader.GetString("description")
                        };
                    }
                    await reader.CloseAsync();
                }
                await connection.CloseAsync();
                return training;
            }
        }

        public async Task<ViewTrainingFull?> GetTrainingCreatePlanned(ulong training_id)
        {

            using (var connection = _databaseFactory.CreateConnection())
            {
                ViewTrainingFull? training = null;
                await connection.OpenAsync();
                string query = @"SELECT `training_id`, `sportsman_id`, SP.`sport_name` AS sport, TT.`type_training_name` AS type_training, `name_training`, `time_start`, " +
                    @"`duration`, `distance`, `local_timestamp`, " +
                    @"`coach_creator`, `description`, S.`status_caption` as status_caption FROM `training` AS T" +
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
                            local_timestamp = reader.GetDateTime("local_timestamp"),
                            is_gps = false,
                            coach_creator = reader.IsDBNull("coach_creator") ? null : reader.GetUInt64("coach_creator"),
                            status_training = reader.IsDBNull("status_caption") ? null : reader.GetString("status_caption"),
                            description = reader.IsDBNull("description") ? null : reader.GetString("description")
                        };
                    }
                    await reader.CloseAsync();
                }
                await connection.CloseAsync();
                return training;
            }
        }

        private async Task<List<TrackpointGraph>> GetFullTrackpoint(ulong training_id, MySqlConnection? connection)
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
                        power = reader.IsDBNull("power") ? null : reader.GetUInt16("power"),
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

        private async Task<List<TrainingLap>> GetLapsTraining(ulong training_id, MySqlConnection? connection)
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
                        lap_caption = reader.IsDBNull("lap_caption") ? null : reader.GetString("lap_caption"),
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

        public async Task<CalendarViewer> GetPeriodTrainings(ulong sportsman_id, DateTime start, DateTime end, DateTime start_report, DateTime end_report)
        {

            end = end.AddDays(1);
            using (var connection = _databaseFactory.CreateConnection())
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
                    await reader.CloseAsync();
                }
                ReportCalendar report = await GetReportCalendar(sportsman_id, start_report, end_report, connection);
                await connection.CloseAsync();
                CalendarViewer viewer = new CalendarViewer
                {
                    trainings = trainings,
                    report = report
                };
                return viewer;
            }
        }

        private async Task<ReportCalendar> GetReportCalendar(ulong id_user, DateTime start, DateTime end, MySqlConnection? connection)
        {
            ReportCalendar report = new ReportCalendar();
            end = end.AddDays(1);
            string query = @"SELECT COUNT(*) AS trainings, SUM(duration) AS total_duration, SUM(training.distance) AS total_distance, SUM(calories) AS total_calories FROM training " +
                @"WHERE sportsman_id = @sportsman_id AND time_start >= @start AND time_start < @end AND status_training_id = 1";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@sportsman_id", MySqlDbType.UInt64).Value = id_user;
                command.Parameters.Add("@start", MySqlDbType.DateTime).Value = start;
                command.Parameters.Add("@end", MySqlDbType.DateTime).Value = end;
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    report = new ReportCalendar
                    {
                        total_trainings = reader.IsDBNull("trainings") ? 0 : reader.GetInt32("trainings"),
                        total_calories = reader.IsDBNull("total_calories") ? 0 : reader.GetInt32("total_calories"),
                        total_distance = reader.IsDBNull("total_distance") ? 0 : reader.GetDouble("total_distance"),
                        total_duration = reader.IsDBNull("total_duration") ? 0 : reader.GetDouble("total_duration")
                    };
                }
                await reader.CloseAsync();
            }
            return report;
        }

        public async Task<ulong> createPlannedTraining(TrainingCreate training)
        {
            ulong newId = 0;
            using (var connection = _databaseFactory.CreateConnection())
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
        public async Task<CalendarViewer> GetPeriodPlannedTrainings(ulong sportsman_id, DateTime start, DateTime end, DateTime start_report, DateTime end_report)
        {

            end = end.AddDays(1);
            using (var connection = _databaseFactory.CreateConnection())
            {
                List<LowTraining> trainings = new List<LowTraining>();
                await connection.OpenAsync();
                string query = @"SELECT training_id, name_training, sport_id, duration, distance, local_timestamp, S.status_caption as status_caption FROM training AS T " +
                    @" INNER JOIN status_training AS S ON S.status_id = T.status_training_id " +
                    @"WHERE sportsman_id = @sportsman_id AND time_start >= @start AND time_start < @end AND T.status_training_id = 2";
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
                    await reader.CloseAsync();
                }
                ReportCalendar report = await GetReportCalendar(sportsman_id, start_report, end_report, connection);
                await connection.CloseAsync();
                CalendarViewer viewer = new CalendarViewer
                {
                    trainings = trainings,
                    report = report
                };
                return viewer;
            }
        }
    }
}
