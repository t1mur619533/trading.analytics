using System.Collections.Generic;

namespace Trading.Analytics.Shared.Models
{
    public class Portfolio : OpenApiResponse<Portfolio.Envelope>
    {
        public Portfolio(string trackingId, string status, Portfolio.Envelope payload) : base(trackingId, status,
            payload)
        {
        }

        public class ExpectedYield
        {
            public string Currency { get; set; }
            public double Value { get; set; }
        }

        public class AveragePositionPrice
        {
            public string Currency { get; set; }
            public double Value { get; set; }
        }

        public class Position
        {
            public string Figi { get; set; }
            public string Ticker { get; set; }
            public string Isin { get; set; }
            public string InstrumentType { get; set; }
            public double Balance { get; set; }
            public int Lots { get; set; }
            public ExpectedYield ExpectedYield { get; set; }
            public AveragePositionPrice AveragePositionPrice { get; set; }
            public string Name { get; set; }
        }

        public class Envelope
        {
            public IList<Position> Positions { get; set; }
        }
    }
}