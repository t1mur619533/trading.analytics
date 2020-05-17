using System;
using System.Collections.Generic;

namespace Trading.Analytics.Shared.Models
{
    public class Operations : OpenApiResponse<Operations.Envelope>
    {
        public Operations(string trackingId, string status, Envelope payload) : base(trackingId, status,
            payload)
        {
        }

        public class Commission
        {
            public string Currency { get; set; }
            public double Value { get; set; }
        }

        public class Trade
        {
            public string TradeId { get; set; }
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
        }

        public class Operation
        {
            public string OperationType { get; set; }
            public DateTime Date { get; set; }
            public bool IsMarginCall { get; set; }
            public string InstrumentType { get; set; }
            public string Figi { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double Payment { get; set; }
            public string Currency { get; set; }
            public string Status { get; set; }
            public string Id { get; set; }
            public Commission Commission { get; set; }
            public IList<Trade> Trades { get; set; }
        }

        public class Envelope
        {
            public IList<Operation> Operations { get; set; }
        }
    }
}