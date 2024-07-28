using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<Department>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Departments.FindAsync(id);
            if (dep is null) return NotFound();

            context.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll()
        {
            return await context.Departments.
                AsNoTracking().
                Include(x => x.GeneralDepartment).
                ToListAsync();
        }

        public async Task<Department> GetById(int id)
        {
            return await context.Departments.Include(x => x.GeneralDepartment).SingleOrDefaultAsync(x => x.Id == id); ;
        }

        public async Task<GeneralResponse> Insert(Department item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Department already added");
            await context.Departments.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await context.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (dep is null) return NotFound();
            context.Departments.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
