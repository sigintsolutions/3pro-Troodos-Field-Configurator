namespace Thingsboard.DeviceController
{
#pragma warning disable IDE1006 // Naming Styles

    public class GetTenantDevicesResponse
    {
        public Datum[]? data { get; set; }
        public int totalPages { get; set; }
        public int totalElements { get; set; }
        public bool hasNext { get; set; }

        public class Datum
        {
            public Id? id { get; set; }
            public long createdTime { get; set; }
            public Tenantid? tenantId { get; set; }
            public Customerid? customerId { get; set; }
            public string? name { get; set; }
            public string? type { get; set; }
            public string? label { get; set; }
            public Deviceprofileid? deviceProfileId { get; set; }
            public Devicedata? deviceData { get; set; }
            public Firmwareid? firmwareId { get; set; }
            public Softwareid? softwareId { get; set; }
            public Additionalinfo? additionalInfo { get; set; }
            public string? customerTitle { get; set; }
            public bool customerIsPublic { get; set; }
            public string? deviceProfileName { get; set; }
        }

        public class Id
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Tenantid
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Customerid
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Deviceprofileid
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Devicedata
        {
            public Configuration? configuration { get; set; }
            public Transportconfiguration? transportConfiguration { get; set; }
        }

        public class Configuration
        {
        }

        public class Transportconfiguration
        {
        }

        public class Firmwareid
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Softwareid
        {
            public string? id { get; set; }
            public string? entityType { get; set; }
        }

        public class Additionalinfo
        {
        }
    }

#pragma warning restore IDE1006 // Naming Styles
}
