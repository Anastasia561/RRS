﻿namespace RRS.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string role);
}