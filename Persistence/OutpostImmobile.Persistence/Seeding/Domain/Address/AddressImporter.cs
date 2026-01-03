using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace OutpostImmobile.Persistence.Seeding.Domain.Address;

internal class AddressImporter
{
    public static IEnumerable<AddressModel> Import()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        
        using var reader = new StreamReader("../../Adresy.csv");
        using var csv = new CsvReader(reader, config);
        
        return csv.GetRecords<AddressModel>();
    }
}