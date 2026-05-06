using Microsoft.EntityFrameworkCore;
using TScan.Infrastructure.Data;
using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public class MatterService : IMatterService
{
    private readonly ApplicationDbContext _context;

    public MatterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Matter?> GetMatterAsync(Guid matterId, Guid firmId)
    {
        return await _context.Matters
            .FirstOrDefaultAsync(m => m.Id == matterId && m.FirmId == firmId);
    }

    public async Task<List<Matter>> GetFirmMattersAsync(Guid firmId)
    {
        return await _context.Matters
            .Where(m => m.FirmId == firmId)
            .OrderBy(m => m.MatterName)
            .ToListAsync();
    }

    public async Task<List<Matter>> SearchMattersAsync(Guid firmId, string searchTerm)
    {
        var lowerSearch = searchTerm.ToLower();
        return await _context.Matters
            .Where(m => m.FirmId == firmId &&
                   (m.MatterName.ToLower().Contains(lowerSearch) ||
                    m.ClientName!.ToLower().Contains(lowerSearch) ||
                    m.FileNumber!.ToLower().Contains(lowerSearch) ||
                    m.ClaimNumber!.ToLower().Contains(lowerSearch)))
            .OrderBy(m => m.MatterName)
            .ToListAsync();
    }

    public async Task<Matter> CreateMatterAsync(Matter matter)
    {
        _context.Matters.Add(matter);
        await _context.SaveChangesAsync();
        return matter;
    }

    public async Task UpdateMatterAsync(Matter matter)
    {
        _context.Matters.Update(matter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMatterAsync(Guid matterId, Guid firmId)
    {
        var matter = await GetMatterAsync(matterId, firmId);
        if (matter != null)
        {
            _context.Matters.Remove(matter);
            await _context.SaveChangesAsync();
        }
    }
}
