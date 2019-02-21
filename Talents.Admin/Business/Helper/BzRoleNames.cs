namespace Business.Helper
{

	public static class BzRoleNames
	{

		public const string Admin = "Admin";
		public const string SchoolAdmin = "SchoolAdmin";
		public const string Teacher = "Teacher";


		public static string[] UserType = new string[]
		{
			"全部",
			Admin,
			SchoolAdmin,
			Teacher
		};

	}

}