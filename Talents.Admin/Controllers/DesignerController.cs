using Business;
using Business.Config;
using Business.Helper;
using Symber.Web.Data;
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


		public Task<ActionResult> InitDeclareEvalGroup()
		{
			//var dr = APDBDef.DeclareReview;
			//var df = APDBDef.DeclareProfile;

			//var result = APQuery.select(dr.Asterisk, df.EduStagePKID)
			//	.from(dr, df.JoinInner(df.UserId == dr.TeacherId))
			//	.where(
			//	 dr.StatusKey == DeclareKeys.ReviewSuccess
			//	 & dr.DeclareTargetPKID == df.DeclareTargetPKID
			//	 & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos })
			//	 & dr.PeriodId == Period.PeriodId)
			//	.query(db, r =>
			//	{
			//		var review = new DeclareReview();
			//		dr.Fullup(r, review, false);
			//		review.StageId = df.EduStagePKID.GetValue(r);
			//		return review;
			//	}).ToList();


			//db.BeginTrans();

			//try
			//{
			//	//5005,5004
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5241 }, null, new string[] { "tdps0101", "tdps0102", "tdps0103" }, new long[] { 1 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5242 }, null, new string[] { "tdps0201", "tdps0202", "tdps0203" }, new long[] { 2 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5243, 5307 }, null, new string[] { "tdps0301", "tdps0302", "tdps0303" }, new long[] { 3 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5247, 5248, 5305 }, null, new string[] { "tdps0401", "tdps0402", "tdps0403" }, new long[] { 4 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5244, 5312, 5245, 5246 }, null, new string[] { "tdps0501", "tdps0502", "tdps0503" }, new long[] { 5 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5251, 5309, 5253, 5252 }, null, new string[] { "tdps0601", "tdps0602", "tdps0603" }, new long[] { 6 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5254, 5256, 5255, 5306 }, null, new string[] { "tdps0701", "tdps0702", "tdps0703" }, new long[] { 7 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5249, 5250, 5303 }, null, new string[] { "tdps0801", "tdps0802", "tdps0803" }, new long[] { 8 });
			//	await CreateGroupAndAssign(result, new long[] { 5004, 5005 }, new long[] { 5300, 5301, 5302, 5308 }, null, new string[] { "tdps0901", "tdps0902", "tdps0903" }, new long[] { 9 });

			//	////5006
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5241 }, new long[] { 1604 }, new string[] { "tdps1001", "tdps1002", "tdps1003" }, new long[] { 10 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5241 }, new long[] { 1603 }, new string[] { "tdps1101", "tdps1102", "tdps1103" }, new long[] { 11 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5241 }, new long[] { 1602 }, new string[] { "tdps1201", "tdps1202", "tdps1203" }, new long[] { 12 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5242 }, new long[] { 1604 }, new string[] { "tdps1301", "tdps1302", "tdps1303" }, new long[] { 13 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5242 }, new long[] { 1603 }, new string[] { "tdps1401", "tdps1402", "tdps1403" }, new long[] { 14 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5242 }, new long[] { 1602 }, new string[] { "tdps1501", "tdps1502", "tdps1503" }, new long[] { 15 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5243, 5307 }, new long[] { 1604 }, new string[] { "tdps1601", "tdps1602", "tdps1603" }, new long[] { 16 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5243, 5307 }, new long[] { 1603 }, new string[] { "tdps1701", "tdps1702", "tdps1703" }, new long[] { 17 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5243, 5307 }, new long[] { 1602 }, new string[] { "tdps1801", "tdps1802", "tdps1803" }, new long[] { 18 });
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5247 }, null, new string[] { "tdps1901", "tdps1902", "tdps1903" }, new long[] { 19 }); //ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5248 }, null, new string[] { "tdps2001", "tdps2002", "tdps2003" }, new long[] { 20 }); //ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5245, 5246 }, null, new string[] { "tdps2101", "tdps2102", "tdps2103" }, new long[] { 21 });//ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5244, 5312 }, null, new string[] { "tdps2201", "tdps2202", "tdps2203" }, new long[] { 22 });//ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5305, 5306, 5256 }, null, new string[] { "tdps2301", "tdps2302", "tdps2303" }, new long[] { 23 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5251, 5253, 5309 }, null, new string[] { "tdps2401", "tdps2402", "tdps2403" }, new long[] { 24 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5252 }, null, new string[] { "tdps2501", "tdps2502", "tdps2503" }, new long[] { 25 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5254, 5304 }, null, new string[] { "tdps2601", "tdps2602", "tdps2603" }, new long[] { 26 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5255 }, null, new string[] { "tdps2701", "tdps2702", "tdps2703" }, new long[] { 27 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5249 }, null, new string[] { "tdps2801", "tdps2802", "tdps2803" }, new long[] { 28 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5250, 5303 }, null, new string[] { "tdps2901", "tdps2902", "tdps2903" }, new long[] { 29 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5308, 5300 }, null, new string[] { "tdps3001", "tdps3002", "tdps3003" }, new long[] { 30 }); // ok
			//	await CreateGroupAndAssign(result, new long[] { 5006 }, new long[] { 5301, 5302 }, null, new string[] { "tdps3101", "tdps3102", "tdps3103" }, new long[] { 31 }); // ok


			//	db.Commit();
			//}
			//catch (Exception e)
			//{
			//	db.Rollback();
			//}

			return null;
		}

		public async Task<ActionResult> InitEvalGroup()
		{
			//var expertName = "hktdzj";
			//int index = 1;
			//var groups = db.ExpGroupDal.ConditionQuery(null, null, null, null);

			//try
			//{
			//	db.BeginTrans();

			//	foreach (var group in groups)
			//	{
			//		await CreateGroupAccesser(string.Format("{0}{1}", expertName, index++), group.GroupId);
			//		await CreateGroupAccesser(string.Format("{0}{1}", expertName, index++), group.GroupId);
			//	}

			//	db.Commit();
			//}
			//catch
			//{
			//	db.Rollback();
			//}

			return null;
		}



		private async Task CreateGroupAndAssign(List<DeclareReview> targets, long[] targetIds, long[] subjectIds, long[] stageIds, string[] accessorNames, long[] groupIds)
		{
			var teachers = new List<DeclareReview>();
			if (stageIds != null)
			{
				teachers = (from t in targets
							where targetIds.Contains(t.DeclareTargetPKID) &&
								  subjectIds.Contains(t.DeclareSubjectPKID) &&
								  stageIds.Contains(t.StageId)
							select t).ToList();
			}
			else
			{
				teachers = (from t in targets
							where targetIds.Contains(t.DeclareTargetPKID) &&
								  subjectIds.Contains(t.DeclareSubjectPKID)
							//stageIds == null ? true : stageIds.Contains(t.StageId)
							select t).ToList();
			}

			teachers = teachers.Distinct().ToList();

			foreach (var gid in groupIds)
			{
				var groupName = gid < 10 ? $"201905020_0{gid}" : $"201905020_{gid}";
				var group = new ExpGroup { Name = groupName, CreateDate = DateTime.Now };
				db.ExpGroupDal.Insert(group);

				foreach (var item in teachers)
				{
					db.ExpGroupTargetDal.Insert(new ExpGroupTarget { GroupId = group.GroupId, MemberId = item.TeacherId });
				}

				foreach (var name in accessorNames)
				{
					await CreateGroupAccesser(name, group.GroupId);
				}
			}

		}

		private async Task CreateGroupAccesser(string name, long groupId)
		{
			var account = new BzUser
			{
				UserName = name,
				Email = name + "@hktd.com",
				Actived = true,
			};
			var profile = new BzUserProfile
			{
				UserName = name,
				UserType = ThisApp.Teacher,
				RealName = name,
				Birthday = DateTime.Now
			};
			var expect = new Expect { ExpectId = account.Id };

			await _initExpertAdd(account, ThisApp.DefaultPassword, profile, expect);

			db.ExpGroupMemberDal.Insert(new ExpGroupMember { ExpectID = account.Id, GroupId = groupId, IsLeader = false });
		}

		private async Task _initExpertAdd(BzUser user, string password, BzUserProfile profile, Expect expert)
		{
			var result = await UserManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				profile.UserId = user.Id;
				expert.ExpectId = user.Id;
				db.BzUserProfileDal.Insert(profile);
				db.ExpectDal.Insert(expert);
			}
			else
			{
				throw new Exception(result.Errors.First());
			}
		}

	}
}