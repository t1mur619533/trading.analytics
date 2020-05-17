using Microsoft.EntityFrameworkCore;

namespace Trading.Analytics.Domain
{
    public sealed class TradingContext : DbContext
    {
        public DbSet<Position> Positions { get; set; }

        public DbSet<PositionSnapshot> PositionSnapshots { get; set; }

        public DbSet<PortfolioSnapshot> PortfolioSnapshots { get; set; }

        public TradingContext(DbContextOptions<TradingContext> options)
            : base(options)
        {
        }
    }
}