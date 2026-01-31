using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence;

public class OutpostImmobileDbContext : IdentityDbContext<UserInternal, IdentityRole<Guid>, Guid>
{
    public bool IsInTestEnv { get; set; } = false;
    
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
    public virtual DbSet<AreaEntity> Areas => Set<AreaEntity>();
    public virtual DbSet<MaczkopatEntity> Maczkopats => Set<MaczkopatEntity>();
    public virtual DbSet<NumberTemplateEntity> NumberTemplates => Set<NumberTemplateEntity>();
    public virtual DbSet<ParcelEntity> Parcels => Set<ParcelEntity>();
    public virtual DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public virtual DbSet<UserInternal> UsersInternal => Set<UserInternal>();
    public virtual DbSet<UserExternal> UsersExternal => Set<UserExternal>();
    public virtual DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutpostImmobileDbContext).Assembly);

        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("pgrouting");
        modelBuilder.HasPostgresExtension("hstore");
        
        modelBuilder.Ignore(typeof(IdentityPasskeyData));

        if (IsInTestEnv)
        {
            modelBuilder.Entity<RouteEntity>().Ignore(x => x.Locations);
            modelBuilder.Entity<AddressEntity>().Ignore(x => x.Location);
        }
    }
}