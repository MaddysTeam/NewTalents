using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Controllers;
using TheSite.Models;

namespace Talents.Admin.Controllers
{
	public class ExpManageController : BaseController
	{

		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;
		static APDBDef.ExpectTableDef e = APDBDef.Expect;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.BzUserProfileTableDef up = APDBDef.BzUserProfile;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;


		#region [ 专家组管理 ]


		// GET: ExpManage/GroupList
		// POST-Ajax: ExpManage/GroupList

		public ActionResult GroupList()
		{
			return View();
		}

		[HttpPost]
		public JsonResult GroupList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var query = APQuery.select(eg.GroupId, eg.Name, eg.DeclareStagePKID, eg.DeclareSubjectPKID, eg.DeclareTargetPKID, eg.MemberCount, eg.TeacherCount, up.RealName, egm.IsLeader)
				 .from(eg,
							egm.JoinLeft(egm.GroupId == eg.GroupId),
							up.JoinLeft(up.UserId == egm.ExpectID)
				  )
				 //.where(egm.IsLeader == true | up.UserName == null)
				 .primary(eg.GroupId)
				 .skip((current - 1) * rowCount)
				 .take(rowCount);

			//过滤条件
			//模糊搜索专家组名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(eg.Name.Match(searchPhrase));
			}

			if (target > 0)
			{
				query.where_and(eg.DeclareTargetPKID == target);
			}
			else if (target == -1)
			{
				query.where_and(eg.DeclareTargetPKID == null);
			}
			if (subject > 0)
			{
				query.where_and(eg.DeclareSubjectPKID == subject);
			}
			if (stage > 0)
			{
				query.where_and(eg.DeclareStagePKID == stage);
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "name": query.order_by(sort.OrderBy(eg.Name)); break;
					case "target": query.order_by(sort.OrderBy(eg.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(eg.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(eg.DeclareStagePKID)); break;
					case "memberCount": query.order_by(sort.OrderBy(eg.MemberCount)); break;
					case "teacherCount": query.order_by(sort.OrderBy(eg.TeacherCount)); break;
					case "leaderRealName": query.order_by(sort.OrderBy(up.RealName)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			 {
				 return new
				 {
					 id = eg.GroupId.GetValue(rd),
					 name = eg.Name.GetValue(rd),
					 memberCount = eg.MemberCount.GetValue(rd),
					 teacherCount = eg.TeacherCount.GetValue(rd),
					 target = DeclareBaseHelper.DeclareTarget.GetName(eg.DeclareTargetPKID.GetValue(rd)),
					 subject = DeclareBaseHelper.DeclareSubject.GetName(eg.DeclareSubjectPKID.GetValue(rd)),
					 stage = DeclareBaseHelper.DeclareStage.GetName(eg.DeclareStagePKID.GetValue(rd)),
					 leaderRealName = up.RealName.GetValue(rd),
					 isLeader = egm.IsLeader.GetValue(rd)
				 };
			 });

			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		// GET: ExpManage/GroupEdit
		// POST-Ajax: ExpManage/GroupEdit

		public ActionResult GroupEdit(long? id)
		{
			ThrowNotAjax();

			var groupModel = id == null ? new ExpGroup() : db.ExpGroupDal.PrimaryGet(id.Value);

			return PartialView("GroupEdit", groupModel);
		}

		[HttpPost]
		public ActionResult GroupEdit(ExpGroup model)
		{
			ThrowNotAjax();

			if (model.GroupId == 0)
			{
				model.CreateDate = DateTime.Now;
				model.Creator = UserProfile.UserId;
				db.ExpGroupDal.Insert(model);
			}
			else
			{
				model.Modifier = UserProfile.UserId;
				model.ModifyDate = DateTime.Now;
				db.ExpGroupDal.Update(model);
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "用户编辑成功"
			});
		}


		// GET: ExpManage/MemberGroupList

		public ActionResult MemberGroupList(long memberId)
		{
			ThrowNotAjax();


			var query = APQuery.select(eg.Name, eg.GroupId, up.RealName)
				 .from(
				  egm,
				  up.JoinInner(up.UserId == egm.ExpectID),
				  eg.JoinInner(eg.GroupId == egm.GroupId)
				 ).where(egm.ExpectID == memberId);

			var result = query.query(db, (rd) =>
			{
				return new MemberGroupModel
				{
					expGroupId = eg.GroupId.GetValue(rd),
					expGroupName = eg.Name.GetValue(rd),
					realName = up.RealName.GetValue(rd)
				};
			}).ToList();

			ViewBag.MemberId = memberId;


			return PartialView("MemberGroupList", result);
		}

		#endregion


		#region [ 专家管理 ]


		// GET: ExpManage/ExpList
		// POST-Ajax: ExpManage/ExpList

		public ActionResult ExpList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ExpList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.RealName, e.GroupCount,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
				 .from(
					  up,
					  d.JoinLeft(up.UserId == d.TeacherId),
					  e.JoinLeft(e.ExpectId == up.UserId)
					  )
				 .where(up.UserType == BzRoleNames.Teacher)
				 .primary(up.UserId)
				 .skip((current - 1) * rowCount)
				 .take(rowCount);


			//过滤条件
			//模糊搜索用户名、实名进行

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(up.RealName.Match(searchPhrase));
			}

			if (target > 0)
			{
				query.where_and(d.DeclareTargetPKID == target);
			}
			else if (target == -1)
			{
				query.where_and(e.GroupCount == 0 | e.GroupCount == null);
			}
			if (subject > 0)
			{
				query.where_and(d.DeclareSubjectPKID == subject);
			}
			if (stage > 0)
			{
				query.where_and(d.DeclareStagePKID == stage);
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
					case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
					case "groupCount": query.order_by(sort.OrderBy(e.GroupCount)); break;
				}
			}


			//获得查询的总数量

			var total = db.ExecuteSizeOfSelect(query);


			//查询结果集

			var result = query.query(db, rd =>
			{
				return new
				{
					id = up.UserId.GetValue(rd),
					realName = up.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					groupCount = e.GroupCount.GetValue(rd)
				};
			});


			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		// GET: ExpManage/AssignExpGroup	
		// POST-Ajax: ExpManage/FindExpGroup
		// POST-Ajax: ExpManage/AssignExpGroup

		public ActionResult AssignExpGroup(long memberId)
		{
			ThrowNotAjax();

			return PartialView("AssignExpGroup", memberId);
		}

		[HttpPost]
		public ActionResult FindExpGroup(long memberId, string name)
		{
			ThrowNotAjax();

			var query = APQuery.select(eg.GroupId, eg.Name)
				 .from(eg)
				 .where(eg.Name.Match(name))
				 .take(10)
				 .query(db, rd =>
				 {
					 return new FindExpGroupModel
					 {
						 expGroupId = eg.GroupId.GetValue(rd),
						 expGroupName = eg.Name.GetValue(rd)
					 };
				 });

			var result = query.ToList();

			return PartialView("FindExpGroup", result);
		}

		[HttpPost]
		public ActionResult AssignExpGroup(long memberId, long expGroupId)
		{
			ThrowNotAjax();


			var existExpGroupMembers = db.ExpGroupMemberDal.ConditionQuery(null, null, null, null).ToDictionary(key => new { key1 = key.GroupId, key2 = key.ExpectID }, value => value);

			if (!existExpGroupMembers.ContainsKey(new { key1 = expGroupId, key2 = memberId }))
			{

				db.BeginTrans();

				try
				{
					var expert = db.ExpectDal.ConditionQuery(e.ExpectId == memberId, null, null, null).FirstOrDefault();

					if (expert == null)
					{
						db.ExpectDal.Insert(new Expect { ExpectId = memberId, GroupCount = 1 });
					}
					else
					{
						APQuery.update(e)
							  .set(e.GroupCount, APSqlRawExpr.Expr("GroupCount + 1"))
							  .where(e.ExpectId == memberId)
							  .execute(db);
					}

					var groupMembersCount = existExpGroupMembers.Where(x => x.Value.GroupId == expGroupId).Count();

					db.ExpGroupMemberDal.Insert(new ExpGroupMember { ExpectID = memberId, GroupId = expGroupId, IsLeader = groupMembersCount == 0 });

					APQuery.update(eg)
									 .set(eg.MemberCount, APSqlRawExpr.Expr("MemberCount + 1"))
									 .where(eg.GroupId == expGroupId)
									 .execute(db);


					db.Commit();
				}
				catch (Exception e)
				{
					db.Rollback();

					return Json(new
					{
						result = AjaxResults.Error,
						msg = e.Message
					});
				}
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "加入专家组成功"
			});
		}


		//	GET: ExpManage/RemoveExpMember
		//	POST-Ajax: ExpManage/RemoveExpMember

		public ActionResult RemoveExpMember(long id)
		{
			ThrowNotAjax();

			var result = APQuery.select(up.UserId, up.RealName,
					d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, egm.IsLeader)
			  .from(
					up,
					egm.JoinRight(egm.ExpectID == up.UserId),
					d.JoinLeft(up.UserId == d.TeacherId)
					)
			  .where(egm.GroupId == id)
			  .query(db, (rd) =>
				 {
					 return new AdjustExpMemberModel
					 {
						 id = up.UserId.GetValue(rd),
						 realName = up.RealName.GetValue(rd),
						 target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
						 subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
						 stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
						 isLeader = egm.IsLeader.GetValue(rd)
					 };
				 }).ToList();


			ViewBag.groupId = id;

			return PartialView("RemoveExpMember", result);
		}

		[HttpPost]
		public ActionResult RemoveExpMember(long memberId, long groupId)
		{
			ThrowNotAjax();


			var existMemebers = db.ExpGroupMemberDal.ConditionQuery(egm.GroupId == groupId, null, null, null);
			var existMemeber = existMemebers.FirstOrDefault(x => x.ExpectID == memberId);

			if (existMemeber != null)
			{
				// 当专家移出专家组时，如果该专家考评的学员未被提交，则删除其考评结果和结果明细

				var er = APDBDef.EvalQualityResult;
				var eri = APDBDef.EvalQualityResultItem;
				var esr = APDBDef.EvalQualitySubmitResult;


				var results = db.EvalQualityResultDal.ConditionQuery(er.Accesser == memberId & er.GroupId == groupId, null, null, null)
					 .ToDictionary(key => new { key.PeriodId, key.TeacherId });

				var submitResults = db.EvalQualitySubmitResultDal.ConditionQuery(esr.GroupId == groupId, null, null, null)
						.ToDictionary(key => new { key.PeriodId, key.TeacherId });

				List<long> removeResults = new List<long>();

				foreach (var item in results)
				{
					if (!submitResults.ContainsKey(item.Key))
					{
						removeResults.Add(item.Value.ResultId);
					}
				}

				var groupCount = db.ExpectDal.ConditionQuery(e.ExpectId == memberId, null, null, null)
					.FirstOrDefault().GroupCount;

				db.BeginTrans();

				try
				{
					// 调整专家参与的组数量，或移除专家身份

					if (groupCount > 1)
					{
						APQuery.update(e)
									.set(e.GroupCount, APSqlRawExpr.Expr("GroupCount - 1"))
									.where(e.ExpectId == memberId & e.GroupCount > 0)
									.execute(db);
					}
					else
					{
						db.ExpectDal.PrimaryDelete(memberId);
					}


					// 调整专家组内专家数量

					APQuery.update(eg)
								.set(eg.MemberCount, APSqlRawExpr.Expr("MemberCount - 1"))
								.where(eg.GroupId == groupId)
								.execute(db);

					db.ExpGroupMemberDal.ConditionDelete(egm.ExpectID == memberId & egm.GroupId == groupId);


					// 设定组长

					if (existMemeber.IsLeader && existMemebers.Count > 1)
					{
						var nextLeader = existMemebers.FirstOrDefault(item => item.ExpectID != memberId && item.GroupId == groupId);

						AssignLeader(nextLeader.ExpectID, groupId);
					}


					// 删除组内该专家对学员的评价（评价未被提交的）

					if (removeResults.Count > 0)
					{
						var removeArray = removeResults.ToArray();
						db.EvalQualityResultItemDal.ConditionDelete(eri.ResultId.In(removeArray));
						db.EvalQualityResultDal.ConditionDelete(er.ResultId.In(removeArray));
					}


					db.Commit();

				}
				catch (Exception ex)
				{
					db.Rollback();

					return Json(new
					{
						result = AjaxResults.Error,
						msg = ex.Message
					});
				}
			}


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "移除专家成功"
			});
		}


		//	POST-Ajax: ExpManage/SetLeader

		[HttpPost]
		public ActionResult SetLeader(long memberId, long groupId)
		{
			ThrowNotAjax();

			db.BeginTrans();

			try
			{
				AssignLeader(memberId, groupId);

				db.Commit();
			}
			catch (Exception ex)
			{
				db.Rollback();

				return Json(new
				{
					result = AjaxResults.Error,
					msg = ex.Message
				});
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "组长设定成功"
			});
		}

		#endregion


		#region [ Helper ]


		private void AssignLeader(long memberId, long groupId)
		{
			APQuery.update(egm)
					  .set(egm.IsLeader.SetValue(false))
					  .where(egm.GroupId == groupId)
					  .execute(db);

			APQuery.update(egm)
								 .set(egm.IsLeader.SetValue(true))
								 .where(egm.GroupId == groupId & egm.ExpectID == memberId)
								 .execute(db);
		}


		#endregion
	}

}