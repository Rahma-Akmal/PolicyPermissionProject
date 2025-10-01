using App.Application.ViewModel;
using App.Core.Constants;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace PermissionApp.Controllers
{
    public class PermissionsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(string roleId)
        {
            var role= await _roleManager.FindByIdAsync(roleId);
            var claims=_roleManager.GetClaimsAsync(role).Result.Select(x=>x.Value).ToList();
            var allPermisiions=Permissions.PermissionList().Select(x=>new RoleClaimsViewModel { Value=x}).ToList();
            foreach(var Permission in allPermisiions)
                if(claims.Any(x=>x==Permission.Value))
                    Permission.Selected = true;

            return View( new PermisionsViewModel{
                RoleId = roleId,
                RoleName=role.Name,
                RoleClaims=allPermisiions
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PermisionsViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            var allclaims = await _roleManager.GetClaimsAsync(role);
            foreach(var claim in allclaims)
                await _roleManager.RemoveClaimAsync(role, claim);
            var selectedClaims = model.RoleClaims.Where(x => x.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim(Helper.Permission, claim.Value));
            }
            return RedirectToAction("Index","Account");

        }
    }
}
