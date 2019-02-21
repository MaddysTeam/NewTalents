using Business.Config;
using Symber.Web.Report;
using System;

namespace Business.Helper
{

	public static class PicklistHelper
	{

		static APDBDef.DeclareActiveTableDef t = APDBDef.DeclareActive;
		static APDBDef.DeclareAchievementTableDef t1 = APDBDef.DeclareAchievement;
		static APDBDef.TeamActiveTableDef t2 = APDBDef.TeamActive;

		public static PicklistAPRptColumn XuesHuodType { get; } = new PicklistAPRptColumn(t.Dynamic9, PicklistKeys.XuesHuodType);
		public static PicklistAPRptColumn TesHuodKaizType { get; } = new PicklistAPRptColumn(t.Dynamic9, PicklistKeys.TesHuodKaizType);
		public static PicklistAPRptColumn Dynamic6 { get; } = new PicklistAPRptColumn(t1.Dynamic6, PicklistKeys.KetYanjType);
		public static PicklistAPRptColumn TeamActiveType { get; } = new PicklistAPRptColumn(t2.ActiveType, PicklistKeys.TeamActiveType);

		public static APRptColumnCollection Columns = new APRptColumnCollection
		{
		};

	}
}

