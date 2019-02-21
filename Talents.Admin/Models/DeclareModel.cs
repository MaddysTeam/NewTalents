using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class DeclareModel
	{

		public long TeacherId { get; set; }
		public string RealName { get; set; }
		public long TargetId { get; set; }
		public string Target { get; set; }
		public string Subject { get; set; }
		public string Stage { get; set; }
		public string CompanyName { get; set; }

	}

}