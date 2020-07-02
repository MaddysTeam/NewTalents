using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class TeamManageController : BaseController
	{

		static APDBDef.BzUserProfileTableDef up = APDBDef.BzUserProfile;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
		static APDBDef.CompanyTableDef c = APDBDef.Company;
		static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;

		#region [ 称号管理 ]


		// GET: TeamManage/DeclareList
		// POST-Ajax: TeamManage/DeclareList

		public ActionResult DeclareList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult DeclareList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.RealName, up.IDCard, c.CompanyName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, d.HasTeam, d.TeamName)
				 .from(up,
			 d.JoinLeft(up.UserId == d.TeacherId),
			 cd.JoinLeft(cd.TeacherId == up.UserId),
			 c.JoinLeft(c.CompanyId == cd.CompanyId)
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
				query.where_and(d.DeclareTargetPKID == null);
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
					case "teamName": query.order_by(sort.OrderBy(d.TeamName)); break;
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
					card = up.IDCard.GetValue(rd),
					companyName = c.CompanyName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					hasTeam = d.HasTeam.GetValue(rd),
					teamName = d.TeamName.GetValue(rd)
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


		////	GET: TeamManage/DeclareEdit
		////	POST-Ajax: TeamManage/DeclareEdit

		public ActionResult DeclareEdit(long id)
		{
			ThrowNotAjax();

			var model = db.DeclareBaseDal.PrimaryGet(id);
			if (model == null)
			{
				model = new DeclareBase
				{
					TeacherId = id,
				};
			}


			return PartialView("DeclareEdit", model);
		}

		[HttpPost]
		public ActionResult DeclareEdit(DeclareBase model)
		{
			ThrowNotAjax();


			var exist = (int)APQuery.select(d.TeacherId.Count())
				 .from(d)
				 .where(d.TeacherId == model.TeacherId)
				 .executeScale(db) > 0;

			var Team = db.TeamMemberDal.ConditionQuery(tm.MemberId == model.TeacherId, null, null, null).FirstOrDefault();
			var isTeamMaster = db.TeamMemberDal.ConditionQueryCount(tm.TeamId == model.TeacherId) > 0;

			if (model.DeclareTargetPKID == 0)
			{
				if (exist)
				{
					db.BeginTrans();

					try
					{
						db.DeclareBaseDal.PrimaryDelete(model.TeacherId);
						db.TeamMemberDal.ConditionDelete(tm.TeamId == model.TeacherId);

						//	如果是其他工作室学员需要给declarebase中的学员数量-1
						if (Team != null)
						{
							RemoveFromTeam(model.TeacherId, Team.TeamId);
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
			}
			else
			{
				if (DeclareTargetIds.HasTeam(model.DeclareTargetPKID) && isTeamMaster)
				{
					if (!DeclareTargetIds.AllowZhidJians(model.DeclareTargetPKID))
					{
						var name = (string)APQuery.select(up.RealName)
							 .from(up)
							 .where(up.UserId == model.TeacherId)
							 .executeScale(db);
						model.TeamName = DeclareBaseHelper.DeclareTarget.GetName(model.DeclareTargetPKID) + "(" + name + ")";
					}
					model.HasTeam = true;
				}
				else
				{
					model.TeamName = "";
					model.HasTeam = false;
				}
				if (!DeclareTargetIds.AllowQunLiud(model.DeclareTargetPKID))
				{
					model.AllowFlowToSchool = false;
				}
				if (!DeclareTargetIds.AllowPeihJiaoyy(model.DeclareTargetPKID))
				{
					model.AllowFitResearcher = false;
				}

				if (exist)
				{
					db.DeclareBaseDal.UpdatePartial(model.TeacherId, new
					{
						model.DeclareTargetPKID,
						model.DeclareSubjectPKID,
						model.DeclareStagePKID,
						model.TeamName,
						model.HasTeam,
						model.AllowFitResearcher,
						model.AllowFlowToSchool
					});
				}
				else
				{
					db.DeclareBaseDal.Insert(model);
				}
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "用户编辑成功"
			});
		}


		#endregion


		#region [ 团队管理 ]


		// GET: TeamManage/TeamList
		// POST-Ajax: TeamManage/TeamList

		public ActionResult TeamList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult TeamList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var subQuery = APQuery.select(tm.TeamId).from(tm).group_by(tm.TeamId);

			var query = APQuery.select(up.UserId, up.RealName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, d.TeamName, d.MemberCount)
				 .from(d,
						up.JoinInner(up.UserId == d.TeacherId)
				 )
				 .where(d.TeacherId.In(subQuery))
				 .primary(d.TeacherId)
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
				query.where_and(d.DeclareTargetPKID == null);
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
					case "teamName": query.order_by(sort.OrderBy(d.TeamName)); break;
					case "memberCount": query.order_by(sort.OrderBy(tm.MemberId.Count())); break;
				}
			}


			//获得查询的总数量

			var total = db.ExecuteSizeOfSelect(query);


			//查询结果集


			var result = query.query(db, rd =>
			{
				var stageName = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd));
				var subjectName = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd));
				var realName = up.RealName.GetValue(rd);
				return new
				{
					id = up.UserId.GetValue(rd),
					realName = realName,
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = stageName,
					stage = stageName,
					teamName = $"{stageName}{subjectName}{realName}",
					memberCount = d.MemberCount.GetValue(rd)
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


		//	GET: TeamManage/TeamMember
		//	POST-Ajax: TeamManage/RemoveMember

		public ActionResult TeamMember(long teamId)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.RealName,
				 d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
				 .from(up, d.JoinInner(up.UserId == d.TeacherId), tm.JoinInner(d.TeacherId == tm.MemberId))
				 .where(tm.TeamId == teamId);

			var list = query.query(db, rd =>
			{
				return new AdjuctMemberModel
				{
					id = up.UserId.GetValue(rd),
					realName = up.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
				};
			}).ToList();


			return PartialView("TeamMember", list);
		}

		[HttpPost]
		public ActionResult RemoveMember(long memberId, long teamId)
		{
			ThrowNotAjax();

			db.BeginTrans();
			try
			{
				RemoveFromTeam(memberId, teamId);

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
				msg = "学员移除成功"
			});
		}

		#endregion


		#region [ 学员管理 ]


		// 梯队列表
		// GET: TeamManage/MemberList
		// POST-Ajax: TeamManage/MemberList

		public ActionResult MemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult MemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var dalias = d.As("dalias");

			var query = APQuery.select(up.UserId, up.RealName, up.CompanyName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, dalias.TeamName)
				 .from(
					  up,
					  d.JoinInner(up.UserId == d.TeacherId),
					  tm.JoinLeft(up.UserId == tm.MemberId),
					  dalias.JoinLeft(tm.TeamId == dalias.TeacherId))
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
				query.where_and(dalias.TeamName == null);
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
					case "company": query.order_by(sort.OrderBy(up.CompanyName)); break;
					case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
					case "teamName": query.order_by(sort.OrderBy(dalias.TeamName)); break;
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
					company = up.CompanyName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					targetId = d.DeclareTargetPKID.GetValue(rd),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					teamName = dalias.TeamName.GetValue(rd)
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


		//	GET: TeamManage/AssignMaster
		//	POST-Ajax: TeamManage/FindMaster
		//	POST-Ajax: TeamManage/AssignMaster

		public ActionResult AssignMaster(long memberId)
		{
			ThrowNotAjax();

			return PartialView("AssignMaster", memberId);
		}

		[HttpPost]
		public ActionResult FindMaster(long memberId, string name)
		{
			ThrowNotAjax();

			var memberTarget = (long)APQuery.select(d.DeclareTargetPKID)
				 .from(d)
				 .where(d.TeacherId == memberId)
				 .executeScale(db);

			var query = APQuery.select(up.UserId, up.RealName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, d.TeamName)
				 .from(up, d.JoinInner(up.UserId == d.TeacherId))
				 .where(d.HasTeam == true & up.UserId != memberId
					  & d.DeclareTargetPKID < memberTarget
					  & up.RealName.Match(name.Trim()))
				 .take(10);

			var result = query.query(db, rd =>
			{
				return new FindMasterModel
				{
					id = up.UserId.GetValue(rd),
					realName = up.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					teamName = d.TeamName.GetValue(rd)
				};
			}).ToList();


			return PartialView("FindMaster", result);
		}

		[HttpPost]
		public ActionResult AssignMaster(long memberId, long teamId)
		{
			ThrowNotAjax();

			var exist = APQuery.select(tm.TeamId)
				 .from(tm)
				 .where(tm.MemberId == memberId)
				 .executeScale(db);

			if (exist != null && (long)exist == teamId)
			{

			}
			else
			{
				db.BeginTrans();
				try
				{
					if (exist == null)
					{
						AddToTeam(memberId, teamId);
					}
					else
					{
						long oldTeamId = (long)exist;
						RemoveFromTeam(memberId, oldTeamId);
						AddToTeam(memberId, teamId);
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
				msg = "指派导师成功"
			});
		}


		#endregion


		#region [ 学员校评单位管理 ]


		// GET: TeamManage/CompanyList
		// POST-Ajax: TeamManage/CompanyList

		public ActionResult CompanyList()
		{
			return View();
		}

		[HttpPost]
		public JsonResult CompanyList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage, long companyId)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.RealName, c.CompanyName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
				 .from(
					  up,
					  d.JoinInner(up.UserId == d.TeacherId),
					  cd.JoinLeft(up.UserId == cd.TeacherId),
					  c.JoinLeft(c.CompanyId == cd.CompanyId)
					  )
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
				query.where_and(d.DeclareTargetPKID == null);
			}
			if (subject > 0)
			{
				query.where_and(d.DeclareSubjectPKID == subject);
			}
			if (stage > 0)
			{
				query.where_and(d.DeclareStagePKID == stage);
			}
			if (companyId > 0)
			{
				query.where_and(cd.CompanyId == companyId);
			}

			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
					case "company": query.order_by(sort.OrderBy(c.CompanyName)); break;
					case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
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
					company = c.CompanyName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
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


		// GET:TeamManage/AssignCompany	
		// POST-Ajax: TeamManage/FindCompany
		// POST-Ajax: TeamManage/AssginCompany

		public ActionResult AssignCompany(long memberId)
		{
			ThrowNotAjax();

			return PartialView("AssignCompany", memberId);
		}

		[HttpPost]
		public ActionResult FindCompany(long memberId, string name)
		{
			ThrowNotAjax();


			var query = APQuery.select(c.CompanyId, c.CompanyName)
					.from(c)
					.where(c.CompanyName.Match(name))
					.take(10)
					.query(db, (rd) =>
					{
						return new FindCompanyModel
						{
							companyId = c.CompanyId.GetValue(rd),
							companyName = c.CompanyName.GetValue(rd)
						};
					});

			var result = query.ToList();

			return PartialView("FindCompany", result);
		}

		[HttpPost]
		public ActionResult AssignCompany(long memberId, long companyId)
		{
			ThrowNotAjax();


			var exist = APQuery.select(cd.TeacherId)
				 .from(cd)
				 .where(cd.TeacherId == memberId)
				 .executeScale(db);

			if (exist == null)
			{
				db.CompanyDeclareDal.Insert(new CompanyDeclare { CompanyId = companyId, TeacherId = memberId });
			}
			else
			{
				APQuery.update(cd)
					 .set(cd.CompanyId.SetValue(companyId))
					 .where(cd.TeacherId == memberId)
					 .execute(db);
			}


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "指派单位成功"
			});
		}

		#endregion


		#region [ 学员质评专家组管理 ]


		// GET: TeamManage/ExpGroupList	
		// POST-Ajax: TeamManage/ExpGroupList

		public ActionResult ExpGroupList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ExpGroupList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.RealName,
					  d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, eg.Name)
				 .from(up,
							d.JoinInner(up.UserId == d.TeacherId),
							egt.JoinLeft(egt.MemberId == up.UserId),
							eg.JoinLeft(eg.GroupId == egt.GroupId)
							)
				 .where(up.UserType == BzRoleNames.Teacher);


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
				query.where_and(egt.GroupId == null);
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
					case "name": query.order_by(sort.OrderBy(eg.Name)); break;
				}
			}


			//获得查询的总数量

			//var total = db.ExecuteSizeOfSelect(query);


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
					name = eg.Name.GetValue(rd),
				};
			}).ToList();


			var total = result.Count;
			var results = result.Skip(rowCount * (current - 1)).Take(rowCount).ToList();

			return Json(new
			{
				rows = results,
				current,
				rowCount,
				total
			});
		}


		// GET: TeamManage/AssignExpGroup	
		// POST-Ajax: TeamManage/AssignExpGroup
		// POST-Ajax: TeamManage/FindExpGroup

		public ActionResult AssignExpGroup(long memberId)
		{
			ThrowNotAjax();


			return PartialView(memberId);
		}

		[HttpPost]
		public ActionResult AssignExpGroup(long memberId, long expGroupId)
		{
			ThrowNotAjax();


			var existGroupTarget = db.ExpGroupTargetDal.ConditionQuery(egt.MemberId == memberId, null, null, null).FirstOrDefault();

			if (existGroupTarget != null && existGroupTarget.GroupId == expGroupId)
			{
				return Json(new
				{
					result = AjaxResults.Success,
					msg = "学员已经绑定该组"
				});
			}


			db.BeginTrans();

			try
			{
				if (existGroupTarget != null)
				{
					RemoveFromExpGroup(memberId, existGroupTarget.GroupId);
				}

				AddToExpGroup(memberId, expGroupId);

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


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "置评绑定成功"
			});
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


		//	GET: TeamManage/RemoveExpMember
		//	POST-Ajax: TeamManage/RemoveExpMember

		public ActionResult RemoveExpMember(long id)
		{
			var query = APQuery.select(up.UserId, up.RealName,
					d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
			  .from(
					up,
					egt.JoinRight(egt.MemberId == up.UserId),
					d.JoinLeft(up.UserId == d.TeacherId)
					)
			  .where(egt.GroupId == id)
			  .query(db, (rd) =>
			  {
				  return new AdjustExpMemberModel
				  {
					  id = up.UserId.GetValue(rd),
					  realName = up.RealName.GetValue(rd),
					  target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					  subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					  stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd))
				  };
			  });

			var result = query.ToList();

			ViewBag.groupId = id;

			return PartialView("RemoveExpMember", result);
		}

		[HttpPost]
		public ActionResult RemoveExpMember(long memberId, long groupId)
		{
			ThrowNotAjax();


			var existExpGroupMembers = db.ExpGroupTargetDal.ConditionQuery(egt.GroupId == groupId & egt.MemberId == memberId, null, null, null)
				 .ToDictionary(key => new { key1 = key.GroupId, key2 = key.MemberId }, value => value);

			if (existExpGroupMembers.ContainsKey(new { key1 = groupId, key2 = memberId }))
			{
				db.BeginTrans();

				try
				{
					RemoveFromExpGroup(memberId, groupId);

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
				msg = "移除学员成功"
			});
		}

		#endregion


		#region [ Helper ]


		private void AddToTeam(long memberId, long teamId)
		{
			APQuery.insert(tm)
				 .set(tm.TeamId.SetValue(teamId))
				 .set(tm.MemberId.SetValue(memberId))
				 .set(tm.ContentValue.SetValue(""))
				 .set(tm.Creator.SetValue(UserProfile.UserId))
				 .set(tm.CreateDate.SetValue(DateTime.Now))
				 .execute(db);
			APQuery.update(d)
				 .set(d.MemberCount.SetValue(APSqlRawExpr.Expr("MemberCount + 1")))
				 .where(d.TeacherId == teamId)
				 .execute(db);
		}

		private void RemoveFromTeam(long memberId, long teamId)
		{
			APQuery.delete(tm)
				 .where(tm.TeamId == teamId & tm.MemberId == memberId)
				 .execute(db);
			APQuery.update(d)
				 .set(d.MemberCount.SetValue(APSqlRawExpr.Expr("MemberCount - 1")))
				 .where(d.TeacherId == teamId)
				 .execute(db);
		}

		private void RemoveFromExpGroup(long teacherId, long groupId)
		{
			APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
			APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
			APDBDef.EvalQualitySubmitResultTableDef esr = APDBDef.EvalQualitySubmitResult;


			// 当会员移出专家组时，如果该会员的考评未被提交，则删除考评结果和考评结果项

			var results = db.EvalQualityResultDal.ConditionQuery(er.TeacherId == teacherId & er.GroupId == groupId, null, null, null)
				 .ToDictionary(key => new { key.PeriodId, key.TeacherId, key.Accesser });

			var submitResults = db.EvalQualitySubmitResultDal.ConditionQuery(esr.GroupId == groupId, null, null, null)
					.ToDictionary(key => new { key.PeriodId, key.TeacherId, key.Accesser });

			var removeResults = new List<long>();

			foreach (var item in results)
			{
				if (!submitResults.ContainsKey(item.Key))
				{
					removeResults.Add(item.Value.ResultId);
				}
				else
				{
					removeResults.Clear();
				}
			}

			if (removeResults.Count > 0)
			{
				var removeArray = removeResults.ToArray();
				db.EvalQualityResultItemDal.ConditionDelete(eri.ResultId.In(removeArray));
				db.EvalQualityResultDal.ConditionDelete(er.ResultId.In(removeArray));
			}


			db.ExpGroupTargetDal.ConditionDelete(egt.MemberId == teacherId);

			APQuery.update(eg)
						.set(eg.TeacherCount, APSqlRawExpr.Expr("TeacherCount - 1"))
						.where(eg.GroupId == groupId)
						.execute(db);
		}

		private void AddToExpGroup(long teacherId, long groupId)
		{
			db.ExpGroupTargetDal.Insert(new ExpGroupTarget { GroupId = groupId, MemberId = teacherId });

			APQuery.update(eg)
					  .set(eg.TeacherCount, APSqlRawExpr.Expr("TeacherCount + 1"))
					  .where(eg.GroupId == groupId)
					  .execute(db);
		}

		#endregion

	}

}