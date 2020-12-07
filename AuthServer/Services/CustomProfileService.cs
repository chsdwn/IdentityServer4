using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace AuthServer.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly ICustomUserRepository _userRepo;

        public CustomProfileService(ICustomUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // /userinfo/ endpoint'ine istek yapıldığında çalışacak method.
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var id = context.Subject.GetSubjectId();
            var user = await _userRepo.FindByIdAsync(Convert.ToInt32(id));

            // access_token ile /userinfo/ endpoint'inden alınacak claimler.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("city", user.City)
            };

            // NOT BEST PRACTICE //
            // Id'si 1 olan kullanıcıya admin rolü claim'i ekle
            if (user.Id.Equals(1))
                claims.Add(new Claim("role", "admin"));
            else
                claims.Add(new Claim("role", "customer"));

            context.AddRequestedClaims(claims);

            // Belirtilen claim'leri direk access_token'ın içine ekler.
            // Yapılmaması gerekir. Token'ı şişirir.
            // Optimize olanı claim'leri /userinfo/ endpoint'inden çekmektir.
            // context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var id = context.Subject.GetSubjectId();
            var user = await _userRepo.FindByIdAsync(Convert.ToInt32(id));

            context.IsActive = user == null ? false : true;
        }
    }
}