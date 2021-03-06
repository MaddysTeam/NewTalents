﻿using Business;
using System;
using System.Collections.Generic;

namespace TheSite.EvalAnalysis
{

	public abstract class AnalysisUnit
	{

		/// <summary>
		/// 视图路径
		/// </summary>
		public virtual string ViewPath { get; set; }


		/// <summary>
		/// 量表视图名称
		/// </summary>
		public virtual string RuleView { get; }


		/// <summary>
		/// 考评视图名称
		/// </summary>
		public abstract string EvalView { get; }


		/// <summary>
		/// 结果视图名称
		/// </summary>
		public abstract string ResultView { get; }


		/// <summary>
		/// 满分
		/// </summary>
		public abstract double FullScroe { get; }

		/// <summary>
		/// 占比系数 
		/// </summary>
		public abstract double Proportion { get; }

	}


	public abstract class SchoolEvalUnitBase : AnalysisUnit
	{

		public override string RuleView
		   => ViewPath + "/SchoolRuleView";


		public override string EvalView
		   => ViewPath + "/SchoolEvalView";


		public override string ResultView
		   => ViewPath + "/SchoolResultView";


		public abstract EvalSchoolResult GetResult(APDBDef db, SchoolEvalParam param);


		public abstract Dictionary<string, EvalSchoolResultItem> GetResultItem(APDBDef db, SchoolEvalParam param);


		public abstract Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalSchoolResultItem> items);


		public abstract long Eval(APDBDef db, SchoolEvalParam param, System.Web.Mvc.FormCollection fc);

	}


	public abstract class VolumnEvalUnitBase : AnalysisUnit
	{

		public abstract long TargetId { get; }


		public abstract EvalVolumnResult GetResult(APDBDef db, VolumnEvalParam param);


		public abstract Dictionary<string, EvalVolumnResultItem> GetResultItem(APDBDef db, VolumnEvalParam param);


		public abstract Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalVolumnResultItem> items);


		public abstract void Eval(APDBDef db, VolumnEvalParam param, params long[] teacherIds);


		public abstract EvalVolumnResult AnalysisContent(APDBDef db, VolumnEvalParam param, long teacherId);

	}


	public abstract class QualityEvalUnitBase : AnalysisUnit
	{

		public virtual long TargetId { get; }

		public virtual string SubmitResultView { get; }

		public abstract EvalQualitySubmitResult GetSubmitResult(APDBDef db, QualityEvalParam param);
		public abstract List<EvalQualityResult> GetResults(APDBDef db, QualityEvalParam param);

		public abstract EvalQualityResult GetResult(APDBDef db, QualityEvalParam param);

		public abstract Dictionary<string, EvalQualityResultItem> GetResultItem(APDBDef db, QualityEvalParam param);

		public abstract Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalQualityResultItem> items);

		public abstract long Eval(APDBDef db, QualityEvalParam param, System.Web.Mvc.FormCollection fc);

	}


	public abstract class DeclareEvalUnitBase : AnalysisUnit
	{

		public abstract long TargetId { get; }

		public virtual double CompanyFullScore => 20;

		public virtual double ExpertFullScore => 70;

		public virtual double SpecialFullScore => 10;

		public abstract List<EvalDeclareResult> GetResults(APDBDef db, DeclareEvalParam param);

		public abstract EvalDeclareResult GetResult(APDBDef db, DeclareEvalParam param);

		public abstract Dictionary<string, EvalDeclareResultItem> GetResultItem(APDBDef db, DeclareEvalParam param);

		public abstract Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalDeclareResultItem> items);

		public abstract long Eval(APDBDef db, DeclareEvalParam param, System.Web.Mvc.FormCollection fc);

	}


	public abstract class TeamEvalUnitBase : AnalysisUnit
	{
		public abstract long TargetId { get; }

		//public abstract string EvalView { get; }// => ViewPath + "/TeamEvalView";

		public override string ResultView => ViewPath + "/TeamResultView";

		public override string RuleView => string.Empty;

		public abstract TeamEvalResult GetResult(APDBDef db, TeamEvalParam param);

		public abstract Dictionary<string, TeamEvalResultItem> GetResultItem(APDBDef db, TeamEvalParam param);

		public abstract long Eval(APDBDef db, TeamEvalParam param, System.Web.Mvc.FormCollection fc);

	}

}