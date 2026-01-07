
using GameStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreApi.Data;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> options) : base(options)
    {
        
    }

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Game> Games => Set<Game>();
}
