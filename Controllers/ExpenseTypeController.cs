using Microsoft.AspNetCore.Mvc;
using wealthify.DTOs.ExpenseType;
using wealthify.Models;
using wealthify.Services.Interfaces;

namespace wealthify.Controllers;

[Route("api/v1/expense-types")]
[ApiController]
public class ExpenseTypeController(IExpenseTypeService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ExpenseTypeDto>>> CreateExpenseType([FromBody] CreateExpenseTypeDto dto, CancellationToken cancellationToken)
    {
        var createdExpenseType = await service.CreateExpenseTypeAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(GetExpenseTypeById),
            new { id = createdExpenseType.Id },
            new ApiResponse<ExpenseTypeDto>
            {
                Data = createdExpenseType,
                Message = "Expense type created successfully"
            }
        );
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ExpenseTypeDto>>>> GetAllExpenseTypes(CancellationToken cancellationToken)
    {
        var expenseTypes = await service.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<List<ExpenseTypeDto>> { Data = expenseTypes });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ExpenseTypeDto?>>> GetExpenseTypeById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var expenseType = await service.GetByIdAsync(id, cancellationToken);
        if (expenseType is null)
        {
            return NotFound(new ApiResponse
            {
                Message = "Expense type not found",
                Errors = ["No expense type exists for id"]
            });
        }

        return Ok(new ApiResponse<ExpenseTypeDto> { Data = expenseType });
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<ExpenseTypeDto>>> UpdateExpenseType([FromBody] UpdateExpenseTypeDto dto, CancellationToken cancellationToken)
    {
        var updatedExpenseType = await service.UpdateExpenseTypeAsync(dto, cancellationToken);
        if (updatedExpenseType is null)
        {
            return NotFound(new ApiResponse
            {
                Message = "Expense type not found",
                Errors = ["No expense type exists for id"]
            });
        }

        return Ok(new ApiResponse<ExpenseTypeDto>
        {
            Data = updatedExpenseType,
            Message = "Expense type updated successfully"
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteExpenseType([FromRoute] int id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteByIdAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiResponse
            {
                Message = "Expense type not found",
                Errors = ["No expense type exists for id"]
            });
        }

        return Ok(new ApiResponse
        {
            Message = "Expense type deleted successfully"
        });
    }
}