﻿using indie_hub_server.Models;
using Microsoft.AspNetCore.Identity;

namespace indie_hub_server.Services
{
    public class AuthService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string providedPassword, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}