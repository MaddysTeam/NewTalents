namespace TheSite.Models
{

	public class LeaderTeamViewModel
	{

		public string Title { get; set; }
		public long ResultCount { get; set; }
		public long MemberCount { get; set; }
		public double Progress { get { if (MemberCount == 0) { return 0; } else { return (ResultCount / MemberCount) * 100; } } }

	}

}
