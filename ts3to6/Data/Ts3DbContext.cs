using Microsoft.EntityFrameworkCore;
using ts3to6.Data.Models.Ts3;

namespace ts3to6.Data;

/// <summary>
/// EF Core context for querying an in-memory TS3 SQLite database.
/// Always constructed with an already-open <see cref="Microsoft.Data.Sqlite.SqliteConnection"/>
/// so EF Core never closes it (which would destroy the in-memory data).
/// No migrations are used — tables already exist in the deserialized database.
/// </summary>
public class Ts3DbContext : DbContext
{
    public Ts3DbContext(DbContextOptions<Ts3DbContext> options) : base(options) { }

    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<Ban> Bans => Set<Ban>();
    public DbSet<Binding> Bindings => Set<Binding>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<ChannelProperty> ChannelProperties => Set<ChannelProperty>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientProperty> ClientProperties => Set<ClientProperty>();
    public DbSet<Complain> Complains => Set<Complain>();
    public DbSet<CustomField> CustomFields => Set<CustomField>();
    public DbSet<GroupsServer> GroupsServers => Set<GroupsServer>();
    public DbSet<GroupsChannel> GroupsChannel => Set<GroupsChannel>();
    public DbSet<GroupChannelToClient> GroupChannelToClient => Set<GroupChannelToClient>();
    public DbSet<GroupServerToClient> GroupServerToClient => Set<GroupServerToClient>();
    public DbSet<InstanceProperty> InstanceProperties => Set<InstanceProperty>();
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<IntegrationAction> IntegrationActions => Set<IntegrationAction>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<PermChannel> PermChannel => Set<PermChannel>();
    public DbSet<PermChannelClient> PermChannelClients => Set<PermChannelClient>();
    public DbSet<PermChannelGroup> PermChannelGroups => Set<PermChannelGroup>();
    public DbSet<PermClient> PermClient => Set<PermClient>();
    public DbSet<PermServerGroup> PermServerGroup => Set<PermServerGroup>();
    public DbSet<Revocation> Revocations => Set<Revocation>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<ServerProperty> ServerProperties => Set<ServerProperty>();
    public DbSet<TemporaryPassword> TemporaryPassword => Set<TemporaryPassword>();
    public DbSet<Token> Tokens => Set<Token>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("api_keys");

            entity.Property(e => e.ApiKeyId).HasColumnName("api_key_id");
            entity.Property(e => e.ApiKeyCreatedAt)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("api_key_created_at");
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
                .HasColumnType("varchar(40)")
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
                .HasColumnType("varchar(44)")
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
            entity
                .HasNoKey()
                .ToTable("channel_properties");

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
                .HasColumnType("varchar(20)")
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
                .HasColumnType("varchar(40)")
                .HasColumnName("client_unique_id");
            entity.Property(e => e.OrgClientId).HasColumnName("org_client_id");
            entity.Property(e => e.ServerId)
                .HasColumnType("integer unsigned")
                .HasColumnName("server_id");
        });

        modelBuilder.Entity<ClientProperty>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("client_properties");

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
            entity
                .HasNoKey()
                .ToTable("complains");

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

        modelBuilder.Entity<GroupChannelToClient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("group_channel_to_client");

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
            entity
                .HasNoKey()
                .ToTable("group_server_to_client");

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
            entity
                .HasNoKey()
                .ToTable("instance_properties");

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
            entity
                .HasNoKey()
                .ToTable("perm_channel");

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
            entity
                .HasNoKey()
                .ToTable("perm_channel_clients");

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
            entity
                .HasNoKey()
                .ToTable("perm_channel_groups");

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
            entity
                .HasNoKey()
                .ToTable("perm_client");

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
            entity
                .HasNoKey()
                .ToTable("perm_server_group");

            entity.HasIndex(e => e.ServerId, "index_perm_server_group_serverid");

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

        modelBuilder.Entity<ServerProperty>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("server_properties");

            entity.HasIndex(e => e.Id, "index_server_properties_id");

            entity.HasIndex(e => e.ServerId, "index_server_properties_serverid");

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
                .HasColumnType("VARCHAR(28)")
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
            entity
                .HasNoKey()
                .ToTable("tokens");

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
