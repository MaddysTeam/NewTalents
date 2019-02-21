using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	/// <summary>
	/// 年度评价算法
	/// </summary>
	public partial class AnnualEngine : EvalAnalysisEngine
	{

		protected const string ViewPath = "../EvalModel/Annual";


		public override string AnalysisName
			=> "年度评价量表 Ver 1.0";


		public override string DevelopKey
			=> "Annual_Ver_1.0";

        public override double FullScore
            => 105;


        public override SchoolEvalUnitBase SchoolEval { get; }
			= new SchoolEvalUnit
			{
				ViewPath = ViewPath
			};


		public override IReadOnlyDictionary<long, VolumnEvalUnitBase> VolumnEvals { get; }
			= new List<VolumnEvalUnitBase>
			{
				new VolumnEvalUnit5002 { ViewPath = ViewPath },
				new VolumnEvalUnit5003 { ViewPath = ViewPath },
				new VolumnEvalUnit5004 { ViewPath = ViewPath },
				new VolumnEvalUnit5005 { ViewPath = ViewPath },
				new VolumnEvalUnit5006 { ViewPath = ViewPath }
			}.ToDictionary(m => m.TargetId);


		public override IReadOnlyDictionary<long, QualityEvalUnitBase> QualityEvals { get; }
		= new List<QualityEvalUnitBase>
		{
				new QualityEvalUnit5002 { ViewPath = ViewPath },
				new QualityEvalUnit5003 { ViewPath = ViewPath },
				new QualityEvalUnit5004 { ViewPath = ViewPath },
				new QualityEvalUnit5005 { ViewPath = ViewPath },
				new QualityEvalUnit5006 { ViewPath = ViewPath },
		}.ToDictionary(m => m.TargetId);

	}

}