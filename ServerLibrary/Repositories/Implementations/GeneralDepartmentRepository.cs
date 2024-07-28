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
    public class GeneralDepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.GeneralDepartments.FindAsync(id);
            if (dep is null) return NotFound();

            context.GeneralDepartments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll()
        {
            return await context.GeneralDepartments.AsNoTracking().ToListAsync();
        }

        public async Task<GeneralDepartment> GetById(int id)
        {
            return await context.GeneralDepartments.SingleOrDefaultAsync(x => x.Id==id);
        }

        public async Task<GeneralResponse> Insert(GeneralDepartment item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Department already added");
            await context.GeneralDepartments.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(GeneralDepartment item)
        {
            //var dep = context.GeneralDepartments.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);
            var dep = await context.GeneralDepartments.FindAsync(item.Id);
            if(dep is null) return NotFound();
            //context.GeneralDepartments.Attach(item).State = EntityState.Modified;
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.GeneralDepartments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
