using Business;
using System.Web.Routing;

namespace System.Web.Mvc
{

   public class DecalrePeriodAttribute : ActionFilterAttribute
   {
      private APDBDef _db;


      public APDBDef db
      {
         get
         {
            if (_db == null)
               _db = new APDBDef();
            return _db;
         }
         private set
         {
            _db = value;
         }
      }

      public override void OnActionExecuting(ActionExecutingContext filterContext)
      {
         var Period = db.GetCurrentDeclarePeriod();
         if (Period == null || !Period.IsInDeclarePeriod)
         {
            var request = filterContext.HttpContext.Request;
            var formDeclare = request.Form["IsDeclare"];
            var isFormDeclare = !string.IsNullOrEmpty(formDeclare) && formDeclare.IndexOf("true") >= 0;
            var isRequestDeclare = request["IsDeclare"] != null && request["IsDeclare"].IndexOf("True") >= 0;
            if (filterContext.HttpContext.Request.IsAjaxRequest()
               && request.RequestType == "POST")
            {
               if (isFormDeclare || isRequestDeclare)
                  filterContext.Result = new JsonResult()
                  {
                     Data = new
                     {
                        result = AjaxResults.Error,
                        msg = "当前不在填报周期，请联系校管理员!"
                     }
                  };
            }
            else
               throw new ApplicationException("当前不在填报周期");
         }
      }

   }

}

