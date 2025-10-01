using App.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PermissionApp.Permission
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
    {
        public PermissionAuthorizationHandler()
        {
            
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
        {
            if (context.User == null)
                return;
            
            var permission = context.User.Claims.Where(x => x.Type == Helper.Permission 
                        && x.Value == requirement.Permission
                        && x.Issuer == "Local Authority");
            if (permission.Any()) 
            {
                context.Succeed(requirement);
                return ;
            }
        }
    }
}
