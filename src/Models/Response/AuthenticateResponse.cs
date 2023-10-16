using System.Text.Json.Serialization;

namespace Backend.Models.Response
{
    public class AuthenticateResponse
    {
        public long UserId { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User u, string jwtToken, string refreshToken)
        {
            UserId = u.Id;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}