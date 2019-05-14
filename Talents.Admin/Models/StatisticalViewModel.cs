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

	public class DeclareEvalStaisticalViewModel
	{
		public long targetId { get; set; }
		public long subjectId { get; set; }
		public long companyId { get; set; }

		public string teacher { get; set; }
		public string target { get; set; }
		public string subject { get; set; }
		public string company { get; set; }
		public double totalScore { get; set; }

		public double xnlz { get; set; }
		public string shd { get; set; }
		public double gkk { get; set; }
		public double jxpb { get; set; }
		public double qt { get; set; }
		public double mt { get; set; }
		public double pingw { get; set; }
		public double dey { get; set; }
		public double drpw2 { get; set; }
		public double xmyj { get; set; }
		public double fblw { get; set; }
		public double ztjz { get; set; }
		public double jspx { get; set; }
		public double zz { get; set; }
		public double qtsf { get; set; }
		public double xycz { get; set; }
	}

}