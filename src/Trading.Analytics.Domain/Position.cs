namespace Trading.Analytics.Domain
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Figi { get; set; }
        public string Currency { get; set; }
    }
}