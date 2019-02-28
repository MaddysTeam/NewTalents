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
         if (!Period.IsInDeclarePeriod)
         {
            var request = filterContext.HttpContext.Request;
            if (filterContext.HttpContext.Request.IsAjaxRequest() && request.RequestType == "POST")
            {
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
               filterContext.Result = new ContentResult { Content = "当前不在填报周期，请联系校管理员!" };
               //throw new Exception("当前不在填报周期，请联系校管理员!");
         }
      }

   }

}

