using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class ExpertEvalOverviewModels
	{

		public long PeriodId { get; set; }
		public long GroupId { get; set; }
		public string GroupName { get; set; }
		public int GroupTargetMemberCount { get; set; }
		public int EvalTargetMemberCount { get; set; }
		public int NotEvalTargetMemberCount { get; set; }
		public string Accessor { get; set; }
		public EvalStatus EvalStatus { get; set;}

	}


	public class DeclareExperEvalManageViewModel
	{
		public long id { get; set; }
		public long teacherId { get; set; }
		public long targetId { get; set; }
		public long subjectId { get; set; }
		public long companyId { get; set; }

		[Display(Name = "专家姓名")]
		public string accessor { get; set; }
		[Display(Name = "教师姓名")]
		public string teacher { get; set; }
		[Display(Name = "申报称号")]
		public string target { get; set; }
		[Display(Name = "申报学科")]
		public string subject { get; set; }
		[Display(Name = "申报单位")]
		public string company { get; set; }
		[Display(Name = "总得分")]
		public double totalScore { get; set; }
		[Display(Name = "教育教学.公开课")]
		public double gkk { get; set; }
		[Display(Name = "教育教学.评比")]
		public double jxpb { get; set; }
		[Display(Name = "教育教学.评比.其他")]
		public double qt { get; set; }
		[Display(Name = "教研工作.中小学命题")]
		public double mt { get; set; }
		[Display(Name = "教研工作.担任评委")]
		public double pingw { get; set; }
		[Display(Name = "教研工作.德育")]
		public double dey { get; set; }
		[Display(Name = "教研工作.担任评委（二）")]
		public double drpw2 { get; set; }
		[Display(Name = "教育科研.立项课题或项目研究")]
		public double xmyj { get; set; }
		[Display(Name = "教育科研.发表论文")]
		public double fblw { get; set; }
		[Display(Name = "教师培训.培训课程")]
		public double ztjz { get; set; }
		[Display(Name = "教师培训.专题讲座")]
		public double jspx { get; set; }
		[Display(Name = "个人特色.专著")]
		public double zz { get; set; }
		[Display(Name = "个人特色.其他身份")]
		public double qtsf { get; set; }
		[Display(Name = "个人特色.学员成长")]
		public double xycz { get; set; }
	}

}
