﻿using Business.Helper;

namespace Business
{

	public partial class DeclareProfile
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

      public bool IsSystemAdmin { get; set; }

      public bool IsSchoolAdmin { get; set; }

      public long TargetId { get; set; }

    }

}