using Dapper;
using GiapTech.BindingDocx.Domain.Entities;
using GiapTech.BindingDocx.Domain.Interfaces;

namespace GiapTech.BindingDocx.Infrastructure.Repositories;

public class TemplateFileRepository(IDbConnectionFactory connectionFactory)
    : BaseRepository<TemplateFile>(connectionFactory), ITemplateFileRepository
{
    protected override string TableName => "TemplateFiles";

    public async Task<Guid> AddAsync(TemplateFile entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO TemplateFiles (Id, GroupId, Name, FilePath, FileType, FileSize, IsActive, CreatedAt)
            VALUES (@Id, @GroupId, @Name, @FilePath, @FileType, @FileSize, @IsActive, @CreatedAt)",
            entity);
        return entity.Id;
    }

    public async Task UpdateAsync(TemplateFile entity, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE TemplateFiles SET Name=@Name, FilePath=@FilePath, FileType=@FileType,
            FileSize=@FileSize, IsActive=@IsActive WHERE Id=@Id",
            entity);
    }

    public async Task<IEnumerable<TemplateFile>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QueryAsync<TemplateFile>(
            "SELECT * FROM TemplateFiles WHERE GroupId = @GroupId ORDER BY CreatedAt ASC",
            new { GroupId = groupId });
    }

    public async Task<IEnumerable<TemplateFile>> GetActiveByGroupIdAsync(Guid groupId, CancellationToken ct = default)
    {
        using var conn = ConnectionFactory.CreateConnection();
        return await conn.QueryAsync<TemplateFile>(
            "SELECT * FROM TemplateFiles WHERE GroupId = @GroupId AND IsActive = 1 ORDER BY CreatedAt ASC",
            new { GroupId = groupId });
    }
}
