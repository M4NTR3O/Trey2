using Trey2.ViewModel;
using Trey2.ViewModel.Report;
using Trey2.ViewModels;

namespace Trey2.Services.Interfaces
{
    public interface ITrainingService
    {
        Task<ReportPage?> GetReportPage(ulong id_sportsman, DateTime start, DateTime end, string bin);
        Task<ViewTrainingMid?> GetMidTraining(ulong training_id, ulong id_sportsman);
        Task<(ViewTrainingFull?, List<TrackpointGraph>)> GetFullTraining(ulong training_id, ulong id_user);
        Task<ViewTrainingFull?> GetTrainingCreateWithoutFiles(ulong training_id);
        Task<ViewTrainingFull?> GetTrainingCreatePlanned(ulong training_id);
        Task<CalendarViewer> GetPeriodTrainings(ulong sportsman_id, DateTime start, DateTime end, DateTime start_report, DateTime end_report);
        Task<ulong> createPlannedTraining(TrainingCreate training);
        Task<CalendarViewer> GetPeriodPlannedTrainings(ulong sportsman_id, DateTime start, DateTime end, DateTime start_report, DateTime end_report);
    }
}
