using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(AppDbContext context) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var branch = await context.Branches.FindAsync(id);
            if (branch is null) return NotFound();

            context.Branches.Remove(branch);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll()
        {
            return await context.Branches.AsNoTracking().Include(x => x.Department).ToListAsync();
        }

        public async Task<Branch> GetById(int id)
        {
            return await context.Branches.Include(x => x.Department).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GeneralResponse> Insert(Branch item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Branch already added");
            await context.Branches.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Branch item)
        {
            var branch = await context.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var branch = await context.Branches.FindAsync(item.Id);
            if (branch is null) return NotFound();
            item.Department = null;
            context.Branches.Attach(item).State = EntityState.Modified;
            //branch.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Branch not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Branches.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
