using System;
using wealthify.DTOs.User;
using wealthify.Entity;

namespace wealthify.Extensions;

public static class UserExtension
{
    extension(CreateUserDto dto)
    {
        public User ToEntity()
        {
            return new User
            {
                Name = dto.Name,
                Email = dto.Email.ToLower(),
                Password = dto.Password,
                IsFamilyHead = dto.IsFamilyHead,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
