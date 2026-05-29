using Trey2.DTO.Coach;
using Trey2.Models;
using Trey2.ViewModel;
using Trey2.ViewModel.Report;

namespace Trey2.Services.Interfaces
{
    public interface ICoachService
    {
        Task<List<CoachElement?>> getCoaches(ulong id);
        Task<List<SportsmanElement>> getSportsmen(ulong id);
        Task<bool> CheckRelation(ulong sportsman_id, ulong coach_id);
        Task<bool> CheckPendingInvite(ulong sportsman_id, ulong coach_id);
        Task CreateInvite(ulong id_sportsman, ulong id_coach);
        Task AcceptedInvite(ulong id_invite);
        Task RejectedInvite(ulong id_invite);
        Task CanceledInvite(ulong id_invite);
        Task<bool> RemoveRelation(ulong id_relation, ulong id_user);
        Task<List<Invite>> GetInvites(ulong id_user);

        Task PatchCommentCoach(ulong id_coach, ulong id_training, string comment);
    }
}
