using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using ts3to6.Data;
using ts3to6.Enums;

namespace ts3to6.Services;

public sealed class TsDbService : IDisposable
{
    private bool _disposed;

    public DbConnection Connection { get; private set; }

    public string? ErrorMessage { get; private set; }
    public DateTime? LoadedAt { get; private set; }
    public DbVersion Version { get; private set; } = DbVersion.Unknown;

    public Ts3DbContext? Ts3DbContext { get; private set; }

    public Task SetErrorAsync(string message) { ErrorMessage = message; return Task.CompletedTask; }

    public async Task<bool> LoadAsync(byte[] data)
    {
        ErrorMessage = null;
        ReleaseResources();

        try
        {
            if (data.Length < 16)
            {
                ErrorMessage = "File is too small to be a valid SQLite database.";
                return false;
            }

            if (System.Text.Encoding.ASCII.GetString(data, 0, 15) != "SQLite format 3")
            {
                ErrorMessage = "This file is not a valid SQLite database (incorrect header).";
                return false;
            }

            DbContextOptions<Ts3DbContext> options = new DbContextOptionsBuilder<Ts3DbContext>()
                .UseSqlite("Data Source=Ts3PureMemory;Mode=Memory;Cache=Shared")
                .Options;

            Ts3DbContext = new Ts3DbContext(options);
            await Ts3DbContext.Database.OpenConnectionAsync();

            DbConnection baseConnection = Ts3DbContext.Database.GetDbConnection();
            if (baseConnection is SqliteConnection sqliteConn)
            {
                string tempFileName = Path.GetRandomFileName();
                await File.WriteAllBytesAsync(tempFileName, data);

                try
                {
                    using (SqliteConnection sourceConn = new SqliteConnection($"Data Source={tempFileName};Pooling=False"))
                    {
                        await sourceConn.OpenAsync();
                        sourceConn.BackupDatabase(sqliteConn);
                        await sourceConn.CloseAsync();
                    }
                }
                finally
                {
                    if (File.Exists(tempFileName))
                        File.Delete(tempFileName);
                }
            }
            else
            {
                throw new InvalidOperationException("The underlying connection is not a SQLite connection.");
            }

            Connection = Ts3DbContext.Database.GetDbConnection();
            DbVersion version = await DetectVersionAsync(Ts3DbContext);
            if (version == DbVersion.Ts6)
            {
                Ts3DbContext.Database.GetDbConnection().Dispose();
                ErrorMessage = "This file is already a TeamSpeak 6 database (tsserver.sqlitedb). " +
                               "Please upload a TeamSpeak 3 database (ts3server.sqlitedb).";
                return false;
            }

            if (version == DbVersion.Unknown)
            {
                Ts3DbContext.Database.GetDbConnection().Dispose();
                ErrorMessage = "This SQLite file does not appear to be a TeamSpeak database " +
                               "(expected tables not found).";
                return false;
            }

            Version = version;
            LoadedAt = DateTime.Now;
            return true;
        }
        catch (Exception ex)
        {
            ReleaseResources();
            ErrorMessage = $"Unexpected error: {ex.Message}";
            return false;
        }
    }

    private static async Task<DbVersion> DetectVersionAsync(Ts3DbContext context)
    {
        try
        {
            bool hasEvents = await context.Database.ExecuteSqlRawAsync(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='events'") > 0;
            return (hasEvents) ? DbVersion.Ts6 : DbVersion.Ts3;
            //if (hasEvents)
            //{
            //    bool hasClientsChannels = await context.Database.ExecuteSqlRawAsync(
            //        "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ('clients','channels','bans')") >= 2;
            //    return hasClientsChannels ? DbVersion.Ts6 : DbVersion.Unknown;
            //}
            //bool hasClientsChannelsProps = await context.Database.ExecuteSqlRawAsync(
            //    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ('clients','channels','server_properties')") >= 2;
            //return hasClientsChannelsProps ? DbVersion.Ts3 : DbVersion.Unknown;
        }
        catch { return DbVersion.Unknown; }
    }

    private void ReleaseResources()
    {
        Ts3DbContext?.Dispose();
        Ts3DbContext = null;
        Connection = null;
        Version = DbVersion.Unknown;
        LoadedAt = null;
    }

    public void Reset() => ReleaseResources();

    public void Dispose()
    {
        if (!_disposed) { ReleaseResources(); _disposed = true; }
    }
}
