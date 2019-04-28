using Business.Config;

namespace Business.Helper
{

	public static class BzRoleIds
	{

		public readonly static long Admin = ThisApp.AppRole_Admin_Id;
		public readonly static long SchoolAdmin = ThisApp.AppRole_Admin_Id + 1;
		public readonly static long Teacher = ThisApp.AppRole_Admin_Id + 2;
      public readonly static long SpecialExpert = ThisApp.AppRole_Admin_Id + 3;

	}

}