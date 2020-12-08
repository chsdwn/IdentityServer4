using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;

namespace AuthServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ICustomUserRepository _userRepo;
        public ResourceOwnerPasswordValidator(ICustomUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var isUser = await _userRepo.ValidateAsync(context.UserName, context.Password);
            if (isUser)
            {
                var user = await _userRepo.FindByEmailAsync(context.UserName);
                // Authorization code grant: code
                // Resource owner credentials grant: pwd
                context.Result = new GrantValidationResult(
                    user.Id.ToString(),
                    OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}