using Business;
using System.Web.Mvc;

namespace TheSite.Controllers
{

	public class RoleController : BaseController
	{

		static APDBDef.BzRoleTableDef u = APDBDef.BzRole;


		// GET: Role/List

		public ActionResult List()
		{
			var list = db.BzRoleDal.ConditionQuery(null, null, null, null);

			return View(list);
		}

	}

}