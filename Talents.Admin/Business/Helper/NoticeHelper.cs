using Symber.Web.Data;
using System.Collections.Generic;
using System.Linq;
using TheSite.Models;

namespace Business.Helper
{

	public static class NoticeHelper
	{

		public static List<NoticeViewModel> GetNoticeList(long UserId)
		{
			var db = new APDBDef();
			var t = APDBDef.Notice;
			var t1 = APDBDef.ReadNotice;
			var u = APDBDef.BzUserProfile;

			var subQuery = APQuery.select(t1.NoticeId)
				.from(t1)
				.where(t1.UserId == UserId);

			var list = APQuery.select(t.NoticeId, t.Title, t.CreatedTime, u.RealName)
				.from(t, u.JoinInner(t.Creator == u.UserId))
				.where(t.IsSend == true & t.NoticeId.NotIn(subQuery))
				.take(5)
				.query(db, r =>
				{
					return new NoticeViewModel
					{
						NoticeId = t.NoticeId.GetValue(r),
						Title = t.Title.GetValue(r),
						CreatorName = u.RealName.GetValue(r),
						CreatedTime = t.CreatedTime.GetValue(r)
					};
				}).ToList();

			return list;
		}


		public static int GetNoticeCount(long UserId)
		{
			var db = new APDBDef();
			var t = APDBDef.Notice;
			var t1 = APDBDef.ReadNotice;

			var subQuery = APQuery.select(t1.NoticeId)
				.from(t1)
				.where(t1.UserId == UserId);

			var count = APQuery.select(t.NoticeId, t.Title, t.CreatedTime)
				.from(t)
				.where(t.NoticeId.NotIn(subQuery))
				.count(db);

			return count;
		}
	}

}