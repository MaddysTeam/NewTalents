using System;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class TeacherEvalInfo
	{
		/// <summary>
		/// 所在单位
		/// </summary>
		public string CompanyName { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string RealName { get; set; }

		/// <summary>
		/// 性别
		/// </summary>
		public string Gender { get; set; }

		/// <summary>
		/// 出生年月
		/// </summary>
		public DateTime Birthday { get; set; }

		/// <summary>
		/// 学历
		/// </summary>
		public string EduBg { get; set; }

		/// <summary>
		/// 称号
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// 现任专职
		/// </summary>
		public string SkillTitle { get; set; }

		/// <summary>
		/// 申报学科
		/// </summary>
		public string DeclareSubject { get; set; }

		/// <summary>
		/// 任教学科
		/// </summary>
		public string EduSubject { get; set; }

		/// <summary>
		/// 任教学段
		/// </summary>
		public string EduStage { get; set; }

		/// <summary>
		/// 学位
		/// </summary>
		public string EduDegree { get; set; }

		/// <summary>
		/// 职称通过年月日
		/// </summary>
		public DateTime SkillDate { get; set; }

		/// <summary>
		/// 政治面貌
		/// </summary>
		public string PoliticalStatus { get; set; }

		/// <summary>
		/// 民族
		/// </summary>
		public string Nationality { get; set; }

		/// <summary>
		/// 行政职务
		/// </summary>
		public string RankTitle { get; set; }

		/// <summary>
		/// 毕业院校与专业
		/// </summary>
		public string GraduateSchool { get; set; }

		/// <summary>
		/// 师训编号
		/// </summary>
		public string TrainNo { get; set; }

		/// <summary>
		/// 手机
		/// </summary>
		public string Phonemobile { get; set; }

		/// <summary>
		/// 电子邮箱
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 学员数量
		/// </summary>
		public int MemberCount { get; set; }

		/// <summary>
		/// 总得分
		/// </summary>
		public double TotalScore { get; set; }

		/// <summary>
		/// 质评得分
		/// </summary>
		public double QualityScore { get; set; }

		/// <summary>
		/// 量评得分
		/// </summary>
		public double VolumnScore { get; set; }

		/// <summary>
		/// 校评得分
		/// </summary>
		public double SchoolScore { get; set; }

		/// <summary>
		/// 特色得分
		/// </summary>
		public double CharacteristicScore { get; set; }

		/// <summary>
		/// 荣誉
		/// </summary>
		public string Honor { get; set; }

	}

}