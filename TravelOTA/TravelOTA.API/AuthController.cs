using Azure.Core;
using Azure.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelOTA.Application;
using TravelOTA.Persistence;
using TravelOTA.Persistence.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TravelDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    public AuthController(TravelDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string username,string email, string password)
    {
        var user = new User
        {
            UserName = username,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password) // نخزن كلمة المرور مشفرة
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User created");
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        command.UserId = 1; // مؤقت للتجربة
        //if (command.Travelers == null || !command.Travelers.Any())
        //    throw new Exception("Travelers list is empty");

        var id = await _mediator.Send(command);

        return Ok(new { BookingId = id });
    }
    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes("ThisIsASuperSecretKey12345678901234567890");

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"], // هنا
    claims: claims,
    expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
