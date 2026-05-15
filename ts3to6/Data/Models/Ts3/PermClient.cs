using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("perm_client")]
public partial class PermClient
{
    public int ServerId { get; set; }

    public int Id1 { get; set; }

    public int Id2 { get; set; }

    public string PermId { get; set; } = null!;

    public int? PermValue { get; set; }

    public int? PermNegated { get; set; }

    public int? PermSkip { get; set; }
}
