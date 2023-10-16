using Backend.Models;
using Backend.Models.Response;

namespace Backend.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Authenticates user and returns a jwt token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        AuthenticateResponse Challenge(string email, string pass);

        AuthenticateResponse Register(string email, string pass);

        // Utils to get user by id or email
        User GetById(long id);
        User GetByEmail(string email);
        // --------------------------------
        
        AuthenticateResponse RefreshToken(string token);
    }
}