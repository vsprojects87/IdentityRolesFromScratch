using System.ComponentModel.DataAnnotations;

namespace IdentityRolesFromScratch.Models.ViewModels
{
	public class RegisterVM
	{
		[Required(ErrorMessage = "Username is required")]
		public string Name { get; set; }

		public string Email { get; set; }

		[Required(ErrorMessage = "Passowrd is Required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "Password don't match")]
		public string ConfirmPassword { get; set; }

		public string Address { get; set; }
	}
}
