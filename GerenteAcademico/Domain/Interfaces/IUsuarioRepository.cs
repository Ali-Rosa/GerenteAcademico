using GerenteAcademico.Domain.Entities;

namespace GerenteAcademico.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsernameAsync(string username);
        Task UpdateAsync(Usuario usuario);
    }
}

