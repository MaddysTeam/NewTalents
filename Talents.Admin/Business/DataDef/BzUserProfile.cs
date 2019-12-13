using Business.Helper;

namespace Business
{

	public partial class BzUserProfile
	{

		public string Gender => BzUserProfileHelper.Gender.GetName(GenderPKID);

		public string PoliticalStatus => BzUserProfileHelper.PoliticalStatus.GetName(PoliticalStatusPKID);

		public string Nationality => BzUserProfileHelper.Nationality.GetName(NationalityPKID);

		public string EduSubject => BzUserProfileHelper.EduSubject.GetName(EduSubjectPKID);

		public string EduStage => BzUserProfileHelper.EduStage.GetName(EduStagePKID);

		public string SkillTitle => BzUserProfileHelper.SkillTitle.GetName(SkillTitlePKID);

		public string RankTitle => BzUserProfileHelper.RankTitle.GetName(RankTitlePKID);

		public string EduBg => BzUserProfileHelper.EduBg.GetName(EduBgPKID);

		public string EduDegree => BzUserProfileHelper.EduDegree.GetName(EduDegreePKID);

		public bool IsExtLogined { get; set; }

		public bool IsDeclare { get; set; }

		public bool IsMaster { get; set; }

		public bool IsMember { get; set; }

		public bool IsExpert { get; set; }

		public bool IsSpecialExpert { get; set; } //TODO:冗余字段，作为特殊情况使用，可以被回收

		public bool IsSystemAdmin { get; set; }

		public bool IsSchoolAdmin { get; set; }

		public long TargetId { get; set; }

		public bool IsTeamLeader { get; set; }

		//TODO: 高地理事长和基地支持人的自荐表特殊处理 ,暂存申报提交状态供自荐表使用
		public string StatusKey { get; set; }

	}

}