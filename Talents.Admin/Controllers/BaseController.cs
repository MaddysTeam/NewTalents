using Business;
using Business.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Symber.Web.Data;
using System;
using System.Web;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	[Authorize]
	public class BaseController : Controller
	{

		#region [ DB ]


		private APDBDef _db;


		public APDBDef db
		{
			get
			{
				if (_db == null)
					_db = HttpContext.GetOwinContext().Get<APDBDef>();
				return _db;
			}
			private set
			{
				_db = value;
			}
		}


		#endregion


		#region [ UserManager ]


		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;


		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}


		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}


		protected IAuthenticationManager AuthenticationManager
			=> HttpContext.GetOwinContext().Authentication;


		#endregion


		#region [ Ajax ]


		protected void ThrowNotAjax()
		{
			if (!Request.IsAjaxRequest())
				throw new NotSupportedException("Action must be Ajax call.");
		}


		#endregion


		#region [ Profile ]


		public BzUserProfile UserProfile
			=> HttpContext.GetUserProfile();

		public object AjaxResult { get; private set; }


		#endregion

	}

}