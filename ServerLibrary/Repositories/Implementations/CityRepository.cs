using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository(AppDbContext context) : IGenericRepositoryInterface<City>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var city = await context.Cities.FindAsync(id);
            if (city is null) return NotFound();

            context.Cities.Remove(city);
            await Commit();
            return Success();
        }

        public async Task<List<City>> GetAll()
        {
            return await context.Cities.AsNoTracking().Include(x => x.Country).ToListAsync();
        }

        public async Task<City> GetById(int id)
        {
            return await context.Cities.AsNoTracking().Include(x => x.Country).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GeneralResponse> Insert(City item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "City already added");
            await context.Cities.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(City item)
        {
            //var city = context.Cities.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);
            var city = await context.Cities.FindAsync(item.Id);
            if (city is null) return NotFound();
            //context.Cities.Attach(item).State = EntityState.Modified;
            city.Name = item.Name;
            city.CountryId = item.CountryId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "City not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Cities.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
