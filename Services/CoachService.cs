using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Trey2.DB;
using Trey2.DTO.Coach;
using Trey2.Models;
using Trey2.Services.Interfaces;
using Trey2.ViewModel;
using Trey2.ViewModel.Report;

namespace Trey2.Services
{
    public class CoachService: ICoachService
    {
        private readonly IDatabaseFactory _databaseFactory;

        public CoachService(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public async Task<List<CoachElement?>> getCoaches(ulong id)
        {
            List<CoachElement?> result = new List<CoachElement?>();
            using(var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT `user_id` AS `id`, `user_email` AS `email`, `user_nickname` AS `nickname`, CONCAT(`user_surname`, ' ', `user_name`, ' ', `user_fathername`) AS `fullname`" +
                    @" FROM user WHERE (`user_only_coach` = 1 OR `like_coach` = 1) AND `user_id` != @user_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("user_id", MySqlDbType.UInt64).Value = id;
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        CoachElement element = new CoachElement
                        {
                            id = reader.GetUInt64("id"),
                            email = reader.GetString("email"),
                            nickname = reader.GetString("nickname"),
                            fullname = reader.GetString("fullname")
                        };
                        if (!(element is null))
                        {
                            result.Add(element);
                        }
                    }
                    await reader.CloseAsync();
                }
                await connection.CloseAsync();
            }
            return result;
        }

        public async Task<List<SportsmanElement>> getSportsmen(ulong id)
        {
            List<SportsmanElement> result = new List<SportsmanElement>();
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT U.`user_id` AS `id`, `user_nickname` AS `nickname`, CONCAT(`user_surname`, ' ', `user_name`, ' ', `user_fathername`) AS `fullname`
                                 FROM user U INNER JOIN `sportsman_coach` SC ON U.user_id = SC.sportsman_id WHERE SC.coach_id = @coach_id;";
                using(var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("coach_id", MySqlDbType.UInt64).Value = id;
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        SportsmanElement element = new SportsmanElement
                        {
                            id = reader.GetUInt64("id"),
                            fullname = reader.GetString("fullname")
                        };
                        if (element is not null)
                        {
                            result.Add(element);
                        }
                    }
                    await reader.CloseAsync();
                }
                await connection.CloseAsync();
            }
            return result;
        }
        public async Task<bool> CheckRelation(ulong sportsman_id, ulong coach_id)
        {
            object? result;
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(*) AS `relation` FROM sportsman_coach WHERE sportsman_id = @sportsman AND coach_id = @coach;";
                using(var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("sportsman", MySqlDbType.UInt64).Value = sportsman_id;
                    command.Parameters.Add("coach", MySqlDbType.UInt64).Value = coach_id;
                    result = await command.ExecuteScalarAsync();
                    
                }
                await connection.CloseAsync();
            }
            return (Convert.ToInt32(result) == 0) || (result is null) ? false : true;
        }
        public async Task<bool> CheckPendingInvite(ulong sportsman_id, ulong coach_id)
        {
            object? result;
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(*) AS `count` FROM invitation WHERE sportsman_id = @sportsman AND coach_id = @coach AND `status` = 0;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("sportsman", MySqlDbType.UInt64).Value = sportsman_id;
                    command.Parameters.Add("coach", MySqlDbType.UInt64).Value = coach_id;
                    result = await command.ExecuteScalarAsync();

                }
                await connection.CloseAsync();
            }
            return (Convert.ToInt32(result) == 0) || (result is null) ? false : true;
        }
        public async Task CreateInvite(ulong id_sportsman, ulong id_coach)
        {
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO invitation (sportsman_id, coach_id, `status`) VALUES (@sportsman, @coach, 0);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("sportsman", MySqlDbType.UInt64).Value = id_sportsman;
                    command.Parameters.Add("coach", MySqlDbType.UInt64).Value = id_coach;
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }
        public async Task AcceptedInvite(ulong id_invite)
        {
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    string query = @"UPDATE invitation SET `status` = 1, responded_at = NOW() WHERE id = @id_invite AND `status`= 0";
                    using (var command = new MySqlCommand(query, connection, transaction))
                    {
                        command.Parameters.Add("id_invite", MySqlDbType.UInt64).Value = id_invite;
                        await command.ExecuteNonQueryAsync();
                    }
                    string query_2 = @"INSERT INTO sportsman_coach (sportsman_id, coach_id) SELECT sportsman_id, coach_id FROM invitation WHERE id = @id_invite;";
                    using (var command = new MySqlCommand(query_2, connection, transaction))
                    {
                        command.Parameters.Add("id_invite", MySqlDbType.UInt64).Value = id_invite;
                        await command.ExecuteNonQueryAsync();
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
        public async Task RejectedInvite(ulong id_invite)
        {
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"UPDATE invitation SET `status` = 2, responded_at = NOW() WHERE id = @id_invite AND `status`= 0";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id_invite", MySqlDbType.UInt64).Value = id_invite;
                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }
        public async Task CanceledInvite(ulong id_invite)
        {
            using (var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"UPDATE invitation SET `status` = 3, responded_at = NOW() WHERE id = @id_invite AND `status`= 0";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("id_invite", MySqlDbType.UInt64).Value = id_invite;
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
            }
        }
        public async Task<bool> RemoveRelation (ulong id_relation)
        {
            object? result;
            using(var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = "DELETE FROM sportsman_coach WHERE sportsman_coach_id = @sportsman_coach_id;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("sportsman_coach_id", MySqlDbType.UInt64).Value = id_relation;
                    result = await command.ExecuteScalarAsync();
                }
            }
            return (Convert.ToInt32(result) == 0) || (result is null) ? false : true;
        }

        public async Task<List<Invitation>> GetInvites(ulong id_user)
        {
            List<Invitation> result = new List<Invitation>();
            using(var connection = _databaseFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string query = @"SELECT `id`, `sportsman_id`, `coach_id`, `status`, `created_at`, `responded_at` FROM `invitation` WHERE `coach_id` = @id_user OR `sportsman_id`=@id_user;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add("@id_user", MySqlDbType.UInt64).Value = id_user;
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        Invitation invite = new Invitation
                        {
                            id = reader.GetUInt64("id"),
                            sportsman_id = reader.GetUInt64("sportsman_id"),
                            coach_id = reader.GetUInt64("coach_id"),
                            status = reader.GetByte("status"),
                            created_at = reader.GetDateTime("created_at"),
                            responded_at = reader.IsDBNull("responded_at") ? null : reader.GetDateTime("responded_at")
                        };
                        if (invite is not null)
                        {
                            result.Add(invite);
                        }
                    }
                    await reader.CloseAsync();
                }
                await connection.CloseAsync();
            }
            return result;
        }
    }
}
