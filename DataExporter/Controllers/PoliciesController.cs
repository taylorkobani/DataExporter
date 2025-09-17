using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private PolicyService _policyService;

        public PoliciesController(PolicyService policyService) 
        { 
            _policyService = policyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPolicies([FromBody] CreatePolicyDto createPolicyDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var policy = await _policyService.CreatePolicyAsync(createPolicyDto, ct);
            if (policy == null)
                return BadRequest("Could not create policy.");

            return CreatedAtAction(nameof(GetPolicy), new { id = policy.Id }, policy);
        }



        [HttpGet]
        public async Task<IActionResult> GetPolicies(CancellationToken ct)
        {
            var policies = await _policyService.ReadPoliciesAsync(ct);
            return Ok(policies);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadPolicyDto>> GetPolicy(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive integer.");

            var dto = await _policyService.ReadPolicyAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }


        [HttpPost("export")]
        public async Task<IActionResult> ExportData([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct)
        {
            if (endDate < startDate)
                return BadRequest("endDate must be greater than or equal to startDate.");

            var result = await _policyService.ExportPoliciesAsync(startDate, endDate, ct);
            return Ok(result);
        }
    }
}
