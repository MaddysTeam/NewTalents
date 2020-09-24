using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class QualityEvalOverviewModels
	{

		public long PeriodId { get; set; }
		public long GroupId { get; set; }
		public string GroupName { get; set; }
		public int GroupTargetMemberCount { get; set; }
		public int EvalTargetMemberCount { get; set; }
		public EvalStatus EvalStatus { get; set; }
		public string Accessor { get; set; }

	}


	public class QualityEvalResultExportModels
	{

		public long ResultId { get; set; }

		[Display(Name = "教师姓名")]
		public string TeacherName { get; set; }

		[Display(Name = "团队名称")]
		public string TeamName { get; set; }

		[Display(Name = "称号")]
		public string TargetName { get; set; }

		[Display(Name ="评审专家")]
		public string Accessor { get; set; }

		[Display(Name = "评审日期")]
		public string AccessDate { get; set; }

      [Display(Name = "得分")]
      public string Score { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.SannGuih+"得分")]
      //public string EvalScore1 { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.SannGuih+"考核评价")]
      //public string EvalComment1 { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.TuandJih + "得分")]
      public string EvalScore2 { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.TuandJih+"考核评价")]
      public string EvalComment2 { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.GerJiHx+"得分")]
      public string EvalScore3 { get; set; }

      //[Display(Name = Business.Helper.EvalQualityRuleKeys.GerJiHx + "考核评价")]
      public string EvalComment3 { get; set; }

   }

}
