using Microsoft.EntityFrameworkCore;

namespace vesa.SQL.Extensions;
public static class DbContextExtensions
{

    public static async Task<string> DeleteAsync(this DbContext context, string tableName, string schemaName = "dbo")
    {
        string name = string.Format("[{0}].[{1}]", schemaName, tableName);
        string cmd = $"DELETE FROM {name}";
        await context.Database.ExecuteSqlRawAsync(cmd);
        return cmd;
    }
}
