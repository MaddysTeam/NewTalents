using Business.Config;
using Symber.Web.Report;

namespace Business.Helper
{

	public static class BzUserProfileHelper
	{

		static APDBDef.BzUserProfileTableDef t = APDBDef.BzUserProfile;

		public static IDAPRptColumn UserId { get; } = new IDAPRptColumn(t.UserId);
		public static TextAPRptColumn UUID { get; } = new TextAPRptColumn(t.UUID);
		// companyID待处理
		public static TextAPRptColumn UserName { get; } = new TextAPRptColumn(t.UserName);
		public static TextAPRptColumn UserType { get; } = new TextAPRptColumn(t.UserType);
		public static TextAPRptColumn RealName { get; } = new TextAPRptColumn(t.RealName);
		public static TextAPRptColumn IDCard { get; } = new TextAPRptColumn(t.IDCard);
		public static TextAPRptColumn TrainNo { get; } = new TextAPRptColumn(t.TrainNo);
		public static PicklistAPRptColumn Gender { get; } = new PicklistAPRptColumn(t.GenderPKID, PicklistKeys.Gender);
		public static DateTimeChineseAPRptColumn Birthday { get; } = new DateTimeChineseAPRptColumn(t.Birthday, APRptDateTimeType.DateOnly);
		public static PicklistAPRptColumn PoliticalStatus { get; } = new PicklistAPRptColumn(t.PoliticalStatusPKID, PicklistKeys.PoliticalStatus);
		public static PicklistAPRptColumn Nationality { get; } = new PicklistAPRptColumn(t.NationalityPKID, PicklistKeys.Nationality);
		public static PicklistAPRptColumn EduSubject { get; } = new PicklistAPRptColumn(t.EduSubjectPKID, PicklistKeys.EduSubject);
		public static PicklistAPRptColumn EduStage { get; } = new PicklistAPRptColumn(t.EduStagePKID, PicklistKeys.EduStage);
		public static DateTimeChineseAPRptColumn JobDate { get; } = new DateTimeChineseAPRptColumn(t.JobDate, APRptDateTimeType.DateOnly);
		public static PicklistAPRptColumn SkillTitle { get; } = new PicklistAPRptColumn(t.SkillTitlePKID, PicklistKeys.SkillTitle);
		public static DateTimeChineseAPRptColumn SkillDate { get; } = new DateTimeChineseAPRptColumn(t.SkillDate, APRptDateTimeType.DateOnly);
		public static TextAPRptColumn CompanyName { get; } = new TextAPRptColumn(t.CompanyName);
		public static TextAPRptColumn CompanyNameOuter { get; } = new TextAPRptColumn(t.CompanyNameOuter);
		public static TextAPRptColumn Companyaddress { get; } = new TextAPRptColumn(t.Companyaddress);
		public static PicklistAPRptColumn RankTitle { get; } = new PicklistAPRptColumn(t.RankTitlePKID, PicklistKeys.RankTitle);
		public static PicklistAPRptColumn EduBg { get; } = new PicklistAPRptColumn(t.EduBgPKID, PicklistKeys.EduBg);
		public static PicklistAPRptColumn EduDegree { get; } = new PicklistAPRptColumn(t.EduDegreePKID, PicklistKeys.EduDegree);
		public static TextAPRptColumn GraduateSchool { get; } = new TextAPRptColumn(t.GraduateSchool);
		public static DateTimeChineseAPRptColumn GraduateDate { get; } = new DateTimeChineseAPRptColumn(t.GraduateDate, APRptDateTimeType.DateOnly);
		public static TextAPRptColumn Phonemobile { get; } = new TextAPRptColumn(t.Phonemobile);
		public static TextAPRptColumn Email { get; } = new TextAPRptColumn(t.Email);

		public static APRptColumnCollection Columns = new APRptColumnCollection
		{
		};

	}
}

