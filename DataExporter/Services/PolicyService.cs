using DataExporter.Dtos;
using DataExporter.Model;
using Microsoft.EntityFrameworkCore;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto, CancellationToken ct = default)
        {
            var entity = new Policy
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };

            _dbContext.Policies.Add(entity);

            try
            {
                await _dbContext.SaveChangesAsync(ct);

                return new ReadPolicyDto
                {
                    Id = entity.Id,
                    PolicyNumber = entity.PolicyNumber,
                    Premium = entity.Premium,
                    StartDate = entity.StartDate
                };
            }
            catch (DbUpdateException)
            {
                // could log here
                return null;
            }
        }


        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a list of ReadPoliciesDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync(CancellationToken ct = default)
        {
            return await _dbContext.Policies
                .AsNoTracking()
                .Select(p => new ReadPolicyDto
                {
                    Id = p.Id,
                    PolicyNumber = p.PolicyNumber,
                    Premium = p.Premium,
                    StartDate = p.StartDate
                })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ReadPolicyDto.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Policies
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ReadPolicyDto
                {
                    Id = p.Id,
                    PolicyNumber = p.PolicyNumber,
                    Premium = p.Premium,
                    StartDate = p.StartDate
                })
                .SingleOrDefaultAsync(ct);
        }

        /// <summary>
        /// Exports policies over a range
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="ct"></param>
        /// <returns>Policies matching the time interval</returns>
        public async Task<IList<ExportDto>> ExportPoliciesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
        {
            return await _dbContext.Policies
                .AsNoTracking()
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
                .Select(p => new ExportDto
                {
                    PolicyNumber = p.PolicyNumber,
                    Premium = p.Premium,
                    StartDate = p.StartDate,
                    Notes = p.Notes.Select(n => n.Text).ToList()
                })
                .ToListAsync(ct);
        }
    }
}
