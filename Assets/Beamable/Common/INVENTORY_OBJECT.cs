#nullable enable
using System.Collections.Generic;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Inventory;

public class INVENTORY_OBJECT<TCONTENT> where TCONTENT : ItemContent
{
    public INVENTORY_OBJECT(TCONTENT content, ItemView item_view)
    {
        ItemContent = content;
        Properties = new Dictionary<string, string>(item_view.properties);
        original_properties = new Dictionary<string, string>(item_view.properties);
        Id = item_view.id;
        CreatedAt = item_view.createdAt;
        UpdatedAt = item_view.updatedAt;
    }

    public INVENTORY_OBJECT(TCONTENT content)
    {
        ItemContent = content;
        Properties = new Dictionary<string, string>();
        original_properties = new Dictionary<string, string>();
        Id = 0;
        CreatedAt = 0;
        UpdatedAt = 0;
    }

    public bool is_in_inventory => CreatedAt != 0;

    public bool properties_modified()
    {
        if (Properties.Count != original_properties.Count)
        {
            return true;
        }

        foreach (var property in original_properties)
        {
            if (!Properties.TryGetValue(property.Key, out var value) || property.Value != value)
            {
                return true;
            }
        }

        return false;
    }

    public TCONTENT ItemContent;
    public Dictionary<string, string> Properties;
    public long Id;
    public long CreatedAt;
    public long UpdatedAt;

    private readonly Dictionary<string, string> original_properties;
}