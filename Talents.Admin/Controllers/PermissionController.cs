using Business;
using Business.Helper;
using Symber.Web.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using TheSite.Models;
using System.Linq;

namespace TheSite.Controllers
{

    public class PermissionController : BaseController
    {

        static APDBDef.BzPermissionTableDef p = APDBDef.BzPermission;
        static APDBDef.BzRolePermissionTableDef bp = APDBDef.BzRolePermission;
        static APDBDef.BzRoleTableDef u = APDBDef.BzRole;

        // GET: Permission/List
        // POST-Ajax: Permission/List

        [Permisson("权限管理")]
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult List(int current, int rowCount, AjaxOrder sort, string searchPhrase, string userType)
        {
            ThrowNotAjax();

            var query = APQuery.select(p.Id, p.Name,
                                                        u.Id.As("RoleId"), u.Name.As("RoleName"),
                                                        bp.RolePermissionId, bp.IsGrant)
                                                     .from(
                                                     p,
                                                     bp.JoinLeft(p.Id == bp.PermissionId),
                                                     u.JoinLeft(u.Id == bp.RoleId))
                                                     .primary(p.Id)
                                                     .skip((current - 1) * rowCount)
                                                     .take(rowCount);


            //过滤条件
            //模糊搜索用户名、实名进行

            searchPhrase = searchPhrase.Trim();
            if (searchPhrase != "")
            {
                query.where_and(p.Name.Match(searchPhrase) | p.Name.Match(searchPhrase));
            }

            userType = userType.Trim();
            if (userType != "全部")
            {
                query.where_and(u.Name == userType);
            }

            //排序条件表达式

            if (sort != null)
            {
                switch (sort.ID)
                {
                    case "permissionName": query.order_by(sort.OrderBy(p.Name)); break;
                    case "roleName": query.order_by(sort.OrderBy(u.Name)); break;
                    case "isGrant": query.order_by(sort.OrderBy(bp.IsGrant)); break; //TODO 以后修改
                }
            }


            //获得查询的总数量

            var total = db.ExecuteSizeOfSelect(query);


            //查询结果集

            var result = query.query(db, r =>
           {
               return new
               {
                   permissionId = p.Id.GetValue(r),
                   roleId = u.Id.GetValue(r, "RoleId"),
                   rpId = bp.RolePermissionId.GetValue(r),
                   permissionName = p.Name.GetValue(r),
                   roleName = u.Name.GetValue(r, "RoleName"),
                   isGrant = bp.IsGrant.GetValue(r) ? "已授予" : "未授予"
               };
           }).ToList();

            return Json(new
            {
                rows = result,
                current,
                rowCount,
                total
            });
        }


        // POST: Permission/GrantRolePermisson

        [HttpPost]
        public ActionResult GrantRolePermisson(long rolePermissonId)
        {
            ThrowNotAjax();

            var success = ChangGrantStatus(rolePermissonId, true);

            
            BzRoleCache.ClearCache();
            BzPermissionCache.ClearCache();
            BzPermissionCache.ClearRolePermissonCache();

            return Json(new
            {
                result = success ? AjaxResults.Success : AjaxResults.Error,
                msg = success ? "成功授予角色权限" : "授权失败"
            });
        }


        // POST: Permission/DenyRolePermisson

        [HttpPost]
        public ActionResult DenyRolePermisson(long rolePermissonId)
        {
            ThrowNotAjax();


            var success = ChangGrantStatus(rolePermissonId, false);

            BzRoleCache.ClearCache();
            BzPermissionCache.ClearCache();
            BzPermissionCache.ClearRolePermissonCache();

            return Json(new
            {
                result = success ? AjaxResults.Success : AjaxResults.Error,
                msg = success ? "成功剥夺角色权限" : "剥夺失败"
            });
        }

        #region [ Helper ]


        private bool ChangGrantStatus(long rolePermissonId, bool isGrant)
        {
            if (rolePermissonId == 0)
                return false;

            var rp = db.BzRolePermissionDal.PrimaryGet(rolePermissonId);
            if (rp == null)
                return false;

            rp.IsGrant = isGrant;
            db.BzRolePermissionDal.Update(rp);

            return true;
        }


        #endregion

    }

}