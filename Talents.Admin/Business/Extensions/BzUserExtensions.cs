using Business.Security;
using Microsoft.AspNet.Identity;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Identity
{
   public static class BzUserExtensions
   {

      public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this BzUser user, ApplicationUserManager manager)
      {
         // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配

         var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);


         // 在此处添加自定义用户声明

         // 分析用户角色对应的操作域
         // 认定登录人的角色，该登录人可以访问的域类型，且在该域类型对应的域ID下的角色
         // role							= 'admin'
         // scopeType/scopeId			= rolename

         var extRoles = manager.GetRoleTypes(user.Id).Result.Where(m => m.ScopeId != 0);

         if (extRoles.Any())
         {
            var dictRole = manager.DB.BzRoleDal.ConditionQuery(null, null, null, null).ToDictionary(m => m.Id);

            foreach (var role in extRoles)
            {
               var scopeClaim = new Claim(role.ScopeType + role.ScopeId.ToString(), dictRole[role.RoleId].Name);
               userIdentity.AddClaim(scopeClaim);
            }
         }


         // 获取用户 Profile，用户和 Profile 必须一一对应，即每创建一个用户必定对应自动生成其 Profile
         // 无法使用 Claim.Properties 保存扩展数据，所以单列数据

         var profile = manager.GetProfile(user.Id).Result;


         // 为 CasLogin 增加

         profile.IsExtLogined = user.IsExtLogined;


         //	获取targetId 

         var dclBase = manager.DB.DeclareBaseDal.PrimaryGet(profile.UserId);
         if (dclBase != null)
         {
            profile.TargetId = dclBase.DeclareTargetPKID;
         }

         // 获取教师角色时的身份

         if (profile.UserType == "Teacher")
         {
            var declare = manager.DB.DeclareBaseDal.PrimaryGet(profile.UserId);
            if (declare != null)
            {
               profile.IsDeclare = true;
               profile.IsMaster = declare.HasTeam;
            }

            var memberCount = manager.DB.TeamMemberDal.ConditionQueryCount(APDBDef.TeamMember.MemberId == profile.UserId);
            profile.IsMember = memberCount > 0;


            // 获取专家身份
            var expertCount = manager.DB.ExpectDal.ConditionQueryCount(APDBDef.Expect.ExpectId == profile.UserId);
            profile.IsExpert = expertCount > 0;

         }

         //获取系统管理员身份
         profile.IsSystemAdmin = profile.UserId == Config.ThisApp.AppRole_Admin_Id;

         //获取学校管理员身份
         profile.IsSchoolAdmin = manager.DB.CompanyAccesserDal.PrimaryGet(profile.CompanyId, profile.UserId) != null;


         foreach (var prop in profile.GetType().GetProperties())
         {
            if (prop.CanWrite)
            {
               var profileClaim = new Claim(BzClaimTypes.UserProfile + prop.Name, Convert.ToString(prop.GetValue(profile)));
               userIdentity.AddClaim(profileClaim);
            }
         }


         return userIdentity;
      }

   }
}
