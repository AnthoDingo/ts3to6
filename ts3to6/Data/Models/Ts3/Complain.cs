using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("complains")]
public partial class Complain
{
    public int? ServerId { get; set; }

    public int? ComplainFromClientId { get; set; }

    public int? ComplainToClientId { get; set; }

    public string? ComplainMessage { get; set; }

    public int? ComplainTimestamp { get; set; }

    public string? ComplainHash { get; set; }
}
