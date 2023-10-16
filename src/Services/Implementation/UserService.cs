using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Config;
using Backend.Contexts;
using Backend.Models;
using Backend.Models.Response;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private DataContext _dataContext;
        private SecuritySettings _appSettings;

        public UserService(DataContext context, IOptions<SecuritySettings> appSettings)
        {
            _dataContext = context;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Challenge(string email, string pass)
        {
            User u = GetByEmail(email);
            if (u == null)
                return null;

            if (u.Password != pass)
                return null;

            RefreshToken rt = GenerateRefreshToken();
            u.RefreshTokens.Add(rt);
            _dataContext.Users.Update(u);
            _dataContext.SaveChanges();
            return new AuthenticateResponse(u, GenerateJwtToken(u), rt.Token);
        }

        public User GetByEmail(string email)
        {
            return _dataContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetById(long id)
        {
            return _dataContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public AuthenticateResponse RefreshToken(string token)
        {
            User user = _dataContext.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return null;

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return null;

            RefreshToken newRefreshToken = GenerateRefreshToken();
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _dataContext.Update(user);
            _dataContext.SaveChanges();

            string jwtToken = GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public void Register(string email, string pass)
        {
            User u = GetByEmail(email);
            if (u != null)
                throw new Exception("User already exists");

            u = new User()
            {
                Email = email,
                Password = pass,
                // will set registred date regardless of time zone
                DateRegistred = DateTime.UtcNow,
            };
            _dataContext.Update(u);
            _dataContext.SaveChanges();

        }


        private RefreshToken GenerateRefreshToken()
        {
            Byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };
        }

        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            Byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}