using GerenteAcademico.Domain.Entities;
using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Repositories
{
    public class AcademiaRepository : IAcademiaRepository
    {
        private readonly ConfigDbContext _context;

        public AcademiaRepository(ConfigDbContext context)
        {
            _context = context;
        }

        public async Task<List<AcademiaConfig>> GetAllAsync()
        {
            return await _context.Academias.ToListAsync();
        }

        public async Task<AcademiaConfig?> GetByIdAsync(int id)
        {
            return await _context.Academias.FindAsync(id);
        }

        public async Task<AcademiaConfig?> GetByCodigoAsync(string codigo)
        {
            return await _context.Academias.FirstOrDefaultAsync(a => a.Codigo == codigo);
        }

        public async Task AddAsync(AcademiaConfig academia)
        {
            await _context.Academias.AddAsync(academia);
        }

        public async Task UpdateAsync(AcademiaConfig academia)
        {
            _context.Academias.Update(academia);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
