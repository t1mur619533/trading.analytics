using System;
using System.Collections.Generic;

namespace Trading.Analytics.Domain
{
    public class PortfolioSnapshot
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public double RubPerUsd { get; set; }
        public double TotalPriceRub { get; set; }
        public double PriceRub { get; set; }
        public double PriceUsd { get; set; }
        public List<PositionSnapshot> Positions { get; set; }
    }
}