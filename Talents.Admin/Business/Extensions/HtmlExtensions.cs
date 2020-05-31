using Business;
using Business.XOrg;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web.Mvc
{

	public static class HtmlExtensions
	{

		public static XOrgData GetXorg(this HtmlHelper helper)
			=> helper.ViewContext.RouteData.GetXorg();


		public static BzUserProfile GetUserProfile(this HtmlHelper helper)
			=> helper.ViewContext.HttpContext.GetUserProfile();

      public static EvalPeriod GetEvalPeriod(this HtmlHelper helper)
        => helper.ViewContext.HttpContext.GetEvalPeriod();

      public static DeclarePeriod GetDeclarePeriod(this HtmlHelper helper) 
         => helper.ViewContext.HttpContext.GetDeclarePeriod();


		public static bool IsRole(this HtmlHelper helper, string roleName)
			=> helper.ViewContext.HttpContext.IsRole(roleName);


		public static bool IsInRole(this HtmlHelper helper, params string[] roleNames)
			=> helper.ViewContext.HttpContext.IsInRole(roleNames);


		public static bool IsRoleInScope(this HtmlHelper helper, string scopeType, long scopeId, string roleName)
			=> helper.ViewContext.HttpContext.IsRoleInScope(scopeType, scopeId, roleName);


		public static bool IsRoleInScope(this HtmlHelper helper, string scopeType, long scopeId, params string[] roleNames)
			=> helper.ViewContext.HttpContext.IsRoleInScope(scopeType, scopeId, roleNames);


      public static bool HasPermission(this HtmlHelper helper, string permisson)
          => helper.ViewContext.HttpContext.HasPermission(permisson);


      public static MvcHtmlString CheckBoxListFor<TModel, TProperty, TItem, TValue, TKey>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> listNameExpr, Expression<Func<TModel, IEnumerable<TValue>>> selectedValuesExpr, IEnumerable<TItem> sourceDataExpr, Expression<Func<TItem, TValue>> valueExpr, Expression<Func<TItem, TKey>> textToDisplayExpr, bool autoChangeLine = true, Dictionary<string, object> htmlAttributes = null)
      {
         string checkbuttonStr = string.Empty;
         int count = 1;
         var name = ExpressionHelper.GetExpressionText(listNameExpr);
         ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(listNameExpr, htmlHelper.ViewData);
         foreach (var item in sourceDataExpr)
         {
            TagBuilder checkbutton = new TagBuilder("input");
            checkbutton.Attributes.Add("type", "checkbox");
            checkbutton.Attributes.Add("name", name);
            checkbutton.Attributes.Add("value", valueExpr.Compile()(item).ToString());
            if (selectedValuesExpr.Compile()(htmlHelper.ViewData.Model) != null && selectedValuesExpr.Compile()(htmlHelper.ViewData.Model).Count() > 0)
            {
               if (selectedValuesExpr.Compile()(htmlHelper.ViewData.Model).Contains(valueExpr.Compile()(item)))
               {
                  checkbutton.Attributes.Add("checked", "checked");
               }
            }
            if (count == 1)
            {
               checkbutton.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, modelMetadata));
            }
            TagBuilder label = new TagBuilder("label");
            if (htmlAttributes != null)
            {
               label.MergeAttributes(htmlAttributes);
               label.InnerHtml = checkbutton.ToString() + textToDisplayExpr.Compile()(item).ToString();
            }
            if (count < sourceDataExpr.Count() && autoChangeLine)
            {
               label.InnerHtml += "<br/>";
            }
            count++;
            checkbuttonStr += label.ToString();
         }
         return MvcHtmlString.Create(checkbuttonStr);
      }

	}

}