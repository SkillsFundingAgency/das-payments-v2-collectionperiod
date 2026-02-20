namespace SFA.DAS.Payments.CollectionPeriod.Application.Models
{
    public class SLDJobContextCollectionPeriodModel
    {
        public byte PeriodNumber { get; set; }
        public short CollectionYear { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public bool IsOpen { get; set; }
    }

}
