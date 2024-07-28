using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository(AppDbContext context) : IGenericRepositoryInterface<Country>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var country = await context.Countries.FindAsync(id);
            if (country is null) return NotFound();

            context.Countries.Remove(country);
            await Commit();
            return Success();
        }

        public async Task<List<Country>> GetAll()
        {
            return await context.Countries.AsNoTracking().ToListAsync();
        }

        public async Task<Country> GetById(int id)
        {
            return await context.Countries.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(Country item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Country already added");
            await context.Countries.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Country item)
        {
            var country = await context.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var dep = await context.Countries.FindAsync(item.Id);
            if (country is null) return NotFound();
            context.Countries.Attach(item).State = EntityState.Modified;
            //country.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Country not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Countries.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
