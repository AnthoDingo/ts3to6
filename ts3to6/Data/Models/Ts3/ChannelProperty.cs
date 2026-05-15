using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("channel_properties")]
public partial class ChannelProperty
{
    public int? ServerId { get; set; }

    public int? Id { get; set; }

    public string Ident { get; set; } = null!;

    public string? Value { get; set; }
}
