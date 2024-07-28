using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionTypeRepository(AppDbContext context) : IGenericRepositoryInterface<SanctionType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var ot = await context.SanctionTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (ot is null) return NotFound();

            context.SanctionTypes.Remove(ot);
            await Commit();
            return Success();
        }

        public async Task<List<SanctionType>> GetAll()
        {
            return await context.SanctionTypes.
                AsNoTracking().
                ToListAsync();
        }

        public async Task<SanctionType> GetById(int id)
        {
            return await context.SanctionTypes.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GeneralResponse> Insert(SanctionType item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "SanctionType already added");
            await context.SanctionTypes.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(SanctionType item)
        {
            var ot = await context.SanctionTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (ot is null) return NotFound();
            context.SanctionTypes.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.SanctionTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
