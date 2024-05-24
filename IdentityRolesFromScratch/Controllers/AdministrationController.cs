using IdentityRolesFromScratch.Models;
using IdentityRolesFromScratch.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityRolesFromScratch.Controllers
{
	public class AdministrationController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<AppUser> userManager;

		public AdministrationController(RoleManager<IdentityRole> roleManager,
										UserManager<AppUser> userManager)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
		}


		//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

		// Users

		[HttpGet]
		public IActionResult ListUsers()
		{
			var users = userManager.Users;
			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={id} cannot be found";
				return View("NotFound");
			}

			var userClaims = await userManager.GetClaimsAsync(user);
			var userRoles = await userManager.GetRolesAsync(user);

			var model = new EditUserVM()
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				Address = user.Address,
				Claims = userClaims.Select(c => c.Value).ToList(),
				// claims have both name and value thats why selecting value only
				Roles = userRoles
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(EditUserVM model)
		{
			var user = await userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={model.Id} cannot be found";
				return View("NotFound");
			}
			else
			{
				user.Email = model.Email;
				user.UserName = model.UserName;
				user.Address = model.Address;
				var result = await userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}

		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={id} cannot be found";
				return View("NotFound");
			}
			else
			{
				var result = await userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View("ListUsers");
			}
		}


		[HttpGet]
		//[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRoles(string userId)
		{
			ViewBag.userId = userId;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={userId} cannot be found";
				return View("NotFound");
			}
			var roles = await roleManager.Roles.ToListAsync();

			var model = roles.Select(role => new UserRolesVM
			{
				RoleId = role.Id,
				RoleName = role.Name,
				IsSelected = userManager.IsInRoleAsync(user, role.Name).Result // Blocking call, but it's safe here
			}).ToList();

			return View(model);
		}


		[HttpPost]
		//[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRoles(List<UserRolesVM> model, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={userId} cannot be found";
				return View("NotFound");
			}

			var roles = await userManager.GetRolesAsync(user);
			var result = await userManager.RemoveFromRolesAsync(user, roles);

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove user existing roles");
				return View(model);
			}

			result = await userManager.AddToRolesAsync(user,
				model.Where(x=>x.IsSelected).Select(y=>y.RoleName));

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected roles to user");
				return View(model);
			}
			return RedirectToAction("EditUser", new { Id = userId });

		}


		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

		// Roles

		[HttpGet]
		public IActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole(CreateRoleVM model)
		{
			if (ModelState.IsValid)
			{
				IdentityRole identityRole = new IdentityRole()
				{
					Name = model.RoleName
				};
				IdentityResult result = await roleManager.CreateAsync(identityRole);
				if (result.Succeeded)
				{
					return RedirectToAction("index", "home");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult ListRoles()
		{
			var roles = roleManager.Roles;
			return View(roles);
		}

		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
				return View("NotFound");
			}
			else
			{
				var result = await roleManager.DeleteAsync(role);

				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View("ListRoles");
			}
		}


		[HttpGet]
		public async Task<IActionResult> EditRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
				return View($"NotFound");
			}
			var model = new EditRoleVM
			{
				Id = role.Id,
				RoleName = role.Name!
			};

			var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);

			foreach (var user in usersInRole)
			{
				model.Users.Add(user.UserName!);
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditRole(EditRoleVM model)
		{
			var role = await roleManager.FindByIdAsync(model.Id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found";
				return View($"NotFound");
			}
			else
			{
				role.Name = model.RoleName;
				var result = await roleManager.UpdateAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}


		[HttpGet]
		//[Authorize(Roles = "Admin")]
		// we can set up role for specific action, to give access or restrict it
		//[AllowAnonymous] this will give access to anyone
		public async Task<IActionResult> EditUsersInRole(string roleId)
		{
			ViewBag.roleId = roleId;

			var role = await roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
				return View($"NotFound");
			}

			var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

			var model = new List<UserRoleVM>();
			foreach (var user in userManager.Users)
			{
				var userRoleVM = new UserRoleVM()
				{
					UserId = user.Id,
					UserName = user.UserName!,
					IsSelected = usersInRole.Contains(user)
				};
				model.Add(userRoleVM);
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUsersInRole(List<UserRoleVM> model, string roleId)
		{
			ViewBag.roleId = roleId;
			var role = await roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
				return View($"NotFound");
			}
			for(int i = 0; i< model.Count; i++)
			{
				var user = await userManager.FindByIdAsync(model[i].UserId);
				IdentityResult result = null;

				if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
				{
					result = await userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
				{
					result = await userManager.RemoveFromRoleAsync(user, role.Name);
				}
				else
				{
					continue;
				}

				// we are checking if user is checked and already doesnt exist in roles then we
				// assign roles otherwise if uncheck then we remove roles
				// if not both then we just skip the current user and check next user in list

				if (result.Succeeded)
				{
					if (i < (model.Count - 1))
					{
						continue;
						// here if i is less than total number of model count then we still have
						// records which is nothing but more users so we will continue
					}
					else
					{
						return RedirectToAction("EditRole", new { Id = roleId });
						// if we have already reach the end of loop then we will exit and
						// we will go back to editrole page we began and will send back the id
						// of that role
					}
				}
			}

			return RedirectToAction("EditRole", new { Id = roleId });

		}


	}
}
