using System;

namespace TheSite.EvalAnalysis
{

	public abstract class EvalParam
	{
		public long PeriodId { get; set; }
		public long TeacherId { get; set; }
		public long AccesserId { get; set; }
	}


	public class SchoolEvalParam : EvalParam
	{

		public long SchoolId { get; set; }

	}


	public class VolumnEvalParam : EvalParam
	{
	}


	public class QualityEvalParam : EvalParam
	{
		public long GroupId { get; set; }
		[Obsolete("this property will be discard in 2019")]
		public long TargetId { get; set; }
		public long ResultId { get; set; }

	}


	public class DeclareEvalParam : EvalParam
	{
		public long GroupId { get; set; }
		public long TargetId { get; set; }
		public long ResultId { get; set; }
	}


	public class TeamEvalParam : EvalParam
	{
		public long TeamId { get; set; }
	}

}