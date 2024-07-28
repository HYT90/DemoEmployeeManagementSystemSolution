using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DoctorRepository(AppDbContext context) : IGenericRepositoryInterface<Doctor>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var doc = await context.Doctors.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (doc is null) return NotFound();

            context.Doctors.Remove(doc);
            await Commit();
            return Success();
        }

        public async Task<List<Doctor>> GetAll()
        {
            return await context.Doctors.
                AsNoTracking().
                ToListAsync();
        }

        public async Task<Doctor> GetById(int id)
        {
            return await context.Doctors.SingleOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Doctor item)
        {
            await context.Doctors.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Doctor item)
        {
            var doc = await context.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == item.EmployeeId);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (doc is null) return NotFound();
            context.Doctors.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Doctor not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
    }
}
