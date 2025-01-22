using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Infrastructure.InternalCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.UserAcess.Domain.Users;
using Modules.UserAcess.Infrastructure.Domain.Users;
using Modules.UserAcess.Infrastructure.InternalCommands;
using Modules.UserAcess.Infrastructure.Outbox;

namespace Modules.UserAcess.Infrastructure;

public class UserAccessContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<InternalCommand> InternalCommands { get; set; }

    private readonly ILoggerFactory _loggerFactory;

    public UserAccessContext(DbContextOptions options, ILoggerFactory loggerFactory)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InternalCommandEntityTypeConfiguration());
    }
}