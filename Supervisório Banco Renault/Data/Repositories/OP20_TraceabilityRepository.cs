using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;

namespace Supervisório_Banco_Renault.Data.Repositories
{
    public interface IOP20_TraceabilityRepository
    {
        Task<ObservableCollection<OP20_Traceability>> GetAllTraceabilities();
        Task<OP20_Traceability?> GetTraceabilityByCode(string radiatorCode);
        Task<bool> AddTraceability(OP20_Traceability traceability);
        Task<bool> UpdateTraceability(OP20_Traceability traceability);
    }

    public class OP20_TraceabilityRepository : IOP20_TraceabilityRepository
    {

        public readonly AppDbContext _context;

        public OP20_TraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<OP20_Traceability>> GetAllTraceabilities()
        {
            return new ObservableCollection<OP20_Traceability>(await _context.OP20_Traceabilities.Include(t => t.User).ToListAsync());
        }

        public async Task<OP20_Traceability?> GetTraceabilityByCode(string traceabilityCode)
        {
            return await _context.OP20_Traceabilities.FirstOrDefaultAsync<OP20_Traceability>(t => t.TraceabilityCode == traceabilityCode);
        }

        public async Task<bool> AddTraceability(OP20_Traceability traceability)
        {
            await _context.OP20_Traceabilities.AddAsync(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTraceability(OP20_Traceability traceability)
        {
            _context.OP20_Traceabilities.Update(traceability);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
