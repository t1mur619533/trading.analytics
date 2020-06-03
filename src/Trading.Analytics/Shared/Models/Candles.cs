using System.Collections.Generic;

namespace Trading.Analytics.Shared.Models
{
    public class Candles : OpenApiResponse<Candles.Envelope>
    {
        public Candles(string trackingId, string status, Envelope payload) : base(trackingId, status,
            payload)
        {
        }

        public class Envelope
        {
            public IList<Candle> Candles { get; set; }
            public string Interval { get; set; }
            public string Figi { get; set; }
        }
    }
}