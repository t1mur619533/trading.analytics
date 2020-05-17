using System.ComponentModel.DataAnnotations.Schema;

namespace Trading.Analytics.Domain
{
    public class PositionSnapshot
    {
        public int Id { get; set; }
        public int PortfolioSnapshotId { get; set; }
        public PortfolioSnapshot PortfolioSnapshot { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }

        public int Count { get; set; }
        public double Value { get; set; }

        [NotMapped]
        public double Price => Count * Value;
    }
}