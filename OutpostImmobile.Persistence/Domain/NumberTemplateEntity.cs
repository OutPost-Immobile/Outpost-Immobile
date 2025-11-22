using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class NumberTemplateEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string TemplateNumber { get; set; }
}