using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("channels")]
public partial class Channel
{
    public int ChannelId { get; set; }

    public int? ChannelParentId { get; set; }

    public int ServerId { get; set; }

    public int? OrgChannelId { get; set; }
}
