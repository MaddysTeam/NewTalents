using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Helper
{

    public static class BzPermissionNames
    {

        public static Dictionary<int, string> AdminPermissions { get; private set; } = new Dictionary<int, string>();
        public static Dictionary<int, string> SchoolAdminPermissions { get; private set; } = new Dictionary<int, string>();
        public static Dictionary<int, string> TeacherPermissions { get; private set; } = new Dictionary<int, string>();
        static int index = 1;

        static BzPermissionNames()
        {
            InititalPermissions(AdminPermissions, typeof(Admin));
            InititalPermissions(SchoolAdminPermissions, typeof(SchoolAdmin));
            InititalPermissions(TeacherPermissions, typeof(Teacher));
        }

        public static void Clear()
        {
            AdminPermissions.Clear();
            SchoolAdminPermissions.Clear();
            TeacherPermissions.Clear();
        }

        private static void InititalPermissions(Dictionary<int, string> permissons,Type permissonNames)
        {

            permissonNames.GetFields().ToList().ForEach(field=>
            {
                if (field.FieldType == typeof(string))
                {
                    permissons.Add(index, field.GetValue(null).ToString());

                    index++;
                }
            });
        }

    }


    public static class Admin
    {

        #region  [  管理员权限  ]

        public const string UserVisit = "用户管理.浏览权限";
        public const string UserAdd = "用户管理.新增权限";
        public const string UserOperation = "用户管理.操作权限";
        public const string RoleVisit = "角色管理.浏览权限";
        public const string TeamDeclareVisit = "梯队称号管理.浏览权限";
        public const string TeamDeclareEdit = "梯队称号管理.操作权限";
        public const string TeamManageVisit = "梯队管理.浏览权限";
        public const string TeamManageOperation = "梯队管理.操作权限";
        public const string TeamMemberVisit = "学员管理.浏览权限";
        public const string TeamMemberOperation = "学员管理.操作权限";
        public const string CompanyVisit = "单位管理.浏览权限";
        public const string CompanyMemberVisit = "学员校评单位管理.浏览权限";
        public const string CompanyMemberOperation = "学员校评单位管理.操作权限";
        public const string EvalVisit = "考评指标.浏览权限";
        public const string ExpGroupVisit = "专家组.浏览权限";
        public const string ExpGroupOperation = "专家组.操作权限";
        public const string ExpManageVisit = "专家管理.浏览权限";
        public const string ExpManageOepration = "专家管理.操作权限";
        public const string ExpGroupMemberVisit = "学员质评专家组管理.浏览权限";
        public const string ExpGroupMemberOperation = "学员质评专家组管理.操作权限";
        public const string SchoolEvalCurrentVisit = "校评管理.当期浏览权限";
        public const string SchoolEvalCurrentOperation = "校评管理.当期操作权限";
        public const string SchoolEvalPreviousVisit = "校评管理.往期浏览权限";
        public const string SchoolEvalPreviousOperation = "校评管理.往期操作权限";
        public const string VolumnEvalCurrentVisit = "量评管理.当期浏览权限";
        public const string VolumnEvalCurrentOperation = "量评管理.当期操作权限";
        public const string VolumnEvalPreviousVisit = "量评管理.往期浏览权限";
        public const string VolumnEvalPreviousOperation = "量评管理.往期操作权限";
        public const string QualityEvalCurrentVisit = "质评管理.当期浏览权限";
        public const string QualityEvalCurrentOperation = "质评管理.当期操作权限";
        public const string QualityEvalPreviousVisit = "质评管理.往期浏览权限";
        public const string QualityEvalPreviousOperation = "质评管理.往期操作权限";
        public const string NewsManageVisit = "新闻管理.浏览权限";
        public const string NewsManageOperation = "新闻管理.浏览权限";
        public const string ManPageImageVisit = "首页图片管理.浏览权限";
        public const string ManPageImageOperation = "首页图片管理.操作权限";
        public const string StatisticMemberVisit = "学员统计.浏览权限";
        public const string StatisticSchoolEvalVisit = "校评统计.浏览权限";
        public const string StatisticVolumnEvalVisit = "量评统计.浏览权限";
        public const string StatisticQualityEvalVisit = "质评统计.浏览权限";
        public const string SystemSyncDataVisit = "数据同步.浏览权限";

        #endregion

    }


    public static class SchoolAdmin { 
}


    public static class Teacher { }

}