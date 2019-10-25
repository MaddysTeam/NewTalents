using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSite.Models;

namespace Business
{
	public static class TeamExtensions
	{

		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;

		public static List<TeamMemberModel> GetMemberListById(this APDBDef db, long teamId)
			=> APQuery.select(u.RealName, u.CompanyName, d.TeacherId,
												d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, tm.ContentValue)
				.from(tm,
						u.JoinInner(tm.MemberId == u.UserId),
						d.JoinInner(tm.MemberId == d.TeacherId))
				.where(tm.TeamId == teamId)
				.query(db, rd =>
				{
					return new TeamMemberModel()
					{
						TeacherId = d.TeacherId.GetValue(rd),
						RealName = u.RealName.GetValue(rd),
						CompanyName = u.CompanyName.GetValue(rd),
						Target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
						//Subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
						Stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
						ContentValue = tm.ContentValue.GetValue(rd),
					};
				}).ToList();

		public static string GetTeamContent(List<TeamContent> list, string key)
			=> list.Find(m => m.ContentKey == key) == null ? "" : list.Find(m => m.ContentKey == key).ContentValue;

		public static bool HasTeam(this APDBDef db,long teamId)
		{
			var tm = APDBDef.TeamMember;

			return db.TeamMemberDal.ConditionQueryCount(tm.TeamId == teamId) > 0;
		}


	}
}
