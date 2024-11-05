using CsvHelper.Configuration;

namespace ConnectorService.Models.Connector.Mapping;

public class ConnectorCsvMap : ClassMap<Connector>
{
    public ConnectorCsvMap()
    {
        Map(m => m.Id).Name("Id");
        Map(m => m.ChargeStationId).Name("ChargeStationId");
        Map(m => m.InternalId).Name("InternalId");
        Map(m => m.MaxCurrent).Name("MaxCurrent");
    }
}
