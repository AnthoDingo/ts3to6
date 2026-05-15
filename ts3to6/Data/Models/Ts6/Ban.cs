using System;
using System.Collections.Generic;

namespace ts3to6.Data.Models.Ts6;

public partial class Ban
{
    public int BanId { get; set; }

    public int? ServerId { get; set; }

    public string? BanIp { get; set; }

    public string? BanName { get; set; }

    public string? BanUid { get; set; }

    public int? BanTimestamp { get; set; }

    public int? BanLength { get; set; }

    public int? BanInvokerClientId { get; set; }

    public string? BanInvokerUid { get; set; }

    public string? BanInvokerName { get; set; }

    public string? BanReason { get; set; }

    public int? BanEnforcements { get; set; }

    public string? BanHash { get; set; }

    public string? BanMytsid { get; set; }

    public string? BanLastnickname { get; set; }
}
