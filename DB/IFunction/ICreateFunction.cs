using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace Trey2.DB.IFunction
{
    public interface ICreateFunction
    {
        public static async Task createUser(Models.User _user, IDatabaseFactory factory)
        {
            var database = factory.Create();
            long newId;
            using (var connection = database.CreateNewConnection())
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO user 
                            (user_surname, user_name, user_fathername, user_email, user_nickname, password, user_birthday, user_gender, user_height, user_weight, user_only_coach, user_hr_rest, user_hr_max, user_pace_max) 
                            VALUES 
                            (@Surname, @Name, @Fathername, @Email, @Nickname, @Password, @Birthday, @Gender, @Height, @Weight, @Only_Coach, @Hr_Rest, @Hr_Max, @Pace_Max);";
                using (var command = new MySqlCommand(query, connection))
                {
                    //Подправить параметры на нулл, перевод пароля в хэш и тд. и тп.
                    command.Parameters.AddWithValue("@Surname", _user.user_surname);
                    command.Parameters.AddWithValue("@Name", _user.user_name);
                    command.Parameters.AddWithValue("@Fathername", (object?)_user.user_fathername ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", _user.user_email);
                    command.Parameters.AddWithValue("@Nickname", _user.user_nickname);
                    command.Parameters.AddWithValue("@Password", PasswordHasher.Hash(_user.password));
                    command.Parameters.Add("@Birthday", MySqlDbType.Date).Value = _user.user_birthday;
                    command.Parameters.AddWithValue("@Gender", _user.user_gender);
                    command.Parameters.AddWithValue("@Height", _user.user_height);
                    command.Parameters.AddWithValue("@Weight", _user.user_weight);
                    command.Parameters.AddWithValue("@Only_Coach", _user.user_only_coach);
                    command.Parameters.AddWithValue("@Hr_Rest", _user.user_hr_rest);
                    command.Parameters.AddWithValue("@Hr_Max", _user.user_hr_max);
                    command.Parameters.AddWithValue("@Pace_Max", (object?)_user.user_pace_max ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                    newId = command.LastInsertedId;
                }
                await connection.CloseAsync();
            }
        }
    }
}
