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

    private async Task<SqlConnection> GetConnectionAsync()
    {
        var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await sqlConnection.OpenAsync();
        return sqlConnection;
    }


    public async Task<int> CreatePasswordEntryAsync(PasswordEntry entry)
    {
        int entryId = 0;

        using (SqlConnection cn = await GetConnectionAsync())
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

    public async Task<bool> DeletePasswordEntryAsync(int entryId)
    {
        var deleted = 0;

        using (SqlConnection cn = await GetConnectionAsync())
        {
            var p = new DynamicParameters(new { Id = entryId });
            p.Add("@Count", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            deleted = await cn.ExecuteAsync("dbo.DeletePasswordEntry", p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            deleted = p.Get<int>("@Count");
        }

        return (deleted == 1);
    }

    public async Task<PasswordEntry> GetPasswordEntryAsync(int entryId)
    {
        var entry = new PasswordEntry();

        using (SqlConnection cn = await GetConnectionAsync())
        {
            var p = new DynamicParameters(new { Id = entryId });

            entry = (await cn.QueryAsync<PasswordEntry>("dbo.GetPasswordEntries", p, commandTimeout: 0, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        return entry;
    }

    public async Task<List<PasswordEntry>> GetPasswordEntriesAsync()
    {
        var entries = new List<PasswordEntry>();
        using (SqlConnection cn = await GetConnectionAsync())
        {
            entries = (await cn.QueryAsync<PasswordEntry>("dbo.GetPasswordEntries", null, commandTimeout: 0, commandType: CommandType.StoredProcedure)).ToList();
        }
            return entries;
    }

    public Task<PasswordEntry> UpdatePasswordEntryAsync(PasswordEntry entry)
    {
        throw new NotImplementedException();
    }
}
