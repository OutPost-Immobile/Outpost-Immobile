namespace OutpostImmobile.Core.Infrastructure.QueryResults;

public record EnumTranslationDto
{
    public required int Value { get; init; }
    public required string Label { get; init; }
}
