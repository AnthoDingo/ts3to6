using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("group_channel_to_client")]
public partial class GroupChannelToClient
{
    public int GroupId { get; set; }

    public int ServerId { get; set; }

    public int Id1 { get; set; }

    public int Id2 { get; set; }
}
