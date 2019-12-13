using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class EvalCommentAndScoreViewModel
	{

		public long EvalResultId { get; set; }
		public string ExpertName { get; set; }
		public string EvalTitle { get; set; }
		public string EvalComment { get; set; }
		public double Score { get; set; }
		public double FullScore { get; set; }

	}

}