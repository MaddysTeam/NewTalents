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

}