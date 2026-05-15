using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Event
{
    public int Id { get; set; }

    public string Data { get; set; } = null!;
}
