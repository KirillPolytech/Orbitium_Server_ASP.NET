using Microsoft.EntityFrameworkCore;
using BackEndForGame.Entities.DataBaseEntities;

namespace BackEndForGame.Configuration
{
    public class OrbitiumDataBaseContext : DbContext
    {
        public DbSet<Users> Users {get; set; }
        public DbSet<Players> Players {get; set; }
        public DbSet<LeaderBoards> LeaderBoards {get; set; }
        public DbSet<Inventories> Inventories { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<ItemStorage> ItemStorage { get; set; }

        public OrbitiumDataBaseContext(DbContextOptions<OrbitiumDataBaseContext> options)  : base (options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Users>().HasKey(z => z.ID);
            // юзер имеет одного игрок к одному юзеру, и игрок имеет FK user_id.
            builder.Entity<Users>().
                HasOne(x => x.player).
                WithOne(y => y.user).
                HasForeignKey<Players>(x => x.user_id);
            
            builder.Entity<Players>().HasKey(z => z.ID);
            builder.Entity<Players>().
                HasOne(x => x.user).
                WithOne(y => y.player);
            builder.Entity<Players>().
                HasOne(x => x.inventory).
                WithOne(y => y.player).
                HasForeignKey<Inventories>(x => x.player_id);
            builder.Entity<Players>().
                HasOne(x => x.leaderBoard).
                WithOne(y => y.player).
                HasForeignKey<LeaderBoards>(x => x.player_id);

            builder.Entity<LeaderBoards>().HasKey(z => z.ID);
            builder.Entity<LeaderBoards>().
                HasOne(x => x.player).
                WithOne(y => y.leaderBoard);
            
            builder.Entity<Inventories>().HasKey(z => z.ID);            
            builder.Entity<Inventories>().
                HasOne(x => x.player).
                WithOne(y => y.inventory);
            builder.Entity<Inventories>().
                HasMany(x => x.items).
                WithOne(x => x.inventory);

            builder.Entity<Items>().HasKey(z => z.ID);
            builder.Entity<Items>().
                HasOne(x => x.inventory).
                WithMany(y => y.items).HasForeignKey( x => x.inventory_id );

            builder.Entity<ItemStorage>().HasKey(z => z.ID);
        }
    }
}

/*
builder.Entity<Inventories>().
    HasMany(x => x.items).
    WithOne(y => y.inventory).HasForeignKey( x => x.inventory_id);
*/

//builder.Entity<Items>().HasKey(z => z.ID);
/*
builder.Entity<Items>().
    HasOne(x => x.inventory).
    WithMany(y => y.items).HasForeignKey(x => x.inventory_id);
*/