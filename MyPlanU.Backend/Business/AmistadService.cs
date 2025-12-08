using MyPlanU.Backend.Data;
using MyPlanU.Backend.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPlanU.Backend.Business;

public class AmigoInfo
{
    public int IdAmistad { get; set; }
    public Usuario Usuario { get; set; } = new();
    public string Estado { get; set; } = string.Empty;
}

public class AmistadService
{
    private readonly SQLiteContext _context;

    public AmistadService(SQLiteContext context)
    {
        _context = context;
    }

    private async Task InitAsync()
    {
        await _context.InitAsync();
    }

    public async Task<List<AmigoInfo>> ObtenerAmigosConfirmados(int userId)
    {
        await InitAsync();
        
        // Seleccionamos las amistades donde el usuario participa y están aceptadas
        var query = @"
            SELECT a.IdAmistad, a.EstadoSolicitud as Estado, u.*
            FROM Amistad a
            INNER JOIN Usuario u ON (a.IdUsuarioPrincipal = u.IdUsuario OR a.IdUsuarioAmigo = u.IdUsuario)
            WHERE (a.IdUsuarioPrincipal = ? OR a.IdUsuarioAmigo = ?)
            AND a.EstadoSolicitud = 'Aceptada'
            AND u.IdUsuario != ?";

        // SQLite-net-pcl query mapping is simple, it might not map nested objects automatically.
        // We might need to query Amistad and then fetch Users, or use a flat DTO.
        // Let's try to do it in two steps to be safe and clean with the ORM.
        
        var amistades = await _context.Connection.Table<Amistad>()
            .Where(a => (a.IdUsuarioPrincipal == userId || a.IdUsuarioAmigo == userId) && a.EstadoSolicitud == "Aceptada")
            .ToListAsync();

        var result = new List<AmigoInfo>();
        foreach (var amistad in amistades)
        {
            int friendId = (amistad.IdUsuarioPrincipal == userId) ? amistad.IdUsuarioAmigo : amistad.IdUsuarioPrincipal;
            var friend = await _context.Connection.Table<Usuario>().Where(u => u.IdUsuario == friendId).FirstOrDefaultAsync();
            if (friend != null)
            {
                result.Add(new AmigoInfo { IdAmistad = amistad.IdAmistad, Usuario = friend, Estado = amistad.EstadoSolicitud });
            }
        }
        return result;
    }

    public async Task<List<AmigoInfo>> ObtenerSolicitudesEntrantes(int userId)
    {
        await InitAsync();
        // Solicitudes donde yo soy el Amigo (receptor) y estado es Pendiente
        var amistades = await _context.Connection.Table<Amistad>()
            .Where(a => a.IdUsuarioAmigo == userId && a.EstadoSolicitud == "Pendiente")
            .ToListAsync();

        var result = new List<AmigoInfo>();
        foreach (var amistad in amistades)
        {
            var sender = await _context.Connection.Table<Usuario>().Where(u => u.IdUsuario == amistad.IdUsuarioPrincipal).FirstOrDefaultAsync();
            if (sender != null)
            {
                result.Add(new AmigoInfo { IdAmistad = amistad.IdAmistad, Usuario = sender, Estado = amistad.EstadoSolicitud });
            }
        }
        return result;
    }

    public async Task<List<AmigoInfo>> ObtenerSolicitudesSalientes(int userId)
    {
        await InitAsync();
        // Solicitudes donde yo soy el Principal (emisor) y estado es Pendiente
        var amistades = await _context.Connection.Table<Amistad>()
            .Where(a => a.IdUsuarioPrincipal == userId && a.EstadoSolicitud == "Pendiente")
            .ToListAsync();

        var result = new List<AmigoInfo>();
        foreach (var amistad in amistades)
        {
            var receiver = await _context.Connection.Table<Usuario>().Where(u => u.IdUsuario == amistad.IdUsuarioAmigo).FirstOrDefaultAsync();
            if (receiver != null)
            {
                result.Add(new AmigoInfo { IdAmistad = amistad.IdAmistad, Usuario = receiver, Estado = amistad.EstadoSolicitud });
            }
        }
        return result;
    }

    public async Task<bool> EnviarSolicitud(int senderId, string emailDestino)
    {
        await InitAsync();
        
        var targetUser = await _context.Connection.Table<Usuario>().Where(u => u.Email == emailDestino).FirstOrDefaultAsync();
        if (targetUser == null) return false; // Usuario no encontrado
        if (targetUser.IdUsuario == senderId) return false; // No auto-solicitud

        // Verificar duplicados
        var existing = await _context.Connection.Table<Amistad>()
            .Where(a => (a.IdUsuarioPrincipal == senderId && a.IdUsuarioAmigo == targetUser.IdUsuario) ||
                        (a.IdUsuarioPrincipal == targetUser.IdUsuario && a.IdUsuarioAmigo == senderId))
            .FirstOrDefaultAsync();

        if (existing != null) return false; // Ya existe relación

        var nuevaAmistad = new Amistad
        {
            IdUsuarioPrincipal = senderId,
            IdUsuarioAmigo = targetUser.IdUsuario,
            EstadoSolicitud = "Pendiente",
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _context.Connection.InsertAsync(nuevaAmistad);
        return true;
    }

    public async Task AceptarSolicitud(int amistadId)
    {
        await InitAsync();
        var amistad = await _context.Connection.Table<Amistad>().Where(a => a.IdAmistad == amistadId).FirstOrDefaultAsync();
        if (amistad != null)
        {
            amistad.EstadoSolicitud = "Aceptada";
            amistad.FechaActualizacion = DateTime.UtcNow;
            await _context.Connection.UpdateAsync(amistad);
        }
    }

    public async Task RechazarSolicitud(int amistadId)
    {
        await InitAsync();
        // Rechazar implica eliminar la solicitud pendiente
        var amistad = await _context.Connection.Table<Amistad>().Where(a => a.IdAmistad == amistadId).FirstOrDefaultAsync();
        if (amistad != null)
        {
            await _context.Connection.DeleteAsync(amistad);
        }
    }

    public async Task EliminarAmigo(int amistadId)
    {
        await InitAsync();
        var amistad = await _context.Connection.Table<Amistad>().Where(a => a.IdAmistad == amistadId).FirstOrDefaultAsync();
        if (amistad != null)
        {
            await _context.Connection.DeleteAsync(amistad);
        }
    }
}
