using System;

namespace Trading.Analytics.Shared.Models
{
    public class Candle
    {
        public double O { get; set; }
        public double C { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public int V { get; set; }
        public DateTime Time { get; set; }
        public string Interval { get; set; }
        public string Figi { get; set; }
    }
}
