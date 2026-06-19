using Dapper;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public class ProfileGroupRepository(IDbConnectionFactory connectionFactory)
    : BaseRepository<ProfileGroup>(connectionFactory), IProfileGroupRepository
{
    protected override string TableName => "ProfileGroups";

    public async Task<Guid> AddAsync(ProfileGroup entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO ProfileGroups (Id, Name, Description, TemplatePath, SortOrder, IsActive, CreatedAt)
            VALUES (@Id, @Name, @Description, @TemplatePath, @SortOrder, @IsActive, @CreatedAt)",
            entity);
        return entity.Id;
    }

    public async Task UpdateAsync(ProfileGroup entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE ProfileGroups SET Name=@Name, Description=@Description,
            TemplatePath=@TemplatePath, SortOrder=@SortOrder, IsActive=@IsActive
            WHERE Id=@Id",
            entity);
    }

    public async Task<IEnumerable<ProfileGroup>> GetActiveGroupsAsync(CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QueryAsync<ProfileGroup>(
            "SELECT * FROM ProfileGroups WHERE IsActive = 1 ORDER BY SortOrder ASC");
    }
}
