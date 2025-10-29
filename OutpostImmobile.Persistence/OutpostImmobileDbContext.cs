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
    public virtual DbSet<StaticEnumEntity> StaticEnums => Set<StaticEnumEntity>();
    public virtual DbSet<StaticEnumTranslationEntity> StaticEnumTranslations => Set<StaticEnumTranslationEntity>();
    public virtual DbSet<AddressEntity> Addresses => Set<AddressEntity>();
    public virtual DbSet<AreaEntity> Areas => Set<AreaEntity>();
    public virtual DbSet<MaczkopatEntity> Maczkopats => Set<MaczkopatEntity>();
    public virtual DbSet<NumberTemplateEntity> NumberTemplates => Set<NumberTemplateEntity>();
    public virtual DbSet<ParcelEntity> Parcels => Set<ParcelEntity>();
    public virtual DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public virtual DbSet<LocationMarkerEntity> Locations => Set<LocationMarkerEntity>();
    public virtual DbSet<UserInternal> Users => Set<UserInternal>();
    public virtual DbSet<UserRoles> UserRoles => Set<UserRoles>();
    public virtual DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();
}