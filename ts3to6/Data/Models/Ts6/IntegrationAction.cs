using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class IntegrationAction
{
    public int IntegrationActionId { get; set; }

    public int ServerId { get; set; }

    public string IntegrationId { get; set; } = null!;

    public int IntegrationResponseType { get; set; }

    public string IntegrationResponseValue { get; set; } = null!;

    public int IntegrationActionType { get; set; }

    public string IntegrationActionValue { get; set; } = null!;
}
