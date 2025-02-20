using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace Supervisório_Banco_Renault.Data.Repositories
{

    public interface IOP10_TraceabilityRepository
    {
        Task<ObservableCollection<OP10_TraceabilityModel>> GetAllTraceabilities();
        Task<OP10_TraceabilityModel?> GetTraceabilityByRadiatorCode(string radiatorCode);
        Task<bool> AddTraceability(OP10_TraceabilityModel traceability);
        Task<bool> UpdateTraceability(OP10_TraceabilityModel traceability);

        Task<ObservableCollection<OP10_TraceabilityModel>> GetTraceabilityFiltered(string traceFilter, DateTime? initialDate, DateTime? finalDate, int filterOP20Executed);
    }

    public class OP10_TraceabilityRepository : IOP10_TraceabilityRepository
    {

        public readonly AppDbContext _context;

        public OP10_TraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<OP10_TraceabilityModel>> GetAllTraceabilities()
        {
            return new ObservableCollection<OP10_TraceabilityModel>(await _context.OP10_Traceabilities.ToListAsync());
        }

        public async Task<OP10_TraceabilityModel?> GetTraceabilityByRadiatorCode(string radiatorCode)
        {
            return await _context.OP10_Traceabilities.FirstOrDefaultAsync<OP10_TraceabilityModel>(t => t.RadiatorCode == radiatorCode);
        }

        public async Task<bool> AddTraceability(OP10_TraceabilityModel traceability)
        {
            await _context.OP10_Traceabilities.AddAsync(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTraceability(OP10_TraceabilityModel traceability)
        {
            _context.OP10_Traceabilities.Update(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ObservableCollection<OP10_TraceabilityModel>> GetTraceabilityFiltered(string traceFilter, DateTime? initialDate, DateTime? finalDate, int filterOP20Executed)
        {
            var query = _context.OP10_Traceabilities.AsQueryable();

            if (!string.IsNullOrEmpty(traceFilter))
            {
                query = query.Where(t => t.RadiatorCode.ToLower().Contains(traceFilter.Trim().ToLower()));
            }

            if (initialDate.HasValue)
            {
                query = query.Where(t => t.DateTime >= initialDate);
            }

            if (finalDate.HasValue)
            {
                query = query.Where(t => t.DateTime <= finalDate);
            }

            if (filterOP20Executed == 1)
            {
                query = query.Where(t => t.OP20_Executed);
            }
            else if(filterOP20Executed == 2)
            {
                query = query.Where(t => !t.OP20_Executed);
            }

            var result = await query.OrderBy(t => t.DateTime).ToListAsync();

            return new ObservableCollection<OP10_TraceabilityModel>(result);

        }

    }
}
