using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wealthify.DTOs.InvestmentType;
using wealthify.Exceptions;
using wealthify.Models;
using wealthify.Services.Interfaces;

namespace wealthify.Controllers;

[Route("api/v1/investment-types")]
[ApiController]
public class InvestmentTypeController(IInvestmentTypeService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvestmentTypeDto>>> CreateInvestmentType([FromBody] CreateInvestmentTypeDto dto, CancellationToken cancellationToken)
    {
        var createdInvestmentType = await service.CreateInvestmentTypeAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(GetInvestmentTypeById),
            new { id = createdInvestmentType.Id },
            new ApiResponse<InvestmentTypeDto>
            {
                Data = createdInvestmentType,
                Message = "Investment type created successfully"
            }
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<InvestmentTypeDto?>>> GetInvestmentTypeById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var investmentType = await service.GetByIdAsync(id, cancellationToken);
        if (investmentType is null)
        {
            return NotFound(new ApiResponse
            {
                Message = "Investment type not found",
                Errors = ["No investment type exists for id"]
            });
        }

        return Ok(new ApiResponse<InvestmentTypeDto> { Data = investmentType });
    }
}

