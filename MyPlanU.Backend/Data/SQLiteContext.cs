using SQLite;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data;

public class SQLiteContext
{
    private SQLiteAsyncConnection _database;
    private readonly string _dbPath;

    public SQLiteContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public async Task InitAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);
        
        await _database.CreateTableAsync<Usuario>();
        await _database.CreateTableAsync<Amistad>();
        await _database.CreateTableAsync<PromptyConfig>();
        await _database.CreateTableAsync<Actividad>();
        await _database.CreateTableAsync<Recordatorio>();
        await _database.CreateTableAsync<ActividadCompartida>();
    }

    public SQLiteAsyncConnection Connection => _database;
}
