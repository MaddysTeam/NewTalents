using Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheSite.Models
{
	public class UserInfoModel
	{
		/// <summary>
		/// 民族
		/// </summary>
		[Display(Name = "民族")]
		public string Nationality { get; set; }

		/// <summary>
		/// 政治面貌
		/// </summary>
		[Display(Name = "政治面貌")]
		public string PoliticalStatus { get; set; }

		/// <summary>
		/// 职称通过年月
		/// </summary>
		[Display(Name = "职称通过年月")]
		public DateTime? SkillDate { get; set; }

		/// <summary>
		/// 毕业院校与专业
		/// </summary>
		[Display(Name = "毕业院校与专业")]
		public string GraduateSchool { get; set; }

		/// <summary>
		/// 行政职务
		/// </summary>
		[Display(Name = "行政职务")]
		public string RankTitle { get; set; }

		/// <summary>
		/// 师训编号
		/// </summary>
		[Display(Name = "师训编号")]
		public string TrainNo { get; set; }


		/// <summary>
		/// 手机号码
		/// </summary>
		[Display(Name = "手机号码")]
		public string Phonemobile { get; set; }

		/// <summary>
		/// 电子邮箱
		/// </summary>
		[Display(Name = "电子邮箱")]
		public string Email { get; set; }

		/// <summary>
		/// 学员数量
		/// </summary>
		[Display(Name = "学员数量")]
		public int MemberCount { get; set; }

		/// <summary>
		/// 评论总得分
		/// </summary>
		[Display(Name = "评论总得分")]
		public double TotalScore { get; set; }

		/// <summary>
		/// 校评得分
		/// </summary>
		[Display(Name = "校评得分")]
		public double SchoolScore { get; set; }

		/// <summary>
		/// 量评得分
		/// </summary>
		[Display(Name = "量评得分")]
		public double VolumnScore { get; set; }

		/// <summary>
		/// 质评得分
		/// </summary>
		[Display(Name = "质评得分")]
		public double QualityScore { get; set; }

		/// <summary>
		/// 特色得分
		/// </summary>
		[Display(Name = "特色得分")]
		public double ChaScore { get; set; }

		/// <summary>
		/// 荣誉
		/// </summary>
		[Display(Name = "荣誉")]
		public string Honor { get; set; }
	}
}