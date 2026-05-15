using Microsoft.EntityFrameworkCore;
using ts3to6.Data;
using ts3to6.Data.Models.Ts6;

namespace Ts3Viewer.Data;

/// <summary>
/// EF Core context for querying a TS6 in-memory SQLite database.
/// Channel data is intentionally excluded here — channels require a JOIN with
/// channel_properties and are loaded via raw ADO.NET in the service layer.
/// </summary>
public class Ts6DbContext : DbContext
{
    public Ts6DbContext(DbContextOptions<Ts6DbContext> options) : base(options) { }

    public virtual DbSet<ApiKey> ApiKeys { get; set; }
    public virtual DbSet<Ban> Bans { get; set; }
    public virtual DbSet<Binding> Bindings { get; set; }
    public virtual DbSet<Channel> Channels { get; set; }
    public virtual DbSet<ChannelProperty> ChannelProperties { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<ClientProperty> ClientProperties { get; set; }
    public virtual DbSet<Complain> Complains { get; set; }
    public virtual DbSet<CustomField> CustomFields { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<GroupChannelToClient> GroupChannelToClients { get; set; }
    public virtual DbSet<GroupServerToClient> GroupServerToClients { get; set; }
    public virtual DbSet<GroupsChannel> GroupsChannels { get; set; }
    public virtual DbSet<GroupsServer> GroupsServers { get; set; }
    public virtual DbSet<InstanceProperty> InstanceProperties { get; set; }
    public virtual DbSet<Integration> Integrations { get; set; }
    public virtual DbSet<IntegrationAction> IntegrationActions { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<PermChannel> PermChannels { get; set; }
    public virtual DbSet<PermChannelClient> PermChannelClients { get; set; }
    public virtual DbSet<PermChannelGroup> PermChannelGroups { get; set; }
    public virtual DbSet<PermClient> PermClients { get; set; }
    public virtual DbSet<PermServerGroup> PermServerGroups { get; set; }
    public virtual DbSet<Revocation> Revocations { get; set; }
    public virtual DbSet<Server> Servers { get; set; }
    public virtual DbSet<ServerGroupAssignEntry> ServerGroupAssignEntries { get; set; }
    public virtual DbSet<ServerProperty> ServerProperties { get; set; }
    public virtual DbSet<TemporaryPassword> TemporaryPasswords { get; set; }
    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("api_keys");

            entity.Property(e => e.ApiKeyId).HasColumnName("api_key_id");
            entity.Property(e => e.ApiKeyCreatedAt)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("api_key_created_at");
            entity.Property(e => e.ApiKeyCustomId)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("api_key_custom_id");
            entity.Property(e => e.ApiKeyExpiresAt)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("api_key_expires_at");
            entity.Property(e => e.ApiKeyHash)
                .HasColumnType("VARCHAR(44)")
                .HasColumnName("api_key_hash");
            entity.Property(e => e.ApiKeyOwnerDbid)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("api_key_owner_dbid");
            entity.Property(e => e.ApiKeyScope)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("api_key_scope");
            entity.Property(e => e.ServerId)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<Ban>(entity =>
        {
            entity.ToTable("bans");

            entity.HasIndex(e => e.ServerId, "index_bans_serverid");

            entity.Property(e => e.BanId).HasColumnName("ban_id");
            entity.Property(e => e.BanEnforcements)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("ban_enforcements");
            entity.Property(e => e.BanHash)
                .HasColumnType("varchar(255)")
                .HasColumnName("ban_hash");
            entity.Property(e => e.BanInvokerClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("ban_invoker_client_id");
            entity.Property(e => e.BanInvokerName)
                .HasColumnType("varchar(255)")
                .HasColumnName("ban_invoker_name");
            entity.Property(e => e.BanInvokerUid)
                .HasColumnType("varchar(64)")
                .HasColumnName("ban_invoker_uid");
            entity.Property(e => e.BanIp)
                .HasColumnType("varchar(255)")
                .HasColumnName("ban_ip");
            entity.Property(e => e.BanLastnickname)
                .HasColumnType("varchar(100)")
                .HasColumnName("ban_lastnickname");
            entity.Property(e => e.BanLength)
                .HasColumnType("integer unsigned")
                .HasColumnName("ban_length");
            entity.Property(e => e.BanMytsid)
                .HasColumnType("varchar(64)")
                .HasColumnName("ban_mytsid");
            entity.Property(e => e.BanName)
                .HasColumnType("varchar(2048)")
                .HasColumnName("ban_name");
            entity.Property(e => e.BanReason)
                .HasColumnType("varchar(255)")
                .HasColumnName("ban_reason");
            entity.Property(e => e.BanTimestamp)
                .HasColumnType("integer unsigned")
                .HasColumnName("ban_timestamp");
            entity.Property(e => e.BanUid)
                .HasColumnType("varchar(255)")
                .HasColumnName("ban_uid");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<Binding>(entity =>
        {
            entity.ToTable("bindings");

            entity.Property(e => e.BindingId).HasColumnName("binding_id");
            entity.Property(e => e.Ip)
                .HasColumnType("varchar(45)")
                .HasColumnName("ip");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.ToTable("channels");

            entity.HasIndex(e => e.ChannelId, "index_channels_id");

            entity.HasIndex(e => e.ServerId, "index_channels_serverid");

            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.ChannelParentId)
                .HasColumnType("integer unsigned")
                .HasColumnName("channel_parent_id");
            entity.Property(e => e.OrgChannelId).HasColumnName("org_channel_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<ChannelProperty>(entity =>
        {
            entity.ToTable("channel_properties");
            entity.HasKey(e => new { e.ServerId, e.Id, e.Ident });

            entity.HasIndex(e => e.Id, "index_channel_properties_id");

            entity.HasIndex(e => e.ServerId, "index_channel_properties_serverid");

            entity.Property(e => e.Id)
                .HasColumnType("integer unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Ident)
                .HasColumnType("varchar(255)")
                .HasColumnName("ident");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.Value)
                .HasColumnType("varchar(8192)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("clients");

            entity.HasIndex(e => e.ClientLoginName, "IX_clients_client_login_name").IsUnique();

            entity.HasIndex(e => e.ClientId, "index_clients_id");

            entity.HasIndex(e => new { e.ClientLastconnected, e.ServerId }, "index_clients_lastconnectedserverid");

            entity.HasIndex(e => e.ServerId, "index_clients_serverid");

            entity.HasIndex(e => new { e.ClientUniqueId, e.ServerId }, "index_clients_uid");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.ClientLastconnected)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_lastconnected");
            entity.Property(e => e.ClientLastip)
                .HasColumnType("varchar(45)")
                .HasColumnName("client_lastip");
            entity.Property(e => e.ClientLoginName)
                .HasColumnType("varchar(100)")
                .HasColumnName("client_login_name");
            entity.Property(e => e.ClientLoginPassword)
                .HasColumnType("varchar(40)")
                .HasColumnName("client_login_password");
            entity.Property(e => e.ClientMonthDownload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_month_download");
            entity.Property(e => e.ClientMonthUpload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_month_upload");
            entity.Property(e => e.ClientNickname)
                .HasColumnType("varchar(100)")
                .HasColumnName("client_nickname");
            entity.Property(e => e.ClientTotalDownload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_total_download");
            entity.Property(e => e.ClientTotalUpload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_total_upload");
            entity.Property(e => e.ClientTotalconnections)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_totalconnections");
            entity.Property(e => e.ClientUniqueId)
                .HasColumnType("varchar(100)")
                .HasColumnName("client_unique_id");
            entity.Property(e => e.HomebaseSince)
                .HasColumnType("integer unsigned")
                .HasColumnName("homebase_since");
            entity.Property(e => e.OrgClientId).HasColumnName("org_client_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<ClientProperty>(entity =>
        {
            entity.ToTable("client_properties");
            entity.HasKey(e => new { e.ServerId, e.Id, e.Ident });

            entity.HasIndex(e => e.Id, "index_client_properties_id");

            entity.HasIndex(e => e.ServerId, "index_client_properties_serverid");

            entity.HasIndex(e => new { e.ServerId, e.Id, e.Ident }, "index_client_properties_serverid_id_ident");

            entity.Property(e => e.Id)
                .HasColumnType("integer unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Ident)
                .HasColumnType("varchar(100)")
                .HasColumnName("ident");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.Value)
                .HasColumnType("varchar(255)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Complain>(entity =>
        {
            entity.ToTable("complains");
            entity.HasKey(e => new {
                e.ServerId,
                e.ComplainFromClientId,
                e.ComplainToClientId,
                e.ComplainTimestamp
            });

            entity.HasIndex(e => e.ServerId, "index_complains_serverid");

            entity.Property(e => e.ComplainFromClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("complain_from_client_id");
            entity.Property(e => e.ComplainHash)
                .HasColumnType("varchar(255)")
                .HasColumnName("complain_hash");
            entity.Property(e => e.ComplainMessage)
                .HasColumnType("varchar(255)")
                .HasColumnName("complain_message");
            entity.Property(e => e.ComplainTimestamp)
                .HasColumnType("integer unsigned")
                .HasColumnName("complain_timestamp");
            entity.Property(e => e.ComplainToClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("complain_to_client_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<CustomField>(entity =>
        {
            entity.HasKey(e => new { e.ServerId, e.ClientId, e.Ident });

            entity.ToTable("custom_fields");

            entity.HasIndex(e => new { e.ServerId, e.ClientId }, "index_custom_fields_by_client");

            entity.HasIndex(e => new { e.ServerId, e.Ident }, "index_custom_fields_by_ident");

            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.ClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("client_id");
            entity.Property(e => e.Ident)
                .HasColumnType("varchar(100)")
                .HasColumnName("ident");
            entity.Property(e => e.Value)
                .HasColumnType("varchar(255)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
        });

        modelBuilder.Entity<GroupChannelToClient>(entity =>
        {
            entity.ToTable("group_channel_to_client");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.GroupId });

            entity.HasIndex(e => e.GroupId, "index_group_channel_to_client_id");

            entity.HasIndex(e => e.Id1, "index_group_channel_to_client_id1");

            entity.HasIndex(e => e.Id2, "index_group_channel_to_client_id2");

            entity.HasIndex(e => e.ServerId, "index_group_channel_to_client_serverid");

            entity.Property(e => e.GroupId)
                .HasColumnType("integer unsigned")
                .HasColumnName("group_id");
            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<GroupServerToClient>(entity =>
        {
            entity.ToTable("group_server_to_client");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.GroupId });

            entity.HasIndex(e => e.GroupId, "index_group_server_to_client_id");

            entity.HasIndex(e => e.Id1, "index_group_server_to_client_id1");

            entity.HasIndex(e => e.ServerId, "index_group_server_to_client_serverid");

            entity.Property(e => e.GroupId)
                .HasColumnType("integer unsigned")
                .HasColumnName("group_id");
            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<GroupsChannel>(entity =>
        {
            entity.HasKey(e => e.GroupId);

            entity.ToTable("groups_channel");

            entity.HasIndex(e => e.GroupId, "index_groups_channel_id");

            entity.HasIndex(e => e.ServerId, "index_groups_channel_serverid");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasColumnType("varchar(50)")
                .HasColumnName("name");
            entity.Property(e => e.OrgGroupId).HasColumnName("org_group_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<GroupsServer>(entity =>
        {
            entity.HasKey(e => e.GroupId);

            entity.ToTable("groups_server");

            entity.HasIndex(e => e.GroupId, "index_groups_server_id");

            entity.HasIndex(e => e.ServerId, "index_groups_server_serverid");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasColumnType("varchar(50)")
                .HasColumnName("name");
            entity.Property(e => e.OrgGroupId).HasColumnName("org_group_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<InstanceProperty>(entity =>
        {
            entity.ToTable("instance_properties");
            entity.HasKey(e => new { e.ServerId, e.Id, e.StringId, e.Ident });

            entity.HasIndex(e => e.Id, "index_instance_properties_id");

            entity.HasIndex(e => e.ServerId, "index_instance_properties_serverid");

            entity.HasIndex(e => e.StringId, "index_instance_properties_string_id");

            entity.Property(e => e.Id)
                .HasColumnType("integer unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Ident)
                .HasColumnType("varchar(100)")
                .HasColumnName("ident");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.StringId)
                .HasColumnType("varchar(255)")
                .HasColumnName("string_id");
            entity.Property(e => e.Value)
                .HasColumnType("varchar(255)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Integration>(entity =>
        {
            entity.HasKey(e => new { e.ServerId, e.IntegrationId });

            entity.ToTable("integrations");

            entity.HasIndex(e => e.ServerId, "index_integrations_server_id");

            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.IntegrationId)
                .HasColumnType("varchar(36)")
                .HasColumnName("integration_id");
            entity.Property(e => e.IntegrationType)
                .HasColumnType("integer unsigned")
                .HasColumnName("integration_type");
            entity.Property(e => e.IntegrationUserInfo)
                .HasColumnType("varchar(4096)")
                .HasColumnName("integration_user_info");
        });

        modelBuilder.Entity<IntegrationAction>(entity =>
        {
            entity.ToTable("integration_actions");

            entity.HasIndex(e => e.ServerId, "index_integration_actions_server_id");

            entity.Property(e => e.IntegrationActionId).HasColumnName("integration_action_id");
            entity.Property(e => e.IntegrationActionType)
                .HasColumnType("integer unsigned")
                .HasColumnName("integration_action_type");
            entity.Property(e => e.IntegrationActionValue)
                .HasColumnType("varchar(255)")
                .HasColumnName("integration_action_value");
            entity.Property(e => e.IntegrationId)
                .HasColumnType("varchar(36)")
                .HasColumnName("integration_id");
            entity.Property(e => e.IntegrationResponseType)
                .HasColumnType("integer unsigned")
                .HasColumnName("integration_response_type");
            entity.Property(e => e.IntegrationResponseValue)
                .HasColumnType("varchar(255)")
                .HasColumnName("integration_response_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages");

            entity.HasIndex(e => new { e.MessageToClientId, e.MessageFlagRead }, "index_messages_msgidtoclid_read");

            entity.HasIndex(e => e.ServerId, "index_messages_serverid");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.MessageFlagRead)
                .HasDefaultValue(0)
                .HasColumnName("message_flag_read");
            entity.Property(e => e.MessageFromClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("message_from_client_id");
            entity.Property(e => e.MessageFromClientUid)
                .HasColumnType("varchar(40)")
                .HasColumnName("message_from_client_uid");
            entity.Property(e => e.MessageMsg).HasColumnName("message_msg");
            entity.Property(e => e.MessageSubject)
                .HasColumnType("varchar(255)")
                .HasColumnName("message_subject");
            entity.Property(e => e.MessageTimestamp)
                .HasColumnType("integer unsigned")
                .HasColumnName("message_timestamp");
            entity.Property(e => e.MessageToClientId)
                .HasColumnType("integer unsigned")
                .HasColumnName("message_to_client_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<PermChannel>(entity =>
        {
            entity.ToTable("perm_channel");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.PermId });

            entity.HasIndex(e => e.ServerId, "index_perm_channel_serverid");

            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.PermId)
                .HasColumnType("varchar(100)")
                .HasColumnName("perm_id");
            entity.Property(e => e.PermNegated).HasColumnName("perm_negated");
            entity.Property(e => e.PermSkip).HasColumnName("perm_skip");
            entity.Property(e => e.PermValue).HasColumnName("perm_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<PermChannelClient>(entity =>
        {
            entity.ToTable("perm_channel_clients");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.PermId });            

            entity.HasIndex(e => e.ServerId, "index_perm_channel_clients_serverid");

            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.PermId)
                .HasColumnType("varchar(100)")
                .HasColumnName("perm_id");
            entity.Property(e => e.PermNegated).HasColumnName("perm_negated");
            entity.Property(e => e.PermSkip).HasColumnName("perm_skip");
            entity.Property(e => e.PermValue).HasColumnName("perm_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<PermChannelGroup>(entity =>
        {
            entity.ToTable("perm_channel_groups");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.PermId });            

            entity.HasIndex(e => e.ServerId, "index_perm_channel_groups_serverid");

            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.PermId)
                .HasColumnType("varchar(100)")
                .HasColumnName("perm_id");
            entity.Property(e => e.PermNegated).HasColumnName("perm_negated");
            entity.Property(e => e.PermSkip).HasColumnName("perm_skip");
            entity.Property(e => e.PermValue).HasColumnName("perm_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<PermClient>(entity =>
        {            
            entity.ToTable("perm_client");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.PermId });

            entity.HasIndex(e => e.ServerId, "index_perm_client_serverid");

            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.PermId)
                .HasColumnType("varchar(100)")
                .HasColumnName("perm_id");
            entity.Property(e => e.PermNegated).HasColumnName("perm_negated");
            entity.Property(e => e.PermSkip).HasColumnName("perm_skip");
            entity.Property(e => e.PermValue).HasColumnName("perm_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<PermServerGroup>(entity =>
        {
            entity.ToTable("perm_server_group");
            entity.HasKey(e => new { e.ServerId, e.Id1, e.Id2, e.PermId });

            entity.HasIndex(e => e.ServerId, "index_perm_server_group_serverid");

            entity.HasIndex(e => new { e.ServerId, e.Id1, e.PermId, e.PermValue, e.PermNegated }, "index_perm_server_group_serverid_negated");

            entity.Property(e => e.Id1)
                .HasColumnType("integer unsigned")
                .HasColumnName("id1");
            entity.Property(e => e.Id2)
                .HasColumnType("integer unsigned")
                .HasColumnName("id2");
            entity.Property(e => e.PermId)
                .HasColumnType("varchar(100)")
                .HasColumnName("perm_id");
            entity.Property(e => e.PermNegated).HasColumnName("perm_negated");
            entity.Property(e => e.PermSkip).HasColumnName("perm_skip");
            entity.Property(e => e.PermValue).HasColumnName("perm_value");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<Revocation>(entity =>
        {
            entity.HasKey(e => new { e.RevocationType, e.RevocationKey });

            entity.ToTable("revocations");

            entity.Property(e => e.RevocationType)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("revocation_type");
            entity.Property(e => e.RevocationKey)
                .HasColumnType("VARCHAR(44)")
                .HasColumnName("revocation_key");
            entity.Property(e => e.RevocationExpiration)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("revocation_expiration");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.ToTable("servers");

            entity.HasIndex(e => e.ServerPort, "index_servers_port");

            entity.HasIndex(e => e.ServerId, "index_servers_serverid");

            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.ServerAutostart)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_autostart");
            entity.Property(e => e.ServerMachineId)
                .HasColumnType("varchar(50)")
                .HasColumnName("server_machine_id");
            entity.Property(e => e.ServerMonthDownload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_month_download");
            entity.Property(e => e.ServerMonthUpload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_month_upload");
            entity.Property(e => e.ServerPort)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_port");
            entity.Property(e => e.ServerTotalDownload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_total_download");
            entity.Property(e => e.ServerTotalUpload)
                .HasDefaultValue(0)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_total_upload");
        });

        modelBuilder.Entity<ServerGroupAssignEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId);

            entity.ToTable("server_group_assign_entries");

            entity.HasIndex(e => e.VsGroupId, "index_server_group_assign_entries_group");

            entity.HasIndex(e => e.InvokerClientDbId, "index_server_group_assign_entries_invoker");

            entity.HasIndex(e => e.ServerId, "index_server_group_assign_entries_serverid");

            entity.HasIndex(e => e.EntryType, "index_server_group_assign_entries_type");

            entity.Property(e => e.EntryId).HasColumnName("entry_id");
            entity.Property(e => e.ClientUid)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("client_uid");
            entity.Property(e => e.CreatedTimestamp)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("created_timestamp");
            entity.Property(e => e.EntryType).HasColumnName("entry_type");
            entity.Property(e => e.InvokerClientDbId)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("invoker_client_db_id");
            entity.Property(e => e.MigrationValue)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("migration_value");
            entity.Property(e => e.ModifyPower).HasColumnName("modify_power");
            entity.Property(e => e.Mytsid)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("mytsid");
            entity.Property(e => e.ServerId)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("server_id");
            entity.Property(e => e.Ttl)
                .HasDefaultValue(0)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("ttl");
            entity.Property(e => e.VsGroupId)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("vs_group_id");
        });

        modelBuilder.Entity<ServerProperty>(entity =>
        {
            entity.ToTable("server_properties");
            entity.HasKey(e => new { e.ServerId, e.Id, e.Ident });

            entity.HasIndex(e => e.ServerId, "index_server_prop_sid");

            entity.HasIndex(e => e.Id, "index_server_properties_id");

            entity.Property(e => e.Id)
                .HasColumnType("integer unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Ident)
                .HasColumnType("varchar(100)")
                .HasColumnName("ident");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.Value)
                .HasColumnType("varchar(255)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<TemporaryPassword>(entity =>
        {
            entity.HasKey(e => new { e.ServerId, e.TemporaryPasswordHash });

            entity.ToTable("temporary_passwords");

            entity.Property(e => e.ServerId)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("server_id");
            entity.Property(e => e.TemporaryPasswordHash)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("temporary_password_hash");
            entity.Property(e => e.TemporaryPasswordChannelId).HasColumnName("temporary_password_channel_id");
            entity.Property(e => e.TemporaryPasswordChannelPassword)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("temporary_password_channel_password");
            entity.Property(e => e.TemporaryPasswordCreatorId).HasColumnName("temporary_password_creator_id");
            entity.Property(e => e.TemporaryPasswordDescription)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("temporary_password_description");
            entity.Property(e => e.TemporaryPasswordEndTimestamp)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("temporary_password_end_timestamp");
            entity.Property(e => e.TemporaryPasswordPlaintext)
                .HasColumnType("VARCHAR(255)")
                .HasColumnName("temporary_password_plaintext");
            entity.Property(e => e.TemporaryPasswordStartTimestamp)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("temporary_password_start_timestamp");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("tokens");
            entity.HasKey(e => new { e.ServerId, e.TokenKey });

            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
            entity.Property(e => e.TokenCreated)
                .HasColumnType("integer unsigned")
                .HasColumnName("token_created");
            entity.Property(e => e.TokenCustomset)
                .HasColumnType("varchar(255)")
                .HasColumnName("token_customset");
            entity.Property(e => e.TokenDescription)
                .HasColumnType("varchar(255)")
                .HasColumnName("token_description");
            entity.Property(e => e.TokenFromClientId)
                .HasColumnType("int unsigned")
                .HasColumnName("token_from_client_id");
            entity.Property(e => e.TokenId1)
                .HasColumnType("integer unsigned")
                .HasColumnName("token_id1");
            entity.Property(e => e.TokenId2)
                .HasColumnType("integer unsigned")
                .HasColumnName("token_id2");
            entity.Property(e => e.TokenKey)
                .HasColumnType("varchar(50)")
                .HasColumnName("token_key");
            entity.Property(e => e.TokenType).HasColumnName("token_type");
        });
    }
}
