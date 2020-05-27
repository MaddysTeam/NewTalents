using System.Collections.Generic;

namespace TheSite.EvalAnalysis
{

	/// <summary>
	/// 评价算法规则基础类
	/// </summary>
	public abstract class EvalAnalysisEngine
	{


		/// <summary>
		/// 算法名称
		/// </summary>
		public abstract string AnalysisName { get; }


		/// <summary>
		/// 算法键值
		/// </summary>
		public abstract string DevelopKey { get; }


		/// <summary>
		/// 算法满分
		/// </summary>
		public abstract double FullScore { get; }


		/// <summary>
		/// 校评算法模块
		/// </summary>
		public virtual SchoolEvalUnitBase SchoolEval { get; }


		/// <summary>
		/// 量评算法模块
		/// </summary>
		public virtual IReadOnlyDictionary<long, VolumnEvalUnitBase> VolumnEvals { get; }


		/// <summary>
		/// 质评算法模块
		/// </summary>
		public virtual IReadOnlyDictionary<long, QualityEvalUnitBase> QualityEvals { get; }


		/// <summary>
		/// 称号评分算法模块
		/// </summary>
		public virtual IReadOnlyDictionary<long, DeclareEvalUnitBase> DeclareEvals { get; }


		/// <summary>
		/// 团队考核算法模块
		/// </summary>
		public virtual IReadOnlyDictionary<long,TeamEvalUnitBase> TeamEvals { get; }

	}

}