using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Business;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario?> LoginAsync(string email, string password)
    {
        var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email);
        if (usuario != null && usuario.ContrasenaHash == password) // In real app, hash check
        {
            return usuario;
        }
        return null;
    }

    public async Task<bool> RegisterAsync(Usuario usuario)
    {
        var existing = await _usuarioRepository.GetUsuarioByEmailAsync(usuario.Email);
        if (existing != null)
            return false;
        
        await _usuarioRepository.SaveUsuarioAsync(usuario);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var usuario = await _usuarioRepository.GetUsuarioAsync(userId);
        if (usuario == null) return false;

        if (usuario.ContrasenaHash != currentPassword) return false;

        usuario.ContrasenaHash = newPassword;
        await _usuarioRepository.SaveUsuarioAsync(usuario);
        return true;
    }
}
