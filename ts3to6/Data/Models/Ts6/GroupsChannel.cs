using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class GroupsChannel
{
    public int GroupId { get; set; }

    public int ServerId { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    public int? OrgGroupId { get; set; }
}
