using System.ComponentModel.DataAnnotations;

namespace IdentityRolesFromScratch.Models.ViewModels
{
	public class CreateRoleVM
	{
		[Required]
		public string RoleName { get; set; }
	}
}
