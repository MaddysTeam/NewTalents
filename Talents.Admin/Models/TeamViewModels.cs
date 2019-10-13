using Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class TeamMemberModel
	{

		public long TeacherId { get; set; }
		public string RealName { get; set; }
		public string Target { get; set; }
		public string Subject { get; set; }
		public string Stage { get; set; }
		public string CompanyName { get; set; }
		public string ContentValue { get; set; }

	}

	public class DaijJihModel
	{

		[Display(Name = "培养目标")]
		public string Memo1 { get; set; }

		[Required]
		[Display(Name = "具体计划")]
		public string Memo2 { get; set; }

		[Display(Name = "带教小结")]
		public string Memo3 { get; set; }

		[Display(Name = "是否申报")]
		public bool IsDeclare1 { get; set; }

		[Display(Name = "是否申报")]
		public bool IsDeclare2 { get; set; }

		[Display(Name = "是否申报")]
		public bool IsDeclare3 { get; set; }

	}


	public class TeamSpecialCourseViewModel : TeamSpecialCourse
	{

		public int ItemCount { get; set; }

	}


	public class MemberResultViewModel : TeamActiveResult
	{

		public long TeamId { get; set; }
		public string MemberName { get; set; }

	}


	public class TeamActiveResultViewModel : TeamActiveResult
	{

		public string MemberName { get; set; }

	}


	public class TeamActiveItemViewModel : TeamActiveItem
	{

		public long TeamId { get; set; }
		public string MemberName { get; set; }

	}


	public class TeamActiveViewModel : TeamActive
	{
		public string TypeName { get; set; }

		public List<TeamActiveItemViewModel> Item { get; set; }
		public List<TeamActiveResultViewModel> Result { get; set; }

		public TeamActiveViewModel()
		{
			Item = new List<TeamActiveItemViewModel>();
			Result = new List<TeamActiveResultViewModel>();
		}

	}


	public class TeamSpecialCourseModel : TeamSpecialCourse
	{
		public List<TeamSpecialCourseItem> Item { get; set; }

		public TeamSpecialCourseModel()
		{
			Item = new List<TeamSpecialCourseItem>();
		}
	}


	public class TeamActiveModel : TeamActive
	{

		public int Count { get; set; }
		public int MemberCount { get; set; }

	}


	public class TeamActiveDataModel
	{
		[Display(Name = "ID")]
		public long TeamActiveId { get; set; }

		[Display(Name = "梯队ID")]
		public long TeamId { get; set; }

		[Display(Name = "时间")]
		public DateTime Date { get; set; }

		[Display(Name = "类型")]
		public long ActiveType { get; set; }

		[Display(Name = "标题")]
		[Required()]
		[StringLength(100)]
		public string Title { get; set; }

		[Display(Name = "地点")]
		[Required()]
		[StringLength(100)]
		public string Location { get; set; }

		[Display(Name = "是否显示")]
		public bool IsShow { get; set; }

		[Display(Name = "内容")]
		[Required()]
		public string ContentValue { get; set; }

		[Display(Name = "附件名称")]
		[Required()]
		[StringLength(40)]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		[StringLength(255)]
		public string AttachmentUrl { get; set; }

		[Display(Name = "是否共享")]
		public bool IsShare { get; set; }

		[Display(Name = "是否申报")]
		public bool IsDeclare { get; set; }
	}


	public class TeamJutJihModel
	{
		[Display(Name = "附件名称")]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl { get; set; }

		[Display(Name = "具体计划")]
		[Required()]
		public string ContentValue { get; set; }

		public long TeamContentId { get; set; }

		public bool IsDeclare { get; set; }
	}


	public class TeamDaijXiaojModel
	{
		[Display(Name = "附件名称")]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl { get; set; }

		[Display(Name = "带教小结")]
		[Required()]
		public string ContentValue { get; set; }

		public long TeamContentId { get; set; }

		public bool IsDeclare { get; set; }
	}


	public class TeamMemberDataModel : TeamMember
	{
		[Display(Name = "附件名称")]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl { get; set; }
	}

	#region [  制度建设  201910 ]

	public class ZhidJiansViewModel
	{
		[Display(Name = "团队制度")]
		public string AttachmentName1 { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl1 { get; set; }

		[Display(Name = "三年规划")]
		public string AttachmentName2 { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl2 { get; set; }

		[Display(Name = "第一学期计划")]
		public string SemesterAttachmentName1 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl1 { get; set; }

		[Display(Name = "第二学期计划")]
		public string SemesterAttachmentName2 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl2 { get; set; }

		[Display(Name = "第三学期计划")]
		public string SemesterAttachmentName3 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl3 { get; set; }

		[Display(Name = "第四学期计划")]
		public string SemesterAttachmentName4 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl4 { get; set; }

		[Display(Name = "第五学期计划")]
		public string SemesterAttachmentName5 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl5 { get; set; }

		[Display(Name = "第六学期计划")]
		public string SemesterAttachmentName6 { get; set; }

		[Display(Name = "附件路径")]
		public string SemesterAttachmentUrl6 { get; set; }

	}

	#endregion

	#region [  团队项目  201910 ]

	public class TuandXiangmViewModel
	{
      public long Id { get; set; }

      public long TeamId { get; set; }

		[Display(Name = "项目名称")]
      [Required()]
		public string Name { get; set; }

		[Display(Name = "申报人")]
      [Required()]
      public string UserName { get; set; }

		[Display(Name = "申报人单位")]
      [Required()]
      public string Company { get; set; }

		[Display(Name = "填表日期")]
      [Required()]
      public DateTime Date { get; set; }

		[Display(Name = "项目开题")]
      public string AttachmentName1 { get; set; }
		public string AttachmentUrl1 { get; set; }

		[Display(Name = "项目中期")]
      public string AttachmentName2 { get; set; }
		public string AttachmentUrl2 { get; set; }

		[Display(Name = "项目结题")]
      public string AttachmentName3 { get; set; }
		public string AttachmentUrl3 { get; set; }
	}

   #endregion

   #region [  团队个人计划  201910 ]

   public class TeamGerJihViewModel
   {
      [Display(Name = "第一年计划")]
      public string AttachmentName1 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl1 { get; set; }

      [Display(Name = "第二年计划")]
      public string AttachmentName2 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl2 { get; set; }

      [Display(Name = "第三年计划")]
      public string AttachmentName3 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl3 { get; set; }
   }

   #endregion
}