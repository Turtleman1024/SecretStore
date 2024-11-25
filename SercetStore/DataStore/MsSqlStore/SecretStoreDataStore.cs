using Dapper;
using Microsoft.Data.SqlClient;
using SecretStore.DataStore.Interface;
using SecretStore.Models;
using System.Data;

namespace SecretStore.DataStore.MsSqlStore;

public class SecretStoreDataStore : ISecretStoreDataStore
{
    private readonly IConfiguration _configuration;

    public SecretStoreDataStore(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private async Task<SqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await sqlConnection.OpenAsync(cancellationToken);
        return sqlConnection;
    }


    public async Task<int> CreatePasswordEntryAsync(PasswordEntry entry, CancellationToken cancellationToken = default)
    {
        int entryId = 0;

        using (SqlConnection cn = await GetConnectionAsync(cancellationToken))
        {
            var p = new DynamicParameters(new
            {
                entry.Website,
                entry.Username,
                entry.Password
            });

            p.Add("@EntityId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            await cn.QueryAsync<int>("dbo.CreatePasswordEntry", p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            entryId = p.Get<int>("@EntityId");
        }

        return entryId;
    }

    public async Task<bool> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var deleted = 0;

        using (SqlConnection cn = await GetConnectionAsync(cancellationToken))
        {
            var p = new DynamicParameters(new { Id = entryId });
            p.Add("@Count", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            deleted = await cn.ExecuteAsync("dbo.DeletePasswordEntry", p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            deleted = p.Get<int>("@Count");
        }

        return (deleted == 1);
    }

    public async Task<PasswordEntry?> GetPasswordEntryAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = new PasswordEntry();

        using (SqlConnection cn = await GetConnectionAsync(cancellationToken))
        {
            var p = new DynamicParameters(new { Id = entryId });

            entry = (await cn.QueryAsync<PasswordEntry>("dbo.GetPasswordEntries", p, commandTimeout: 0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        return entry;
    }

    public async Task<List<PasswordEntry>> GetPasswordEntriesAsync(CancellationToken cancellationToken = default)
    {
        var entries = new List<PasswordEntry>();
        using (SqlConnection cn = await GetConnectionAsync(cancellationToken))
        {
            entries = (await cn.QueryAsync<PasswordEntry>("dbo.GetPasswordEntries", null, commandTimeout: 0, commandType: CommandType.StoredProcedure)).ToList();
        }
            return entries;
    }

    public async Task UpdatePasswordEntryAsync(PasswordEntry entry, CancellationToken cancellationToken = default)
    {
        using (SqlConnection cn = await GetConnectionAsync(cancellationToken))
        {
            var p = new DynamicParameters(new
            {
                entry.Id,
                entry.Website,
                entry.Username,
                entry.Password
            });

            await cn.ExecuteAsync("UpdatePasswordEntry", p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        }
    }
}
