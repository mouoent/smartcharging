using CsvHelper.Configuration;

namespace ChargeStationService.Models.ChargeStation.Mapping;

public class ChargeStationCsvMap : ClassMap<ChargeStation>
{
    public ChargeStationCsvMap()
    {
        Map(m => m.Id).Name("Id");
        Map(m => m.Name).Name("Name");
        Map(m => m.InternalConnectorIds)
            .Convert(row => row.Row.GetField("InternalConnectorIds")?
            .Split(',')
            .Select(id => int.Parse(id.Trim()))
            .ToList());
        Map(m => m.GroupId).Name("GroupId");
    }
}
