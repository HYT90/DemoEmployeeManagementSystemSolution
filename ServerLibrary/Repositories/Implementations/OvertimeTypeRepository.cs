using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeTypeRepository(AppDbContext context) : IGenericRepositoryInterface<OvertimeType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var ot = await context.OvertimeTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (ot is null) return NotFound();

            context.OvertimeTypes.Remove(ot);
            await Commit();
            return Success();
        }

        public async Task<List<OvertimeType>> GetAll()
        {
            return await context.OvertimeTypes.
                AsNoTracking().
                ToListAsync();
        }

        public async Task<OvertimeType> GetById(int id)
        {
            return await context.OvertimeTypes.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GeneralResponse> Insert(OvertimeType item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "OvertimeType already added");
            await context.OvertimeTypes.AddAsync(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(OvertimeType item)
        {
            var ot = await context.OvertimeTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            //var dep = await context.Departments.FindAsync(item.Id);
            if (ot is null) return NotFound();
            context.OvertimeTypes.Attach(item).State = EntityState.Modified;
            //dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.OvertimeTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
