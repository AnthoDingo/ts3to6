using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("servers")]
public partial class Server
{
    public int ServerId { get; set; }

    public int? ServerPort { get; set; }

    public int? ServerAutostart { get; set; }

    public string? ServerMachineId { get; set; }

    public int? ServerMonthUpload { get; set; }

    public int? ServerMonthDownload { get; set; }

    public int? ServerTotalUpload { get; set; }

    public int? ServerTotalDownload { get; set; }
}
