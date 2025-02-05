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
    public interface ILabelRepository
    {
        Task<ObservableCollection<Label>> GetAllLabels();
        Task<Label?> GetLabelById(Guid id);
        Task<bool> AddLabel(Label label);
        Task<bool> RemoveLabel(Label label);
        Task<bool> UpdateLabel(Label label);
        Task<bool> VerifyData(Label label);
    }
    public class LabelRepository : ILabelRepository
    {
        public readonly AppDbContext _context;

        public LabelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<Label>> GetAllLabels()
        {
            return new ObservableCollection<Label>(await _context.Labels.ToListAsync());
        }

        public async Task<Label?> GetLabelById(Guid id)
        {
            return await _context.Labels.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddLabel(Label label)
        {
            await VerifyData(label);
            await _context.Labels.AddAsync(label);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveLabel(Label label)
        {
            if (label != null)
            {
                _context.Labels.Remove(label);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateLabel(Label label)
        {
            await VerifyData(label);
            _context.Labels.Update(label);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> VerifyData(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }
            return true;
        }
    }
}
