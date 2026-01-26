using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Trey2.Models;

namespace Trey2.DB.IFunction
{
    public class IReadFunction
    {
        public static async Task<User?> getUser(ulong id, IDatabaseFactory factory)
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
                            user_fathername = reader[3].ToString()! != null ? reader[3].ToString() : null,
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

        public static async Task<(ulong, bool)> getAuthEmail(AuthRequest request, IDatabaseFactory factory)
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
    }
}
