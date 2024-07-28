using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using System.Transactions;

namespace ServerLibrary.Repositories.Implementations
{
    public class EmployeeRepository(AppDbContext context) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await context.Employees.FindAsync(id);
            if (item is null) return NotFound();

            context.Employees.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            return await context.Employees
                .AsNoTracking()
                .Include(x => x.Town)
                .ThenInclude(x => x.City)
                .ThenInclude(x => x.Country)
                .Include(x => x.Branch)
                .ThenInclude(x => x.Department)
                .ThenInclude(x => x.GeneralDepartment)
                .ToListAsync();
        }

        public async Task<Employee> GetById(int id)
        {
            return await context.Employees
                .Include(x => x.Town)
                .ThenInclude(x => x.City)
                .ThenInclude(x => x.Country)
                .Include(x => x.Branch)
                .ThenInclude(x => x.Department)
                .ThenInclude(x => x.GeneralDepartment)
                .SingleOrDefaultAsync(x => x.Id == id)!;
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Department already added");
            await context.Employees.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            var emp = await context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var emp = await context.Employees.FindAsync(item.Id);
            if (emp is null) return NotFound();
            item.Town = null;
            item.Branch = null;
            context.Employees.Attach(item).State = EntityState.Modified;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Employee not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Employees.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
