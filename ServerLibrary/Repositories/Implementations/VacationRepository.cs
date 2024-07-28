using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationRepository(AppDbContext context) : IGenericRepositoryInterface<Vacation>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var vac = await context.Vacations.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (vac is null) return NotFound();

            context.Vacations.Remove(vac);
            await Commit();
            return Success();
        }

        public async Task<List<Vacation>> GetAll()
        {
            return await context.Vacations.
                AsNoTracking().
                Include(x => x.VacationType).
                ToListAsync();
        }

        public async Task<Vacation> GetById(int id)
        {
            return await context.Vacations.Include(x => x.VacationType).SingleOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Vacation item)
        {
            await context.Vacations.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Vacation item)
        {
            var vac = await context.Vacations.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (vac is null) return NotFound();
            context.Vacations.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
    }
}
