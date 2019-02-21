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

}