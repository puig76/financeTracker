using System;
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Models;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.ApiService.Mapping;

public static class UserMapper
{
    public static User ToEntity(this UserDTO userModel)
    {
        if (userModel == null) throw new ArgumentNullException(nameof(userModel));

        return new User
        {
            Id = userModel.Id,
            Username = userModel.Username,
            Email = userModel.Email,
            CreatedAt = userModel.CreatedAt
        };
    }

    public static User ToEntity(this UserRegisterDTO userModel)
    {
        if (userModel == null) throw new ArgumentNullException(nameof(userModel));

        User user = new();
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, userModel.Password);

        user.PasswordHash = hashedPassword;
        user.Username = userModel.Username;
        user.Email = userModel.Email;

        return user;
    }
    
    public static UserDTO ToDTO(this User userEntity)
    {
        if (userEntity == null) throw new ArgumentNullException(nameof(userEntity));

        return new UserDTO
        {
            Id = userEntity.Id,
            Username = userEntity.Username,
            Email = userEntity.Email,
            CreatedAt = userEntity.CreatedAt
        };
    }
}
