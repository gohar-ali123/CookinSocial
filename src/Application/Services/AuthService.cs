using Application.Common.Responses;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public async Task<Response> RegisterAsync(UserSignupDto dto)
    {
        // Gives a clean early message; the DB unique index is the real duplicate guarantee.
        var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingEmail is not null)
        {
            return new Response
            {
                Success = false,
                Message = "Email is already in use."
            };
        }

        var existingUsername = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUsername is not null)
        {
            return new Response
            {
                Success = false,
                Message = "Username is already taken."
            };
        }

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.UserName : dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return new Response
            {
                Success = false,
                Message = string.Join(" ", result.Errors.Select(e => e.Description))
            };
        }

        _logger.LogInformation("New user registered: {Email}", dto.Email);

        return new Response
        {
            Success = true,
            Message = "Registration successful."
        };
    }
    public async Task<AuthenticationResponse> LoginAsync(UserLoginDto userLoginDto)
    {
        throw new NotImplementedException();
    }

}
