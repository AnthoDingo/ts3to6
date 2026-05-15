using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Integration
{
    public int ServerId { get; set; }

    public string IntegrationId { get; set; } = null!;

    public int IntegrationType { get; set; }

    public string? IntegrationUserInfo { get; set; }
}
