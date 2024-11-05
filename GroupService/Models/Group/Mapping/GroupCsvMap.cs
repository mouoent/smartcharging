using CsvHelper.Configuration;
using GroupService.Models;
using GroupService.Models.Group;

public class GroupCsvMap : ClassMap<Group>
{
    public GroupCsvMap()
    {
        Map(m => m.Id);
        Map(m => m.Name);
        Map(m => m.Capacity);
        Map(m => m.CurrentLoad);
        Map(m => m.ChargeStationIds)
            .Convert(row => row.Row.GetField("ChargeStationIds")?
                .Split(',')
                .Select(id => Guid.TryParse(id.Trim(), out var guid) ? guid : Guid.Empty)
                .Where(guid => guid != Guid.Empty)
                .ToList());

    }
}
