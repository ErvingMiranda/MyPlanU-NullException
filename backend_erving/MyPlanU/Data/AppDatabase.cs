using SQLite;
using MyPlanU.Models;

namespace MyPlanU.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public AppDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitAsync()
    {
        await _db.CreateTableAsync<Usuario>();
        await _db.CreateTableAsync<Amistad>();
        await _db.CreateTableAsync<PromptyConfig>();
        await _db.CreateTableAsync<Actividad>();
        await _db.CreateTableAsync<Recordatorio>();
        await _db.CreateTableAsync<ActividadCompartida>();
    }

    public SQLiteAsyncConnection Connection => _db;
}
