namespace Thingsboard;

#pragma warning disable IDE1006 // Naming Styles
public class TBDeviceEntry
{
    public Id? id { get; set; }
    public long createdTime { get; set; }
    public object? additionalInfo { get; set; }
    public Tenantid? tenantId { get; set; }
    public Customerid? customerId { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
    public object? label { get; set; }
    public Deviceprofileid? deviceProfileId { get; set; }
    public Devicedata? deviceData { get; set; }
    public object? firmwareId { get; set; }
    public object? softwareId { get; set; }
}

public class Id
{
    public string? entityType { get; set; }
    public string? id { get; set; }
}

public class Tenantid
{
    public string? entityType { get; set; }
    public string? id { get; set; }
}

public class Customerid
{
    public string? entityType { get; set; }
    public string? id { get; set; }
}

public class Deviceprofileid
{
    public string? entityType { get; set; }
    public string? id { get; set; }
}

public class Devicedata
{
    public Configuration? configuration { get; set; }
    public Transportconfiguration? transportConfiguration { get; set; }
}

public class Configuration
{
    public string? type { get; set; }
}

public class Transportconfiguration
{
    public string? type { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles

