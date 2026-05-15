using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class ApiKey
{
    public int ApiKeyId { get; set; }

    public int ServerId { get; set; }

    public string ApiKeyHash { get; set; } = null!;

    public int ApiKeyOwnerDbid { get; set; }

    public int ApiKeyScope { get; set; }

    public int ApiKeyCreatedAt { get; set; }

    public int ApiKeyExpiresAt { get; set; }

    public string ApiKeyCustomId { get; set; } = null!;
}
