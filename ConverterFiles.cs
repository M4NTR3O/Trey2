using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Trey2.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Globalization;
using Trey2.Models;
using System.Drawing;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Trey2
{
    public interface ConverterFiles
    {
        public static void covert(string filepath)
        {

            /* Runtime.PythonDLL = @"D:\Users\emil_\PycharmProjects\DecoderTrainings\.venv\Scripts\python313.dll";

             string venvPath = @"D:\Users\emil_\PycharmProjects\DecoderTrainings\.venv";
             Environment.SetEnvironmentVariable("PYTHONHOME", venvPath);
             Environment.SetEnvironmentVariable("PYTHONPATH", Path.Combine(venvPath, "Lib", "site-packages"));

             PythonEngine.Initialize();

             using (Py.GIL())  // GIL нужен для многопоточности
             {
                 dynamic sys = Py.Import("sys");
                 sys.path.append("D:\\Users\\emil_\\PycharmProjects\\DecoderTrainings");

                 dynamic myModule = Py.Import("FitConverter"); // my_module.py
                 myModule.printFitFiles(@"C:\Users\emil_\Downloads\Telegram Desktop\21426241335\21399806193_ACTIVITY.fit");
                 dynamic result = myModule.decodeFitFiles(@"C:\Users\emil_\Downloads\Telegram Desktop\21426241335\21399806193_ACTIVITY.fit");
                 Training training = new Training();
                 training.id = Guid.Parse(result.id.ToString());
                 training.sportsmen_id = (int?)result.sportsmen_id;
                 training.sport_id = (string?)result.sport_id;
                 training.type_training_id = (int?)result.type_training_id;
                 training.name = (string?)result.name;
                 training.duration = (double?)result.duration;
                 training.distance = (double?)result.distance;
                 training.time_start = DateTime.Parse(result.time_start.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                 training.local_timestamp = DateTime.Parse(result.local_timestamp.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                 training.avg_heart_rate = (int?)result.avg_heart_rate;
                 training.max_heart_rate = (int?)result.max_heart_rate;
                 training.min_heart_rate = (int?)result.min_heart_rate;
                 training.avg_speed = (double?)result.avg_speed;
                 training.max_speed = (double?)result.max_speed;
                 training.total_ascent = (int?)result.total_ascent;
                 training.total_descent = (int?)result.total_descent;
                 training.calories = (int?)result.calories;
                 training.avg_running_cadence = (int?)result.avg_running_cadence;
                 training.max_running_cadence = (int?)result.max_running_cadence;
                 training.avg_power = (int?)result.avg_power;
                 training.max_power = (int?)result.max_power;
                 training.start_position_long = (long?)result.start_position_long;
                 training.start_position_lat = (long?)result.start_position_lat;
                 training.is_gps = (bool)result.is_gps;
                 var trainingLaps = new List<TrainingLap>();
                 dynamic laps = result.laps;
                 int count = laps.__len__();  // Python len()
                 for (int i = 0; i < count; i++)
                 {
                     dynamic lap = laps[i];
                     trainingLaps.Add(new TrainingLap
                     {
                         lap_number = (int?)lap.lap_number,
                         duration = (double?)lap.duration,
                         name = lap.name.ToString(),
                         distance = (double?)lap.distance,
                         avg_heart_rate = (int?)lap.avg_heart_rate,
                         max_heart_rate = (int?)lap.max_heart_rate,
                         min_heart_rate = (int?)lap.min_heart_rate,
                         avg_speed = (double?)lap.avg_speed,
                         max_speed = (double?)lap.max_speed,
                         total_ascent = (int?)lap.total_ascent,
                         total_descent = (int?)lap.total_descent,
                         avg_running_cadence = (int?)lap.avg_running_cadence,
                         max_running_cadence = (int?)lap.max_running_cadence,
                         avg_power = (int?)lap.avg_power,
                         max_power = (int?)lap.max_power,
                         timestamp = DateTime.Parse(lap.timestamp.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                         start_time = DateTime.Parse(lap.start_time.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
                     });
                 }
                 training.laps = trainingLaps;
                 //training.Laps = ((List<dynamic>)result.laps).Select(l => new TrainingLap
                 //{
                 //    LapNumber = (int)l.lap_number,
                 //    Duration = (double)l.duration,
                 //    Name = l.name.ToString(),
                 //    Distance = (double)l.distance
                 //}).ToList();
                 var training_gps_hr_points = new List<Training_GPS_HR>();
                 dynamic trainingGpsHrPoints = result.training_gps_hr_points;
                 if (trainingGpsHrPoints != null)
                 {
                     count = trainingGpsHrPoints.__len__();  // Python len()
                     for (int i = 0; i < count; i++)
                     {
                         dynamic point = trainingGpsHrPoints[i];
                         training_gps_hr_points.Add(new Training_GPS_HR
                         {
                             duration_point = DateTime.Parse(point.duration_point.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                             heart_rate = (int?)point.heart_rate,
                             latitude = (long?)point.latitude,
                             longitude = (long?)point.longitude,
                             altitude = (int?)point.altitude,
                             speed = (double?)point.speed,
                             distance = (double?)point.distance,
                             power = (int?)point.power,
                             cadence = (int?)point.cadence
                         });
                     }
                     training.training_gps_hr_points = training_gps_hr_points;
                 }
                 //training.TrainingGPSHRPoints = ((List<dynamic>)result.training_gps_hr_points).Select(point => new TrainingGPSHR
                 //{
                 //    DurationPoint = DateTime.Parse(point.duration_point.ToString()),
                 //    HR = point.hr,
                 //    Latitude = point.latitude,
                 //    Longitude = point.longitude,
                 //    Altitude = point.altitude,
                 //    Speed = point.speed,
                 //    Distance = (double)point.distance
                 //}).ToList();

                 var trainingHR = new List<TrainingHR>();
                 dynamic trainingHRPoints = result.training_hr_points;
                 if (trainingHRPoints != null)
                 {
                     count = trainingHRPoints.__len__();  // Python len()
                     for (int i = 0; i < count; i++)
                     {
                         dynamic pointHR = trainingHRPoints[i];
                         trainingHR.Add(new TrainingHR
                         {
                             duration_point = DateTime.Parse(pointHR.duration_point.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                             heart_rate = pointHR.heart_rate
                         });
                     }
                     training.training_hr_points = trainingHR;
                 }
                 //training.TrainingHRPoints = ((List<dynamic>)result.training_hr_points).Select(point => new TrainingHR
                 //{
                 //    DurationPoint = DateTime.Parse(point.duration_point.ToString()),
                 //    HR = point.hr
                 //}).ToList();
                 var options = new JsonSerializerOptions
                 {
                     WriteIndented = true
                 };

                 string json = JsonSerializer.Serialize<Training>(training, options);
                 File.WriteAllText(@"C:\Users\emil_\Downloads\sample6.txt", json);
                 Console.WriteLine(json); 
             }*/
        }
    }
}
