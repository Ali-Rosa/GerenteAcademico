using GerenteAcademico.Domain.Entities;

namespace GerenteAcademico.Domain.Interfaces
{
    public interface IAcademiaRepository
    {
        Task<List<AcademiaConfig>> GetAllAsync();
        Task<AcademiaConfig?> GetByIdAsync(int id);
        Task<AcademiaConfig?> GetByCodigoAsync(string codigo);
        Task AddAsync(AcademiaConfig academia);
        Task UpdateAsync(AcademiaConfig academia);
        Task SaveChangesAsync();
    }
}

