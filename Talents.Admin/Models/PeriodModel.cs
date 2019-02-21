using Business;
using System.Collections.Generic;
using TheSite.EvalAnalysis;

namespace TheSite.Models
{

	public class PeriodModel
	{

		public long TeacherId { get; set; }
		public long PeriodId { get; set; }
		public long ResultId { get; set; }
		public EvalPeriod Period { get; set; }
		public DeclareModel Declare { get; set; }
		public string Message { get; set; }
		public AnalysisUnit AnalysisUnit { get; set; }

	}


	public class SchoolEvalPeriodModel : PeriodModel
	{
		public SchoolEvalPeriodModel(SchoolEvalParam param)
		{
			TeacherId = param.TeacherId;
			PeriodId = param.PeriodId;
		}


		public EvalSchoolResult Result { get; set; }
		public Dictionary<string, EvalSchoolResultItem> ResultItems { get; set; }


	}


	public class VolumnEvalPeriodModel : PeriodModel
	{
		public VolumnEvalPeriodModel(VolumnEvalParam param)
		{
			TeacherId = param.TeacherId;
			PeriodId = param.PeriodId;
		}

		public EvalVolumnResult Result { get; set; }
		public Dictionary<string, EvalVolumnResultItem> ResultItems { get; set; }

	}


	public class QualityEvalSubmitPeriodModel : PeriodModel
	{
		public QualityEvalSubmitPeriodModel(QualityEvalParam param)
		{
			TeacherId = param.TeacherId;
			PeriodId = param.PeriodId;
			ResultId = param.ResultId;
			GroupId = param.GroupId;
		}

		// GroupId 无实际用途，仅在显示结果页面时，用于判断是否允许加载对话框等信息
		public long GroupId { get; set; }
		public EvalQualitySubmitResult Result { get; set; }
		public List<EvalQualityResult> EvalResults { get; set; }
		public List<string> NotEvalExperts { get; set; }
		public bool IsLeader { get; set; }
		public bool IsSubmit => Result != null;
		public bool CanSubmit => (Result == null && (NotEvalExperts != null && NotEvalExperts.Count == 0));
		public EvalQualitySubmitResult DoSubmit { get; set; }

	}


	public class QualityEvalPeriodModel : PeriodModel
	{
		public QualityEvalPeriodModel(QualityEvalParam param)
		{
			TeacherId = param.TeacherId;
			PeriodId = param.PeriodId;
			ResultId = param.ResultId;
		}

		public EvalQualityResult Result { get; set; }
		public Dictionary<string, EvalQualityResultItem> ResultItems { get; set; }
		public Dictionary<string, string> ChooseItems { get; set; }

	}

}