using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class TeamEvalOverviewModels
	{

		public long PeriodId { get; set; }
		public long TeamId { get; set; }
		public string TeamName { get; set; }
		public int TeamMemberCount { get; set; }
		public int TeamEvalTargetMemberCount { get; set; }
		public EvalStatus EvalStatus { get; set; }

	}


	public class TeamEvalResultExportModels
	{

		public long ResultId { get; set; }

		[Display(Name ="教师姓名")]
		public string TeacherName { get; set; }

		[Display(Name = "团队名称")]
		public string TeamName { get; set; }

		[Display(Name = "称号")]
		public string TargetName { get; set; }

		[Display(Name = "评审日期")]
		public string AccessDate { get; set; }

		[Display(Name = "得分")]
		public string Score { get; set; }

		[Display(Name ="考核项目")]
		public string EvalTitle { get; set; }

		[Display(Name = "考核评价")]
		public string EvalComment { get; set; }
	}


}
