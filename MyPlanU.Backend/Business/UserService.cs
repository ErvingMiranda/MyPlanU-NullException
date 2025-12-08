using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPlanU.Backend.Business;

public class UserService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UserService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<Usuario>> SearchUsuariosAsync(string searchText, int currentUserId)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return new List<Usuario>();

        return await _usuarioRepository.SearchUsuariosAsync(searchText, currentUserId);
    }
}
