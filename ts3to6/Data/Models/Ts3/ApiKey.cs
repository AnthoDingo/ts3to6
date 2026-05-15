using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("api_keys")]
public partial class ApiKey
{
    public int ApiKeyId { get; set; }

    public int ServerId { get; set; }

    public string ApiKeyHash { get; set; } = null!;

    public int ApiKeyOwnerDbid { get; set; }

    public int ApiKeyScope { get; set; }

    public int ApiKeyCreatedAt { get; set; }

    public int ApiKeyExpiresAt { get; set; }
}
