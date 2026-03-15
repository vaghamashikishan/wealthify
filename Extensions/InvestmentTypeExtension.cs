using System;
using wealthify.Entity;

namespace wealthify.Extensions;

public static class InvestmentTypeExtension
{
    extension(InvestmentType entity)
    {
        public InvestmentTypeDto ToDto()
        {
            return new InvestmentTypeDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
