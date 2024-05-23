using System.ComponentModel.DataAnnotations;

namespace IdentityRolesFromScratch.Models.ViewModels
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Passowrd is Required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me")]
		public bool RememberMe { get; set; }

	}
}
