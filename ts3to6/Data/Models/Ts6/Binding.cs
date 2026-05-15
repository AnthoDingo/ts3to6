using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Binding
{
    public int BindingId { get; set; }

    public string Ip { get; set; } = null!;

    public int? Type { get; set; }
}
