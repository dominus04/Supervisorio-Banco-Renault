using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Data.Repositories
{

    public interface IOP10_TraceabilityRepository
    {
        Task<ObservableCollection<OP10_Traceability>> GetAllTraceabilities();
        Task<OP10_Traceability?> GetTraceabilityByRadiatorCode(string radiatorCode);
        Task<bool> AddTraceability(OP10_Traceability traceability);
        Task<bool> UpdateTraceability(OP10_Traceability traceability);
    }

    public class OP10_TraceabilityRepository : IOP10_TraceabilityRepository
    {

        public readonly AppDbContext _context;

        public OP10_TraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<OP10_Traceability>> GetAllTraceabilities()
        {
            return new ObservableCollection<OP10_Traceability>(await _context.OP10_Traceabilities.Include(t => t.User).ToListAsync());
        }

        public async Task<OP10_Traceability?> GetTraceabilityByRadiatorCode(string radiatorCode)
        {
            return await _context.OP10_Traceabilities.FirstOrDefaultAsync<OP10_Traceability>(t => t.RadiatorCode == radiatorCode);
        }

        public async Task<bool> AddTraceability(OP10_Traceability traceability)
        {
            await _context.OP10_Traceabilities.AddAsync(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTraceability(OP10_Traceability traceability)
        {
            _context.OP10_Traceabilities.Update(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
