using Microsoft.AspNetCore.Mvc;
using wealthify.DTOs.FamilyMember;
using wealthify.Models;
using wealthify.Services.Interfaces;

namespace wealthify.Controllers;

[Route("api/v1/family-members")]
[ApiController]
public class FamilyMemberController(IFamilyMemberService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> CreateFamilyMember([FromBody] CreateFamilyMemberDto dto, CancellationToken cancellationToken)
    {
        var createdFamilyMember = await service.CreateFamilyMemberAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(GetFamilyMemberById),
            new { id = createdFamilyMember.Id },
            new ApiResponse<FamilyMemberDto>
            {
                Data = createdFamilyMember,
                Message = "Family member created successfully"
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto?>>> GetFamilyMemberById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var familyMember = await service.GetByIdAsync(id, cancellationToken);
        if (familyMember is null)
        {
            return NotFound(new ApiResponse
            {
                Message = "Family member not found",
                Errors = ["No family member exists for id"]
            });
        }

        return Ok(new ApiResponse<FamilyMemberDto> { Data = familyMember });
    }
}
