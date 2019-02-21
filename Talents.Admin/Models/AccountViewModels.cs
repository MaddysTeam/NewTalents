using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class LoginViewModel
	{

		[Required]
		[Display(Name = "用户名")]
		public string Username { get; set; }


		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }


		[Display(Name = "记住我?")]
		public bool RememberMe { get; set; }

	}


	public class ChgPwdViewModel
	{

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "旧密码")]
		public string OldPassword { get; set; }


		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "新密码")]
		public string NewPassword { get; set; }


		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare("NewPassword", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }

	}

}