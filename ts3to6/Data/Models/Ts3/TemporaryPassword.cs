using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("temporary_passwords")]
public partial class TemporaryPassword
{
    public int ServerId { get; set; }

    public string TemporaryPasswordHash { get; set; } = null!;

    public string TemporaryPasswordPlaintext { get; set; } = null!;

    public int TemporaryPasswordCreatorId { get; set; }

    public int TemporaryPasswordStartTimestamp { get; set; }

    public int TemporaryPasswordEndTimestamp { get; set; }

    public int TemporaryPasswordChannelId { get; set; }

    public string? TemporaryPasswordChannelPassword { get; set; }

    public string? TemporaryPasswordDescription { get; set; }
}
