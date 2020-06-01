using System.Collections.Generic;
using System.Linq;

namespace TheSite.EvalAnalysis
{

	/// <summary>
	/// 年度评价算法
	/// </summary>
	public partial class AnnualEngine : EvalAnalysisEngine
	{

		protected const string ViewPath = "../EvalModel/Annual";


		public override string AnalysisName
		   => "年度评价量表 Ver 2.0";


		public override string DevelopKey
		   => "Annual_Ver_2.0";


		// only for 2019
		public override double FullScore
			=> 100;

		public override SchoolEvalUnitBase SchoolEval { get; }
		 = new SchoolEvalUnit
		 {
			 ViewPath = ViewPath
		 };


		public override IReadOnlyDictionary<long, QualityEvalUnitBase> QualityEvals { get; }
		= new List<QualityEvalUnitBase>
		{
			new QualityEvalUnit5002 {ViewPath = ViewPath },
			new QualityEvalUnit5003 {ViewPath = ViewPath },
			new QualityEvalUnit5004 {ViewPath = ViewPath },
			new QualityEvalUnit5005 {ViewPath = ViewPath },
			new QualityEvalUnit5006 {ViewPath = ViewPath }
		}.ToDictionary(m => m.TargetId);



		public override IReadOnlyDictionary<long, TeamEvalUnitBase> TeamEvals { get; }
		= new List<TeamEvalUnitBase>
		{
			new TeamEvalUnit { ViewPath=ViewPath },
			new TeamEvalUnitSpecial { ViewPath=ViewPath }
		}.ToDictionary(m => m.TargetId);

	}


}