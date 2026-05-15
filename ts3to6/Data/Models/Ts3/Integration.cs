using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("integrations")]
public partial class Integration
{
    public int ServerId { get; set; }

    public string IntegrationId { get; set; } = null!;

    public int IntegrationType { get; set; }

    public string? IntegrationUserInfo { get; set; }
}
