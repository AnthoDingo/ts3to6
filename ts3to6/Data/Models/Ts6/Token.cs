using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Token
{
    public int? ServerId { get; set; }

    public string TokenKey { get; set; } = null!;

    public int? TokenType { get; set; }

    public int? TokenId1 { get; set; }

    public int? TokenId2 { get; set; }

    public int? TokenCreated { get; set; }

    public string? TokenDescription { get; set; }

    public string? TokenCustomset { get; set; }

    public int? TokenFromClientId { get; set; }
}
