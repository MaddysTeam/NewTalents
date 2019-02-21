using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class AdminOverViewModel
	{

		public long TeacherCount { get; set; }
		public long MemberCount { get; set; }
		public long TeamCount { get; set; }
		public long ExpertsGroupCount { get; set; }
		public long ExpertsCount { get; set; }
		public long SchollCount { get; set; }

	}

}