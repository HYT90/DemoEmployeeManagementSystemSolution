

using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionRepository(AppDbContext context) : IGenericRepositoryInterface<Sanction>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var san = await context.Sanctions.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (san is null) return NotFound();

            context.Sanctions.Remove(san);
            await Commit();
            return Success();
        }

        public async Task<List<Sanction>> GetAll()
        {
            return await context.Sanctions.
                AsNoTracking().
                Include(x => x.SanctionType).
                ToListAsync();
        }

        public async Task<Sanction> GetById(int id)
        {
            return await context.Sanctions.Include(x => x.SanctionType).SingleOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Sanction item)
        {
            await context.Sanctions.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Sanction item)
        {
            var san = await context.Sanctions.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (san is null) return NotFound();
            context.Sanctions.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
    }
}
