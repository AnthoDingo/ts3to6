namespace ts3to6.Data.Models.Ts6;

public partial class Client
{
    public int ClientId { get; set; }

    public int? ServerId { get; set; }

    public string? ClientUniqueId { get; set; }

    public string? ClientNickname { get; set; }

    public string? ClientLoginName { get; set; }

    public string? ClientLoginPassword { get; set; }

    public int? ClientLastconnected { get; set; }

    public int? ClientTotalconnections { get; set; }

    public int? ClientMonthUpload { get; set; }

    public int? ClientMonthDownload { get; set; }

    public int? ClientTotalUpload { get; set; }

    public int? ClientTotalDownload { get; set; }

    public string? ClientLastip { get; set; }

    public int? OrgClientId { get; set; }

    public int? HomebaseSince { get; set; }
}
