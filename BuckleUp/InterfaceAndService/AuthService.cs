using BuckleUp.DatabaseContext;
using BuckleUp.DTOs;
using BuckleUp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BuckleUp.InterfaceAndService
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITenantProvider _tenantProvider;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext db, ITenantProvider tenantProvider, IConfiguration configuration)
        {
            _db = db;
            _tenantProvider = tenantProvider;
            _configuration = configuration;
        }
        public async Task<string> Login(UserDto request)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    throw new Exception("Wrong password entered");
                }
                if (user == null)
                {
                    throw new Exception("User not found with entered email");
                }
                var token = CreateToken(user, _tenantProvider.GetTenant().Identifier.ToString());

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string CreateToken(User user, string tenantId)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "User"),
                        new Claim("TenantId", tenantId.ToString()) // Adding TenantId claim
                };

            var tokenSecret = _configuration.GetSection("AppSettings:Token").Value!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool VerifyPasswordHash(string password, byte[] passhwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passhwordHash);
            }
        }

        public async Task<User> Register(UserDto request)
        {
            try
            {
                var exists = await _db.Users.AnyAsync(a => a.Email == request.Email);
                if (exists is true)
                {
                    throw new Exception("User with same email id exists");
                }
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var newUser = new User()
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    //Tenant = _tenantProvider.GetTenant(),
                    TenantId = _tenantProvider.GetTenant().Id.ToString(),
                };
                await _db.Users.AddAsync(newUser);
                await _db.SaveChangesAsync();
                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception Caught, " + ex.Message);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
