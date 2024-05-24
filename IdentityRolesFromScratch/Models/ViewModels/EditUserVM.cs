using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace IdentityRolesFromScratch.Models.ViewModels
{
	public class EditUserVM
	{
        public EditUserVM()
        {
			Claims = new List<string>();
			Roles = new List<string>();
		}
		public string Id { get; set; }
		[Required]
		public string UserName { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public string Address { get; set; }
		public List<string> Claims { get; set; }
		public IList<string> Roles { get; set; }
	}
}
