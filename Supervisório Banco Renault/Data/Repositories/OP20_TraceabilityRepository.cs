using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.Data.Repositories
{
    public interface IOP20_TraceabilityRepository
    {
        Task<ObservableCollection<OP20_TraceabilityModel>> GetAllTraceabilities();
        Task<OP20_TraceabilityModel?> GetTraceabilityByCode(string radiatorCode);
        Task<bool> AddTraceability(OP20_TraceabilityModel traceability);
        Task<bool> UpdateTraceability(OP20_TraceabilityModel traceability);
        Task<ObservableCollection<OP20_TraceabilityModel>> GetTraceabilityFiltered(string traceabilityFilter, string radiatorFilter, DateTime? initialDate, DateTime? finalDate);
    }

    public class OP20_TraceabilityRepository : IOP20_TraceabilityRepository
    {

        public readonly AppDbContext _context;

        public OP20_TraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<OP20_TraceabilityModel>> GetAllTraceabilities()
        {
            return new ObservableCollection<OP20_TraceabilityModel>(await _context.OP20_Traceabilities.ToListAsync());
        }

        public async Task<OP20_TraceabilityModel?> GetTraceabilityByCode(string traceabilityCode)
        {
            return await _context.OP20_Traceabilities.FirstOrDefaultAsync<OP20_TraceabilityModel>(t => t.TraceabilityCode == traceabilityCode);
        }

        public async Task<bool> AddTraceability(OP20_TraceabilityModel traceability)
        {
            try
            {
                await _context.OP20_Traceabilities.AddAsync(traceability);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTraceability(OP20_TraceabilityModel traceability)
        {
            _context.OP20_Traceabilities.Update(traceability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ObservableCollection<OP20_TraceabilityModel>> GetTraceabilityFiltered(string traceabilityFilter, string radiatorFilter, DateTime? initialDate, DateTime? finalDate)
        {
            var query = _context.OP20_Traceabilities.AsQueryable();

            if (!string.IsNullOrEmpty(traceabilityFilter))
            {
                query = query.Where(t => t.TraceabilityCode.ToLower().Contains(traceabilityFilter.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(radiatorFilter))
            {
                query = query.Where(t => t.RadiatorCode.ToLower().Contains(radiatorFilter.Trim().ToLower()));
            }

            if (initialDate.HasValue)
            {
                query = query.Where(t => t.DateTimeOP20 >= initialDate);
            }

            if (finalDate.HasValue)
            {
                query = query.Where(t => t.DateTimeOP20 <= finalDate);
            }

            var result = await query.OrderByDescending(t => t.DateTimeOP20).ToListAsync();

            return new ObservableCollection<OP20_TraceabilityModel>(result);

        }
    }
}
