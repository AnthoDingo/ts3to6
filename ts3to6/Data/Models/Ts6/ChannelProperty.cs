using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class ChannelProperty
{
    public int? ServerId { get; set; }

    public int? Id { get; set; }

    public string Ident { get; set; } = null!;

    public string? Value { get; set; }
}
