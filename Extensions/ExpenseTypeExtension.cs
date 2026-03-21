using wealthify.DTOs.ExpenseType;
using wealthify.Entity;

namespace wealthify.Extensions;

public static class ExpenseTypeExtension
{
    extension(ExpenseType entity)
    {
        public ExpenseTypeDto ToDto()
        {
            return new ExpenseTypeDto
            {
                Id = entity.Id,
                ExpenseTypeName = entity.ExpenseTypeName
            };
        }
    }
}