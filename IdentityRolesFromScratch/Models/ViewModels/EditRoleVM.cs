using System.ComponentModel.DataAnnotations;

namespace IdentityRolesFromScratch.Models.ViewModels
{
	public class EditRoleVM
	{
		public EditRoleVM()
		{
			Users = new List<string>();
		}

		public string Id { get; set; }

		[Required(ErrorMessage = "Role name is required")]
		public string RoleName { get; set; }

		public List<string> Users { get; set; }
	}
}
