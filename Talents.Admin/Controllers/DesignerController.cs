using Business;
using Business.Config;
using Business.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TheSite.Controllers
{

	[AllowAnonymous]
	public class DesignerController : BaseController
	{

		// GET: Designer/InitUserRole

		#region [ Init User & Role & Permisson ]


		public async Task<ActionResult> InitUserRole()
		{
			db.BeginTrans();

			try
			{
				var roles = new List<BzRole>
				{
					new BzRole { Id = BzRoleIds.Admin, Name = "Admin", NormalizedName = "管理员" , },
					new BzRole { Id = BzRoleIds.SchoolAdmin, Name = "SchoolAdmin", NormalizedName = "学校管理员" },
					new BzRole { Id = BzRoleIds.Teacher, Name = "Teacher", NormalizedName = "教师"},
				};
				foreach (var item in roles)
				{
					await _initRole(item);
				}


				await _initUser(
					new BzUser
					{
						Id = ThisApp.AppUser_Admin_Id,
						UserName = "admin",
						Actived = true,
					},
					ThisApp.DefaultPassword,
					new BzUserProfile
					{
						UserName = "admin",
						RealName = "系统管理员",
						Birthday = DateTime.Now,
						UserType = "管理员",
					},
					new List<UserScope> {
						new UserScope() { RoleId=ThisApp.AppRole_Admin_Id, ScopeType = "", ScopeId = 0 },
					});


				db.Commit();

				return Content("InitUserRole Success!");
			}
			catch (Exception ex)
			{
				db.Rollback();

				return Content(ex.Message);
			}
		}


		public ActionResult InitPermission()
		{
			var exists = db.BzPermissionDal.ConditionQueryCount(null) > 0;


			db.BeginTrans();

			try
			{
				if (exists)
				{
					db.BzRolePermissionDal.ConditionDelete(null);
					db.BzPermissionDal.ConditionDelete(null);
				}


				var roleId = BzRoleIds.Admin;
				foreach (var item in BzPermissionNames.AdminPermissions)
				{
					db.BzPermissionDal.Insert(new BzPermission { Id = item.Key, Name = item.Value, Status = 1 });
					db.BzRolePermissionDal.Insert(new BzRolePermission { PermissionId = item.Key, RoleId = roleId, IsGrant = true });
				}

				roleId = BzRoleIds.SchoolAdmin;
				foreach (var item in BzPermissionNames.SchoolAdminPermissions)
				{
					db.BzPermissionDal.Insert(new BzPermission { Id = item.Key, Name = item.Value, Status = 1 });
					db.BzRolePermissionDal.Insert(new BzRolePermission { PermissionId = item.Key, RoleId = roleId, IsGrant = true });
				}

				roleId = BzRoleIds.Teacher;
				foreach (var item in BzPermissionNames.TeacherPermissions)
				{
					db.BzPermissionDal.Insert(new BzPermission { Id = item.Key, Name = item.Value, Status = 1 });
					db.BzRolePermissionDal.Insert(new BzRolePermission { PermissionId = item.Key, RoleId = roleId, IsGrant = true });
				}


				db.Commit();

				return Content("Inital Permission Success!");
			}
			catch (Exception ex)
			{
				db.Rollback();

				return Content(ex.Message);
			}
		}


		public ActionResult InitEvalGroup()
		{
			//主持人和带头人是199人，骨干454人

			int gugGroupCount = 20; // 骨干20组
			int daitrCount = 5; //学科和带头人是10组
			int gzszcrCount = 5; //工作室主持人5组

			var dr = APDBDef.DeclareReview;
			var teachers = db.DeclareReviewDal.ConditionQuery(
				   dr.StatusKey == DeclareKeys.ReviewSuccess
				 & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos })
				 & dr.PeriodId==Period.PeriodId
				 , null, null, null);

			var ggjs = teachers.FindAll(x => x.DeclareTargetPKID == DeclareTargetIds.GugJiaos);
			var xldtr = teachers.FindAll(x => x.DeclareTargetPKID == DeclareTargetIds.XuekDaitr);
			var zcr = teachers.FindAll(x => x.DeclareTargetPKID == DeclareTargetIds.GongzsZhucr);
			var groupid = 8000;

			db.BeginTrans();

			try
			{
				for (int i = 1; i < gugGroupCount; i++, groupid++)
				{
					db.ExpGroupDal.Insert(new ExpGroup { GroupId = groupid, Name = $"申报评审骨干专家{i}组", DeclareTargetPKID = 5006, CreateDate = DateTime.Now });
					var targets = ggjs.Skip(30 * (i - 1)).Take(30).ToList();
					foreach (var item in targets)
					{
						db.ExpGroupTargetDal.Insert(new ExpGroupTarget { GroupId = groupid, MemberId = item.TeacherId });
					}
				}

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			for (int i = 0; i < daitrCount; i++)
			{
				db.ExpGroupDal.Insert(new ExpGroup { Name = $"申报评审学科带头人专家组{i}", DeclareTargetPKID = 5006, CreateDate = DateTime.Now });
			}

			for (int i = 0; i < gzszcrCount; i++)
			{
				db.ExpGroupDal.Insert(new ExpGroup { Name = $"申报评审工作室人专家组{i}", DeclareTargetPKID = 5006, CreateDate = DateTime.Now });
			}

			return null;
		}


		public ActionResult InitEvalExpert()
		{
			return null;
		}


		public ActionResult InitEvalMember()
		{
			return null;
		}


		private async Task _initRole(BzRole role)
		{
			await Task.Run(() =>
			{
				if (db.BzRoleDal.PrimaryGet(role.Id) == null)
				{
					db.BzRoleDal.Insert(role);
				}
			});
		}


		private async Task _initUser(BzUser user, string password, BzUserProfile profile, List<UserScope> roles)
		{
			if (db.BzUserDal.PrimaryGet(user.Id) == null)
			{
				user.Email
					= profile.Email
					= user.UserName + ThisApp.DefaultEmailSuffix;

				var result = await UserManager.CreateAsync(user, password);

				if (!result.Succeeded)
				{
					throw new Exception("Create User Error!");
				}

				profile.UserId = user.Id;
				db.BzUserProfileDal.Insert(profile);

				if (roles.Any())
				{
					foreach (var role in roles)
					{
						result = await UserManager.AddToRoleAsync(user.Id, role.RoleId, role.ScopeType, role.ScopeId);
					}
				}
			}
		}


		private class UserScope
		{
			public long RoleId { get; set; }
			public string ScopeType { get; set; }
			public long ScopeId { get; set; }
		}


		#endregion

	}
}