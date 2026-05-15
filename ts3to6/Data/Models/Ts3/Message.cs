using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("messages")]
public partial class Message
{
    public int MessageId { get; set; }

    public int? ServerId { get; set; }

    public int? MessageFromClientId { get; set; }

    public string? MessageFromClientUid { get; set; }

    public int? MessageToClientId { get; set; }

    public string? MessageSubject { get; set; }

    public string? MessageMsg { get; set; }

    public int? MessageTimestamp { get; set; }

    public int? MessageFlagRead { get; set; }
}
