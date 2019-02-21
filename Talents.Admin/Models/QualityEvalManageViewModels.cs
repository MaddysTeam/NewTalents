namespace TheSite.Models
{

	public class QualityEvalOverviewModels
	{

		public long PeriodId { get; set; }
		public long GroupId { get; set; }
		public string GroupName { get; set; }
		public int GroupTargetMemberCount { get; set; }
		public int EvalTargetMemberCount { get; set; }
		public EvalStatus EvalStatus { get; set;}

	}

}
