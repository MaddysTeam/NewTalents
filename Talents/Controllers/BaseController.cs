using Business;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace Talents.Controllers
{
	public class BaseController : Controller
    {
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

	}
}