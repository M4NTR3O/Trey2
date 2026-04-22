namespace Trey2.ViewModel.Report
{
    public class ReportOverview
    {
        public uint total_trainings { get; set; }
        public double total_duration { get; set; }
        public double? total_distance { get; set; }
        public int? total_calories { get; set; }
        public double? avg_pace { get; set; }
        public int? total_ascent { get; set; }
        public int? total_descent { get; set; }
    }
}
