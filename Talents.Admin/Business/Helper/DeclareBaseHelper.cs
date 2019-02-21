using Business.Config;
using Symber.Web.Report;

namespace Business.Helper
{

	public static class DeclareBaseHelper
	{

		static APDBDef.DeclareBaseTableDef t = APDBDef.DeclareBase;

		public static IDAPRptColumn TeacherId { get; } = new IDAPRptColumn(t.TeacherId);
		public static PicklistAPRptColumn DeclareTarget { get; } = new PicklistAPRptColumn(t.DeclareTargetPKID, PicklistKeys.DeclareTarget);
		public static PicklistAPRptColumn DeclareSubject { get; } = new PicklistAPRptColumn(t.DeclareSubjectPKID, PicklistKeys.DeclareSubject);
		public static PicklistAPRptColumn DeclareStage { get; } = new PicklistAPRptColumn(t.DeclareStagePKID, PicklistKeys.DeclareStage);
		public static CheckAPRptColumn AllowFlowToSchool { get; } = new CheckAPRptColumn(t.AllowFlowToSchool);
		public static CheckAPRptColumn AllowFitResearcher { get; } = new CheckAPRptColumn(t.AllowFitResearcher);
		public static CheckAPRptColumn HasTeam { get; } = new CheckAPRptColumn(t.HasTeam);
		public static TextAPRptColumn TeamName { get; } = new TextAPRptColumn(t.TeamName);
		public static Int32APRptColumn MemberCount { get; } = new Int32APRptColumn(t.MemberCount);
		public static Int32APRptColumn ActiveCount { get; } = new Int32APRptColumn(t.ActiveCount);

		public static APRptColumnCollection Columns = new APRptColumnCollection
		{
		};

	}
}

