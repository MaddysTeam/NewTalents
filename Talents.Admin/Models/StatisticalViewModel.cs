using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class TempListViewModel
	{

		public string Name { get; set; }
		public long Count { get; set; }

	}

	public class MulTempListViewModel
	{

		public string Name { get; set; }
		public long Count { get; set; }
		public long EvalCount { get; set; }
		public long NotEvalCount { get; set; }

	}

	public class DeclareEvalStaisticalViewModel
	{
		public long teacherId { get; set; }
		public long targetId { get; set; }
		public long subjectId { get; set; }
		public long companyId { get; set; }

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
		[Display(Name = "校内履职")]
		public double xnlz { get; set; }
		[Display(Name = "师德")]
		public string shd { get; set; }
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