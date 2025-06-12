using Microsoft.EntityFrameworkCore;
using NLog;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.Data.Repositories
{

    public interface IRecipeRepository
    {
        Task<ObservableCollection<Recipe>> GetAllRecipes();
        Task<Recipe?> GetRecipeByModuleCode(string moduleCode);
        Task<bool> AddRecipe(Recipe recipe);
        Task<bool> RemoveRecipe(Recipe recipe);
        Task<bool> UpdateRecipe(Recipe recipe);
        Task<bool> VerifyData(Recipe recipe);
    }

    public class RecipeRepository : IRecipeRepository
    {
        public readonly AppDbContext _context;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RecipeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservableCollection<Recipe>> GetAllRecipes()
        {
            return new ObservableCollection<Recipe>(await _context.Recipes.Include(r => r.Label).ToListAsync());
        }

        public async Task<Recipe?> GetRecipeByModuleCode(string moduleCode)
        {
            return await _context.Recipes.FirstOrDefaultAsync(x => x.ModuleCode == moduleCode);
        }

        public async Task<bool> AddRecipe(Recipe recipe)
        {
            await VerifyData(recipe);
            await _context.Recipes.AddAsync(recipe);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRecipe(Recipe recipe)
        {
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateRecipe(Recipe recipe)
        {
            await VerifyData(recipe);
            _context.Recipes.Update(recipe);
            return await _context.SaveChangesAsync() > 0;
        }

        // Gerar Verifydata com erros em português e sem usar chaves nos ifs
        public async Task<bool> VerifyData(Recipe recipe)
        {
            Recipe? recipeByMouleCode = await GetRecipeByModuleCode(recipe.ModuleCode);

            if (recipeByMouleCode != null && recipeByMouleCode.Id != recipe.Id)
                throw new Exception("Já existe uma receita para esse módulo.");
            if (recipe.ModuleCode.Trim() == string.Empty)
                throw new Exception("Código do módulo não pode ser nulo");
            if (recipe.AteqRadiatorProgram <= 0 && recipe.VerifyRadiator == true)
                throw new Exception("Verificação do radiador ativa.\nPrograma do ATEQ não pode ser nulo.");
            if (recipe.AteqCondenserProgram <= 0 && recipe.VerifyCondenser == true)
                throw new Exception("Verificação do condensador ativa.\nPrograma do ATEQ não pode ser nulo.");
            if (recipe.RadiatorPSMinimum == 0 && recipe.VerifyRadiator == true) 
                throw new Exception("Verificação do radiador ativa.\nPressão mínima de radiador não pode ser nula");
            if (recipe.RadiatorPSMaximum == 0 && recipe.VerifyRadiator == true)
                throw new Exception("Verificação do radiador ativa.\nPressão máxima de radiador não pode ser nula");
            if (recipe.CondenserPSMinimum == 0 && recipe.VerifyCondenser == true)
                throw new Exception("Verificação do condensador ativa.\nPressão mínima de condensador não pode ser nula");
            if (recipe.CondenserPSMaximum == 0 && recipe.VerifyCondenser == true)
                throw new Exception("Verificação do condensador ativa.\nPressão máxima de condensador não pode ser nula");

            return true;
        }



    }
}
