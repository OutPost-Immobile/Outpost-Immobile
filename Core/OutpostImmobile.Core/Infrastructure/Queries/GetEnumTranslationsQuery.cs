using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Infrastructure.QueryResults;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Core.Infrastructure.Queries;

public record GetEnumTranslationsQuery : IRequest<GetEnumTranslationsQuery, Task<List<EnumTranslationDto>>>
{
    public required string EnumName { get; init; }
    public TranslationLanguage Language { get; init; } = TranslationLanguage.Pl;
}

internal class GetEnumTranslationsQueryHandler : IRequestHandler<GetEnumTranslationsQuery, Task<List<EnumTranslationDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetEnumTranslationsQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<EnumTranslationDto>> Handle(GetEnumTranslationsQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        return await context.StaticEnumTranslations
            .Where(x => x.EnumName == request.EnumName && x.TranslationLanguage == request.Language)
            .OrderBy(x => x.EnumValue)
            .Select(x => new EnumTranslationDto
            {
                Value = x.EnumValue,
                Label = x.Translation
            })
            .ToListAsync(ct);
    }
}
