using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class TownRepository(AppDbContext context) : IGenericRepositoryInterface<Town>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var town = await context.Towns.FindAsync(id);
            if (town is null) return NotFound();

            context.Towns.Remove(town);
            await Commit();
            return Success();
        }

        public async Task<List<Town>> GetAll()
        {
            return await context.Towns.AsNoTracking().Include(x => x.City).ToListAsync();
        }

        public async Task<Town> GetById(int id)
        {
            return await context.Towns.Include(x => x.City).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GeneralResponse> Insert(Town item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Town already added");
            await context.Towns.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Town item)
        {
            //var town = context.Towns.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);
            var town = await context.Towns.FindAsync(item.Id);
            if (town is null) return NotFound();
            //context.Towns.Attach(item).State = EntityState.Modified;
            town.Name = item.Name;
            town.CityId = item.CityId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Town not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Towns.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
