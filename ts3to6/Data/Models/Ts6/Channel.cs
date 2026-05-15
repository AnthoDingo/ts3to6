using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Channel
{
    public int ChannelId { get; set; }

    public int? ChannelParentId { get; set; }

    public int ServerId { get; set; }

    public int? OrgChannelId { get; set; }
}
