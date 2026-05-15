using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("groups_servers")]
public partial class GroupsServer
{
    public int GroupId { get; set; }

    public int ServerId { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    public int? OrgGroupId { get; set; }
}
