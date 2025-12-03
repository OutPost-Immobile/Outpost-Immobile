using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums;

namespace OutpostImmobile.Persistence;

public class OutpostImmobileDbContext : DbContext
{
    public OutpostImmobileDbContext()
    {
    }

    public OutpostImmobileDbContext(DbContextOptions<OutpostImmobileDbContext> options) : base(options)
    {
    }

    public virtual DbSet<CommunicationEventLogEntity> CommunicationEventLogs => Set<CommunicationEventLogEntity>();
    public virtual DbSet<ParcelEventLogEntity> ParcelEventLogs => Set<ParcelEventLogEntity>();
    public virtual DbSet<MaczkopatEventLogEntity> MaczkopatEventLogs => Set<MaczkopatEventLogEntity>();
    public virtual DbSet<StaticEnumEntity> StaticEnums => Set<StaticEnumEntity>();
    public virtual DbSet<StaticEnumTranslationEntity> StaticEnumTranslations => Set<StaticEnumTranslationEntity>();
    public virtual DbSet<AddressEntity> Addresses => Set<AddressEntity>();
    public virtual DbSet<MaczkopatEntity> Maczkopats => Set<MaczkopatEntity>();
    public virtual DbSet<ParcelEntity> Parcels => Set<ParcelEntity>();
    public virtual DbSet<RouteEntity> Routes => Set<RouteEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutpostImmobileDbContext).Assembly);

        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("pgrouting");
        modelBuilder.HasPostgresExtension("hstore");
    }
}