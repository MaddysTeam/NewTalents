using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class SchoolEvalOverviewModel
	{

		public long CompanyId { get; set; }
		public long PeriodId { get; set; }
		public string CompanyName { get; set; }
		public int TotalMemberCount { get; set; }
		public int EvalMemberCount { get; set; }
		public EvalStatus EvalStatus { get; set;}

	}

	public enum EvalStatus
	{

		Success=1,
		Pending=2,
		NotStart=3

	}

   public class SchoolEvalResultExportModels
   {

      public long ResultId { get; set; }

      [Display(Name = "教师姓名")]
      public string TeacherName { get; set; }

      [Display(Name = "学校名称")]
      public string Company { get; set; }

      [Display(Name = "称号")]
      public string TargetName { get; set; }

      [Display(Name = "评审日期")]
      public string AccessDate { get; set; }

      [Display(Name = "得分")]
      public string Score { get; set; }
   }

}
