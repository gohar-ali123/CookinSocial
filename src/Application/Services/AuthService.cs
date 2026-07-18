using Application.Common.Responses;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    JwtSettings jwtSettings,
    ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
    public async Task<AuthenticationResponse> LoginAsync(UserLoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        // SignInManager (not UserManager) so sign-in policies like RequireConfirmedEmail apply.
        // lockoutOnFailure: false — lockout policy not configured yet.
        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, dto.Password, lockoutOnFailure: false);

        if (signInResult.IsNotAllowed)
        {
            // Unreachable until RequireConfirmedEmail is turned on.
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Email is not confirmed. Please confirm your email before logging in."
            };
        }

        if (!signInResult.Succeeded)
        {
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        _logger.LogInformation("User logged in: {Email}", dto.Email);

        var (token, expiresAt) = GenerateJwtToken(user);

        return new AuthenticationResponse
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    private (string Token, DateTime ExpiresAt) GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

