namespace TheSite.Models
{
	public class FindExpGroupModel
	{
		public long expGroupId { get; set; }
		public string expGroupName { get; set; }
	}

	public class AdjustExpMemberModel
	{
		public long id { get; set; }
		public string realName { get; set; }
		public string target { get; set; }
		public string subject { get; set; }
		public string stage { get; set; }
		public bool isLeader { get; set; }
	}

	public class MemberGroupModel
	{
		public long expGroupId { get; set; }
		public string expGroupName { get; set; }
		public string realName { get; set; }
	}

	
}
