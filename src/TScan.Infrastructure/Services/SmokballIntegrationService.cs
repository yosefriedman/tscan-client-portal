using System.Text.Json;
using TScan.Infrastructure.Data;
using TScan.Web.Models;

namespace TScan.Infrastructure.Services;

public class SmokballIntegrationService : ISmokballIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;

    public SmokballIntegrationService(HttpClient httpClient, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<List<Matter>> SyncMattersFromSmokballAsync(Guid firmId, string tenantId)
    {
        try
        {
            // This is a placeholder implementation
            // In production, replace with actual Smokeball API call
            var matters = new List<Matter>();

            // Example: GET /api/matters?tenantId={tenantId}
            var response = await _httpClient.GetAsync($"/api/matters?tenantId={tenantId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var smokballMatters = JsonSerializer.Deserialize<List<SmokballMatterDto>>(content, options);

                if (smokballMatters != null)
                {
                    foreach (var smokballMatter in smokballMatters)
                    {
                        var existingMatter = await _context.Matters
                            .FirstOrDefaultAsync(m => m.SmokballMatterId == smokballMatter.Id && m.FirmId == firmId);

                        if (existingMatter == null)
                        {
                            var matter = new Matter
                            {
                                FirmId = firmId,
                                SmokballMatterId = smokballMatter.Id,
                                MatterName = smokballMatter.Name,
                                ClientName = smokballMatter.ClientName,
                                FileNumber = smokballMatter.FileNumber,
                                ClaimNumber = smokballMatter.ClaimNumber,
                                CaseCaption = smokballMatter.CaseCaption,
                                ResponsibleAttorney = smokballMatter.ResponsibleAttorney,
                                MatterStatus = smokballMatter.Status,
                                LastSyncedAt = DateTime.UtcNow
                            };
                            _context.Matters.Add(matter);
                            matters.Add(matter);
                        }
                        else
                        {
                            existingMatter.MatterName = smokballMatter.Name;
                            existingMatter.ClientName = smokballMatter.ClientName;
                            existingMatter.MatterStatus = smokballMatter.Status;
                            existingMatter.LastSyncedAt = DateTime.UtcNow;
                            _context.Matters.Update(existingMatter);
                            matters.Add(existingMatter);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return matters;
        }
        catch (Exception ex)
        {
            // Log error
            throw new InvalidOperationException("Failed to sync matters from Smokeball", ex);
        }
    }

    public async Task<Matter?> GetMatterFromSmokballAsync(string matterId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/matters/{matterId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var smokballMatter = JsonSerializer.Deserialize<SmokballMatterDto>(content, options);

                if (smokballMatter != null)
                {
                    return new Matter
                    {
                        SmokballMatterId = smokballMatter.Id,
                        MatterName = smokballMatter.Name,
                        ClientName = smokballMatter.ClientName,
                        FileNumber = smokballMatter.FileNumber
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve matter from Smokeball", ex);
        }
    }

    private class SmokballMatterDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public string? FileNumber { get; set; }
        public string? ClaimNumber { get; set; }
        public string? CaseCaption { get; set; }
        public string? ResponsibleAttorney { get; set; }
        public string? Status { get; set; }
    }
}
