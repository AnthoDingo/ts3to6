using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("tokens")]
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
