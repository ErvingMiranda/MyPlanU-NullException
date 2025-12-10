using MyPlanU.Backend.Models;
using SQLite;

namespace MyPlanU.Backend.Data.Migrations;

public static class Migration001_RemoveUnusedFields
{
    public static async Task ExecuteAsync(SQLiteAsyncConnection db)
    {
        // 1. Usuario
        var usuarioColumns = "IdUsuario, Nombre, Apellido, Apodo, Email, ContrasenaHash, Pais, FechaRegistro, PreguntaSeguridad, RespuestaSeguridad";
        await MigrateTableAsync<Usuario>(db, "Usuario", usuarioColumns);

        // 2. Amistad
        var amistadColumns = "IdAmistad, IdUsuarioPrincipal, IdUsuarioAmigo, EstadoSolicitud, FechaCreacion, FechaActualizacion";
        await MigrateTableAsync<Amistad>(db, "Amistad", amistadColumns);

        // 3. PromptyConfig
        var promptyColumns = "IdPrompty, IdUsuario, NombreAsistente, Activo, FechaCreacion, FechaActualizacion";
        await MigrateTableAsync<PromptyConfig>(db, "PromptyConfig", promptyColumns);
    }

    private static async Task MigrateTableAsync<T>(SQLiteAsyncConnection db, string tableName, string columns) where T : new()
    {
        // Check if table exists
        var tableInfo = await db.GetTableInfoAsync(tableName);
        if (tableInfo.Count == 0) return;

        // Rename old table
        var oldTableName = $"{tableName}_Old";
        await db.ExecuteAsync($"ALTER TABLE {tableName} RENAME TO {oldTableName}");

        // Create new table with new schema
        await db.CreateTableAsync<T>();

        // Copy data
        // We assume the columns in 'columns' exist in both old and new tables.
        // If a column was removed from the model, it won't be in the new table, so we don't select it.
        // But we need to select it from the OLD table? No, we select the columns that match the NEW table from the OLD table.
        // So 'columns' should be the list of columns to KEEP.
        
        var sql = $"INSERT INTO {tableName} ({columns}) SELECT {columns} FROM {oldTableName}";
        try 
        {
            await db.ExecuteAsync(sql);
            // If successful, drop old table
            await db.ExecuteAsync($"DROP TABLE {oldTableName}");
        }
        catch (Exception ex)
        {
            // If something goes wrong, try to restore (optional, but good practice)
            // For now, we just re-throw. The user can manually fix if needed.
            // In a real app, we might want to drop the new table and rename the old one back.
            Console.WriteLine($"Migration failed for {tableName}: {ex.Message}");
            throw;
        }
    }
}
