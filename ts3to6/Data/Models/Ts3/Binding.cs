using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("bindings")]
public partial class Binding
{
    public int BindingId { get; set; }

    public string Ip { get; set; } = null!;

    public int? Type { get; set; }
}
