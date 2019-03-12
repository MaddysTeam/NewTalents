using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

   public class DeclareModel
   {

      public long TeacherId { get; set; }
      public string RealName { get; set; }
      public long TargetId { get; set; }
      public string Target { get; set; }
      public string Subject { get; set; }
      public string Stage { get; set; }
      public string CompanyName { get; set; }
   }


   public class DeclareParam
   {
      public long DeclareTargetId { get; set; }
      public string TypeKey { get; set; }
      public string View { get; set; }
   }


   public class DeclarePreviewViewModel
   {
      public long DeclareTargetId { get; set; }
      public string Decalre { get; set; }
      public string RealName { get; set; }
      public string Subject { get; set; }
      public bool IsBrokRoles { get; set; }
      public string Gender { get; set; }
      public string Birthday { get; set; }
      public string Company { get; set; }
      public string Plitics { get; set; }
      public string Nation { get; set; }
      public string TrainNo { get; set; }
      public string CourseCount { get; set; }
      public string Mobile { get; set; }
      public string Phone { get; set; }
      public string Email { get; set; }
      public string SkillTitle { get; set; }
      public string RankTitle { get; set; }
      public string Hiredate { get; set; }
      public bool Is1000 { get; set; } //是否是“攻关计划”学员
      public bool Is2000 { get; set; } //是否是“种子计划”领衔人
      public bool Is3000 { get; set; } //上一轮是骨干教师
      public bool Is5006 { get; set; } //上一轮是教学能手
      public bool Is5007 { get; set; } //上一轮是教学新秀
      public bool Is5008 { get; set; } //上一轮是教学新秀
      public bool Is6000 { get; set; } //高端教师研修班学员
      public string Comment1 { get; set; }
      public string FirstYearScore { get; set; }
      public string SecondYearScore { get; set; }
      public string ThirdYearScore { get; set; }
      public string Reason { get; set; }
      public bool IsAllowdFlow { get; set; }
      public bool IsAllowDownGrade { get; set; }
      public Dictionary<string, List<DeclarePreviewItems>> Items { get; set; }

      public class DeclarePreviewItems
      {
         public string Dynamic1 { get; set; }
         public string Dynamic2 { get; set; }
         public string Dynamic3 { get; set; }
         public string Dynamic4 { get; set; }
         public string Dynamic5 { get; set; }
         public string Dynamic6 { get; set; }
         public string Dynamic7 { get; set; }
      }

   }
}