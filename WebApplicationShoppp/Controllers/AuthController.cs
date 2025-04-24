using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationShoppp.Models;
using WebApplicationShoppp.Services;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailInterface _emailSender;

    public AuthController(IConfiguration configuration,UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailInterface emailSender)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.Password != model.ConfirmPassword)
            return BadRequest("Passwords do not match");

        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        
        var otp = new Random().Next(100000, 999999).ToString(); 
        OtpStore.UserOtps[user.Email] = otp;

        await _emailSender.SendOTPEmailAsync(model.Email, otp);

        return Ok("Registration successful, please check your email for OTP.");
    }


    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("User not found.");

       
        if (OtpStore.UserOtps.ContainsKey(model.Email) && OtpStore.UserOtps[model.Email] == model.OTP)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            OtpStore.UserOtps.Remove(model.Email);  
            return Ok("Email verified successfully.");
        }

        return BadRequest("Invalid OTP.");
    }

    private string GenerateOTP()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString(); 
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized("Invalid login attempt.");

        
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return BadRequest("Email not confirmed yet. Please verify your email before logging in.");

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

        if (!result.Succeeded)
            return Unauthorized("Invalid login attempt.");

        var token = GenerateJwtToken(user);

        return Ok(new { token, user.Email });
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims = claims.Append(new Claim(ClaimTypes.Role, role)).ToArray();
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
