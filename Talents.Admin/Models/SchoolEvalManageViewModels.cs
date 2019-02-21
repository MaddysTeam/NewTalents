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

}
