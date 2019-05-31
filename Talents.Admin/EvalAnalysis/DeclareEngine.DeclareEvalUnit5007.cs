using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	public partial class DeclareEngine
	{

		public class DeclareEvalUnit5007 : DeclareEvalUnit
		{

			public override long TargetId => 5007;

			public override double CompanyFullScore => 100;

			public override double ExpertFullScore => 0;

			public override double SpecialFullScore => 0;

			protected override void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items)
			{

			}

		}

	}

}