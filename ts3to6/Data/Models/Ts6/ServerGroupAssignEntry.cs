using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class ServerGroupAssignEntry
{
    public int EntryId { get; set; }

    public int ServerId { get; set; }

    public int InvokerClientDbId { get; set; }

    public int ModifyPower { get; set; }

    public int EntryType { get; set; }

    public int VsGroupId { get; set; }

    public int? Ttl { get; set; }

    public string? MigrationValue { get; set; }

    public string? ClientUid { get; set; }

    public string? Mytsid { get; set; }

    public int CreatedTimestamp { get; set; }
}
