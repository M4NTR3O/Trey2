namespace Trey2.ViewModel.Report
{
    public class ReportPage
    {
        public ReportOverview overview { get; set; }
        public List<ReportBin>? reportBins { get; set; }
        public List<ReportPie>? typePie { get; set; }
        public List<ReportPie>? sportSessionPie { get; set; }
        public List<ReportPie>? sportDistancePie { get; set; }
        public List<ReportPie>? sportDurationPie { get; set; }
    }
}
