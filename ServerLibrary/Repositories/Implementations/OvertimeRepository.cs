using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeRepository(AppDbContext context) : IGenericRepositoryInterface<Overtime>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var ot = await context.Overtimes.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (ot is null) return NotFound();

            context.Overtimes.Remove(ot);
            await Commit();
            return Success();
        }

        public async Task<List<Overtime>> GetAll()
        {
            return await context.Overtimes.
                AsNoTracking().
                Include(x => x.OvertimeType).
                ToListAsync();
        }

        public async Task<Overtime> GetById(int id)
        {
            return await context.Overtimes.Include(x => x.OvertimeType).SingleOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Overtime item)
        {
            await context.Overtimes.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Overtime item)
        {
            var ot = await context.Overtimes.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (ot is null) return NotFound();
            context.Overtimes.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
    }
}
