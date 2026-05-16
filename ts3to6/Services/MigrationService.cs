using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ts3to6.Data;
using ts3to6.Data.Models.Ts3;
using ts3to6.Enums;
using Ts3Viewer.Data;

namespace ts3to6.Services;

/// <summary>
/// Scoped service that converts a loaded TS3 in-memory database into a TS6-compatible
/// <c>tsserver.sqlitedb</c> file and stores it in <see cref="DownloadCache"/>.
/// </summary>
public sealed class MigrationService
{
    private readonly TsDbService _ts3;
    private Ts3DbContext _ts3DbContext;
    private readonly DownloadCache      _cache;

    

    public MigrationService(TsDbService ts3, DownloadCache cache)
    {
        _ts3   = ts3;
        _cache = cache;
    }

    // ── Result / Stats ──────────────────────────────────────────────────────

    public sealed record MigrationResult(
        bool   Success,
        string? Error,
        string? DownloadId,
        long    FileSizeBytes,
        MigrationStats Stats
    );

    public sealed class MigrationStats
    {
        public int Servers { get; init; }
        public int Clients         { get; init; }
        public int Channels        { get; init; }
        public int ServerProperties{ get; init; }
        public int Bans            { get; init; }
        public int ServerGroups    { get; init; }
        public int ChannelGroups   { get; init; }
        public int GroupAssignments{ get; init; }
        public int Perms { get; init; }
        public int Tokens { get; init; }
        public int Messages { get; init; }
        public IReadOnlyList<string> Warnings { get; init; } = [];
    }

    // ── Entry point ─────────────────────────────────────────────────────────

    public async Task<MigrationResult> RunAsync()
    {
        _ts3DbContext = _ts3.Ts3DbContext;

        if (_ts3.Connection is not { } ts3Conn)
            return Fail("TS3 database not loaded.");

        if (_ts3.Version != DbVersion.Ts3)
            return Fail("This file is not a valid TeamSpeak 3 database.");

        string tempPath = Path.Combine(Path.GetTempPath(), $"ts6_{Guid.NewGuid():N}.db");
        List<string> warnings = new List<string>();

        try
        {
            DbContextOptions<Ts6DbContext> options = new DbContextOptionsBuilder<Ts6DbContext>()
                .UseSqlite("Data Source=Ts6PureMemory;Mode=Memory;Cache=Shared")
                .Options;
            Ts6DbContext ts6DbContext = new Ts6DbContext(options);
            await ts6DbContext.Database.OpenConnectionAsync();
            //await ts6DbContext.Database.MigrateAsync();
            await ts6DbContext.Database.EnsureCreatedAsync();

            // 2 — Migrate each section in order of FK dependency

            int servers     = await MigrateServersAsync            (_ts3DbContext, ts6DbContext, warnings);
            int clients     = await MigrateClientsAsync            (_ts3DbContext, ts6DbContext, warnings);
            int channels    = await MigrateChannelsAsync           (_ts3DbContext, ts6DbContext, warnings);
            int properties  = await MigrateServerPropertiesAsync   (_ts3DbContext, ts6DbContext, warnings);
            int bans        = await MigrateBansAsync               (_ts3DbContext, ts6DbContext, warnings);
            int sGroups     = await MigrateServerGroupsAsync       (_ts3DbContext, ts6DbContext, warnings);
            int cGroups     = await MigrateChannelGroupsAsync      (_ts3DbContext, ts6DbContext, warnings);
            int assignments = await MigrateGroupAssignmentsAsync   (_ts3DbContext, ts6DbContext, warnings);
            int perms       = await MigratePermissionsAsync        (_ts3DbContext, ts6DbContext, warnings);
            int tokens      = await MigrateTokensAsync             (_ts3DbContext, ts6DbContext, warnings);
            int message     = await MigrateMessagesAsync           (_ts3DbContext, ts6DbContext, warnings);

            // Export the in-memory database to a physical file so we can read its bytes
            using (var fileConn = new SqliteConnection($"Data Source={tempPath}"))
            {
                await fileConn.OpenAsync();
                if(ts6DbContext.Database.GetDbConnection() is SqliteConnection sourceconn)
                {
                    sourceconn.BackupDatabase(fileConn);
                }
            }

            SqliteConnection.ClearAllPools();
            byte[] bytes = await File.ReadAllBytesAsync(tempPath);
            string id    = _cache.Store(bytes);

            return new MigrationResult(true, null, id, bytes.Length, new MigrationStats
            {
                Clients          = clients,
                Channels         = channels,
                ServerProperties = properties,
                Bans             = bans,
                ServerGroups     = sGroups,
                ChannelGroups    = cGroups,
                GroupAssignments = assignments,
                Perms            = perms,
                Tokens           = tokens,
                Messages         = message,
                Warnings         = warnings,
            });
        }
        catch (Exception ex)
        {
            return Fail($"Unexpected error: {ex.Message}");
        }
        finally
        {
            try { File.Delete(tempPath); } catch {
                Debug.WriteLine($"Failed to delete file {tempPath}");
            }
        }
    }

    // ── Table migrations ─────────────────────────────────────────────────────

    #region Table migrations

    private static async Task<int> MigrateServersAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.Server server in ts3.Servers.AsNoTracking())
            {
                ts6.Servers.Add(new Data.Models.Ts6.Server
                {
                    ServerId = server.ServerId,
                    ServerPort = server.ServerPort,
                    ServerAutostart = server.ServerAutostart,
                    ServerMachineId = server.ServerMachineId,
                    ServerMonthUpload = server.ServerMonthUpload,
                    ServerMonthDownload = server.ServerMonthDownload,
                    ServerTotalUpload = server.ServerTotalUpload,
                    ServerTotalDownload = server.ServerTotalDownload,
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Servers: {ex.Message}"); }

        try
        {
            foreach (Data.Models.Ts3.InstanceProperty instance in ts3.InstanceProperties.AsNoTracking())
            {
                string value;
                switch (instance.Ident)
                {
                    case "serverinstance_database_version":
                        // TS3 version 3.13.6 and below used "65" as the database version, but TS6 requires "66"
                        value = "66";
                        break;
                    case "serverinstance_permissions_version":
                        value = "26";
                        break;
                    default:
                        value = instance.Value;
                        break;
                }
                ts6.InstanceProperties.Add(new Data.Models.Ts6.InstanceProperty
                {
                    ServerId = instance.ServerId,
                    StringId = instance.StringId,
                    Id = instance.Id,
                    Ident = instance.Ident,
                    Value = value,
                });
                await ts6.SaveChangesAsync();
                
            }
            ts6.InstanceProperties.Add(new Data.Models.Ts6.InstanceProperty
            {
                ServerId = 0,
                StringId = "0",
                Id = 0,
                Ident = "serverinstance_max_homebases",
                Value = "-1",
            });
            await ts6.SaveChangesAsync();
        }
        catch (Exception ex) { warn.Add($"Instance Properties: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateClientsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach (Data.Models.Ts3.Client client in ts3.Clients.AsNoTracking())
            {
                ts6.Clients.Add(new Data.Models.Ts6.Client
                {
                    ClientId = client.ClientId,
                    ServerId = client.ServerId,
                    ClientUniqueId = client.ClientUniqueId,
                    ClientNickname = client.ClientNickname,
                    ClientLoginName = client.ClientLoginName,
                    ClientLoginPassword = client.ClientLoginPassword,
                    ClientLastconnected = client.ClientLastconnected,
                    ClientTotalconnections = client.ClientTotalconnections,
                    ClientMonthUpload = client.ClientMonthUpload,
                    ClientMonthDownload = client.ClientMonthDownload,
                    ClientTotalUpload = client.ClientTotalUpload,
                    ClientTotalDownload = client.ClientTotalDownload,
                    ClientLastip = client.ClientLastip,
                    OrgClientId = client.OrgClientId
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Clients: {ex.Message}"); }

        try
        {
            foreach (Data.Models.Ts3.ClientProperty prop in ts3.ClientProperties.AsNoTracking())
            {
                ts6.ClientProperties.Add(new Data.Models.Ts6.ClientProperty
                {
                    ServerId = prop.ServerId,
                    Id = prop.Id,
                    Ident = prop.Ident,
                    Value = prop.Value
                });
                await ts6.SaveChangesAsync();
            }
        }
        catch (Exception ex) { warn.Add($"Client properties: {ex.Message}"); }
            
        return n;
    }

    private static async Task<int> MigrateChannelsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.Channel ch in ts3.Channels.AsNoTracking())
            {
                ts6.Channels.Add(new Data.Models.Ts6.Channel
                {
                    ChannelId = ch.ChannelId,
                    ChannelParentId = ch.ChannelParentId,
                    ServerId = ch.ServerId,
                    OrgChannelId = ch.OrgChannelId
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Channels: {ex.Message}"); }
        try
        {
            foreach (Data.Models.Ts3.ChannelProperty prop in ts3.ChannelProperties.AsNoTracking())
            {
                ts6.ChannelProperties.Add(new Data.Models.Ts6.ChannelProperty
                {
                    ServerId = prop.ServerId,
                    Id = prop.Id,
                    Ident = prop.Ident,
                    Value = prop.Value
                });
                await ts6.SaveChangesAsync();
            }
        }
        catch (Exception ex) { warn.Add($"Channel properties: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateServerPropertiesAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.ServerProperty prop in ts3.ServerProperties.AsNoTracking())
            {
                if (prop.Ident == "virtualserver_file_storage_class")
                    continue;

                ts6.ServerProperties.Add(new Data.Models.Ts6.ServerProperty
                {
                    ServerId = prop.ServerId,
                    Id = prop.Id,
                    Ident = prop.Ident,
                    Value = prop.Value
                });
                await ts6.SaveChangesAsync();
                n++;
            }

            Dictionary<string, string> defaultProps = new Dictionary<string, string>
            {
                { "virtualserver_address", string.Empty },
                { "virtualserver_storage_quota", "4294967295" },
                { "virtualserver_webrtc_certificate", string.Empty },
                { "virtualserver_webrtc_private_key", string.Empty },
                { "virtualserver_canonical_name", string.Empty },
                { "virtualserver_mytsid_connect_only", "0" },
                { "virtualserver_max_homebases", "64" },
                { "virtualserver_homebase_storage_quota", "4294967295" },
                { "virtualserver_sfu_endpoint", string.Empty },
            };
            foreach (var kvp in defaultProps)
            {
                if (!ts6.ServerProperties.Any(p => p.Ident == kvp.Key))
                {
                    ts6.ServerProperties.Add(new Data.Models.Ts6.ServerProperty
                    {
                        ServerId = 1,
                        Id = 1,
                        Ident = kvp.Key,
                        Value = kvp.Value
                    });
                    await ts6.SaveChangesAsync();
                    n++;
                }
            }
        }
        catch (Exception ex) { warn.Add($"Server properties: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateBansAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.Ban ban in ts3.Bans.AsNoTracking())
            {
                ts6.Bans.Add(new Data.Models.Ts6.Ban
                {
                    BanId = ban.BanId,
                    ServerId = ban.ServerId,
                    BanIp = ban.BanIp,
                    BanName = ban.BanName,
                    BanUid = ban.BanUid,
                    BanTimestamp = ban.BanTimestamp,
                    BanLength = ban.BanLength,
                    BanInvokerClientId = ban.BanInvokerClientId,
                    BanInvokerName = ban.BanInvokerName,
                    BanReason = ban.BanReason,
                    BanEnforcements = ban.BanEnforcements,
                    BanLastnickname = ban.BanLastnickname
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Bans: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateServerGroupsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.GroupsServer grp in ts3.GroupsServers.AsNoTracking())
            {
                ts6.GroupsServers.Add(new Data.Models.Ts6.GroupsServer
                {
                    GroupId = grp.GroupId,
                    ServerId = grp.ServerId,
                    Name = grp.Name,
                    Type = grp.Type,
                    OrgGroupId = grp.OrgGroupId
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Server groups: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateChannelGroupsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach(Data.Models.Ts3.GroupsChannel grp in ts3.GroupsChannel.AsNoTracking())
            {
                ts6.GroupsChannels.Add(new Data.Models.Ts6.GroupsChannel
                {
                    GroupId = grp.GroupId,
                    ServerId = grp.ServerId,
                    Name = grp.Name,
                    Type = grp.Type,
                    OrgGroupId = grp.OrgGroupId
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Channel groups: {ex.Message}"); }
        return n;
    }

    private static async Task<int> MigrateGroupAssignmentsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int total = 0;

        // ── server group ↔ client ──────────────────────────────────────────
        try
        {
            foreach(Data.Models.Ts3.GroupServerToClient assign in ts3.GroupServerToClient.AsNoTracking())
            {
                ts6.GroupServerToClients.Add(new Data.Models.Ts6.GroupServerToClient
                {
                    GroupId = assign.GroupId,
                    ServerId = assign.ServerId,
                    Id1 = assign.Id1,
                    Id2 = assign.Id2,
                });
                await ts6.SaveChangesAsync();
                total++;
            }
        }
        catch (Exception ex) { warn.Add($"Server group assignments: {ex.Message}"); }

        // ── channel group ↔ client ─────────────────────────────────────────
        try
        {
            foreach(Data.Models.Ts3.GroupChannelToClient assign in ts3.GroupChannelToClient.AsNoTracking())
            {
                ts6.GroupChannelToClients.Add(new Data.Models.Ts6.GroupChannelToClient
                {
                    GroupId = assign.GroupId,
                    ServerId = assign.ServerId,
                    Id1 = assign.Id1,
                    Id2 = assign.Id2,
                });
                await ts6.SaveChangesAsync();
                total++;
            }
        }
        catch (Exception ex) { warn.Add($"Channel group assignments: {ex.Message}"); }

        return total;
    }

    private static async Task<int> MigratePermissionsAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int total = 0;
        foreach(Data.Models.Ts3.PermServerGroup perm in ts3.PermServerGroup.AsNoTracking())
        {
            ts6.PermServerGroups.Add(new Data.Models.Ts6.PermServerGroup
            {
                ServerId = perm.ServerId,
                Id1 = perm.Id1,
                Id2 = perm.Id2,
                PermId = perm.PermId,
                PermValue = perm.PermValue,
                PermNegated = perm.PermNegated,
                PermSkip = perm.PermSkip
            });
            await ts6.SaveChangesAsync();
            total++;
        }

        foreach(Data.Models.Ts3.PermChannel perm in ts3.PermChannel.AsNoTracking())
        {
            ts6.PermChannels.Add(new Data.Models.Ts6.PermChannel
            {
                ServerId = perm.ServerId,
                Id1 = perm.Id1,
                Id2 = perm.Id2,
                PermId = perm.PermId,
                PermValue = perm.PermValue,
                PermNegated = perm.PermNegated,
                PermSkip = perm.PermSkip
            });
            await ts6.SaveChangesAsync();
            total++;
        }

        foreach(Data.Models.Ts3.PermChannelGroup perm in ts3.PermChannelGroups.AsNoTracking())
        {
            ts6.PermChannelGroups.Add(new Data.Models.Ts6.PermChannelGroup
            {
                ServerId = perm.ServerId,
                Id1 = perm.Id1,
                Id2 = perm.Id2,
                PermId = perm.PermId,
                PermValue = perm.PermValue,
                PermNegated = perm.PermNegated,
                PermSkip = perm.PermSkip
            });
            await ts6.SaveChangesAsync();
            total++;
        }

        foreach(Data.Models.Ts3.PermChannelClient perm in ts3.PermChannelClients.AsNoTracking())
        {
            ts6.PermChannelClients.Add(new Data.Models.Ts6.PermChannelClient
            {
                ServerId = perm.ServerId,
                Id1 = perm.Id1,
                Id2 = perm.Id2,
                PermId = perm.PermId,
                PermValue = perm.PermValue,
                PermNegated = perm.PermNegated,
                PermSkip = perm.PermSkip
            });
            await ts6.SaveChangesAsync();
            total++;
        }

        foreach (Data.Models.Ts3.PermClient perm in ts3.PermClient.AsNoTracking())
        {
            ts6.PermClients.Add(new Data.Models.Ts6.PermClient
            {
                ServerId = perm.ServerId,
                Id1 = perm.Id1,
                Id2 = perm.Id2,
                PermId = perm.PermId,
                PermValue = perm.PermValue,
                PermNegated = perm.PermNegated,
                PermSkip = perm.PermSkip
            });
            await ts6.SaveChangesAsync();
            total++;
        }
        
        return total;
    }

    private static async Task<int> MigrateTokensAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int total = 0;
        try
        {
            foreach (Data.Models.Ts3.Token token in ts3.Tokens.AsNoTracking())
            {
                ts6.Tokens.Add(new Data.Models.Ts6.Token
                {
                    ServerId = token.ServerId,
                    TokenKey = token.TokenKey,
                    TokenType = token.TokenType,
                    TokenId1 = token.TokenId1,
                    TokenId2 = token.TokenId2,
                    TokenCreated = token.TokenCreated,
                    TokenDescription = token.TokenDescription,
                    TokenCustomset = token.TokenCustomset,
                    TokenFromClientId = token.TokenFromClientId
                });
                await ts6.SaveChangesAsync();
                total++;
            }
        }
        catch (Exception ex) { warn.Add($"Tokens: {ex.Message}"); }

#if MIGRATE_API
        try
        {
            foreach (Data.Models.Ts3.ApiKey key in ts3.ApiKeys.AsNoTracking())
            {
                ts6.ApiKeys.Add(new Data.Models.Ts6.ApiKey
                {
                    ApiKeyId = key.ApiKeyId,
                    ServerId = key.ServerId,
                    ApiKeyHash = key.ApiKeyHash,
                    ApiKeyOwnerDbid = key.ApiKeyOwnerDbid,
                    ApiKeyScope = key.ApiKeyScope,
                    ApiKeyCreatedAt = key.ApiKeyCreatedAt,
                    ApiKeyExpiresAt = key.ApiKeyExpiresAt,
                });
                await ts6.SaveChangesAsync();
                total++;
            }
        }
        catch (Exception ex) { warn.Add($"API keys: {ex.Message}"); }
#endif

        try
        {
            foreach (Data.Models.Ts3.Revocation rev in ts3.Revocations.AsNoTracking())
            {
                ts6.Revocations.Add(new Data.Models.Ts6.Revocation
                {
                    RevocationKey = rev.RevocationKey,
                    RevocationType = rev.RevocationType,
                    RevocationExpiration = rev.RevocationExpiration
                });
                await ts6.SaveChangesAsync();
                total++;
            }
        }
        catch (Exception ex) { warn.Add($"Revocations: {ex.Message}"); }

        try
        {
            foreach(Data.Models.Ts3.Complain comp in ts3.Complains.AsNoTracking())
            {
                ts6.Complains.Add(new Data.Models.Ts6.Complain
                {
                    ServerId = comp.ServerId,
                    ComplainFromClientId = comp.ComplainFromClientId,
                    ComplainToClientId = comp.ComplainToClientId,
                    ComplainTimestamp = comp.ComplainTimestamp,
                    ComplainMessage = comp.ComplainMessage,
                    ComplainHash = comp.ComplainHash,
                });
            }
        } catch(Exception ex) { warn.Add($"Complains: {ex.Message}"); }

        return total;
    }

    private static async Task<int> MigrateMessagesAsync(Ts3DbContext ts3, Ts6DbContext ts6, List<string> warn)
    {
        int n = 0;
        try
        {
            foreach (Data.Models.Ts3.Message msg in ts3.Messages.AsNoTracking())
            {
                ts6.Messages.Add(new Data.Models.Ts6.Message
                {
                    MessageId = msg.MessageId,
                    ServerId = msg.ServerId,
                    MessageFromClientId = msg.MessageFromClientId,
                    MessageFromClientUid = msg.MessageFromClientUid,
                    MessageToClientId = msg.MessageToClientId,
                    MessageSubject = msg.MessageSubject,
                    MessageMsg = msg.MessageMsg,
                    MessageTimestamp = msg.MessageTimestamp,
                    MessageFlagRead = msg.MessageFlagRead
                });
                await ts6.SaveChangesAsync();
                n++;
            }
        }
        catch (Exception ex) { warn.Add($"Messages: {ex.Message}"); }
        return n;
    }

#endregion

    // ── Helpers ──────────────────────────────────────────────────────────────

    #region Helpers

    private static MigrationResult Fail(string msg) => new(false, msg, null, 0, new MigrationStats());

    private static object Str(SqliteDataReader r, int i) =>
        r.IsDBNull(i) ? DBNull.Value : (object)r.GetString(i);

    private static object Int(SqliteDataReader r, int i)
    {
        if (r.IsDBNull(i)) return DBNull.Value;
        try   { return (object)r.GetInt64(i); }
        catch { }
        try   { return (object)(long)r.GetDouble(i); }
        catch { }
        try
        {
            if (long.TryParse(r.GetString(i), out var v)) return (object)v;
        }
        catch { }
        return DBNull.Value;
    }

    #endregion

}
