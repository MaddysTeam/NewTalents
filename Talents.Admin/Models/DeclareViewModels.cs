using System;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

   public abstract class DeclareActiveModel
   {

      public long DeclareActiveId { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }

      [Display(Name = "是否共享")]
      public bool IsShare { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare { get; set; }

      [Display(Name = "证明文件路径")]
      public string VertificationUrl { get; set; }

      [Required]
      [Display(Name = "证明文件名称")]
      public string VertificationName { get; set; }

      // 申报的称号
      public long DeclareTargetId { get; set; }

   }


   public abstract class DeclareAchievementModel
   {

      [Display(Name = "是否共享")]
      public bool IsShare { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare { get; set; }

      [Display(Name = "证明文件路径")]
      public string VertificationUrl { get; set; }

      [Required]
      [Display(Name = "证明文件名称")]
      public string VertificationName { get; set; }

      // 申报的称号
      public long DeclareTargetId { get; set; }

   }


   public class ZisFaz_GerChengjModel
   {

      [Display(Name = "研究成果")]
      public string Memo1 { get; set; }

      [Display(Name = "曾经获得荣誉奖项")]
      public string Memo2 { get; set; }

      [Display(Name = "之后获得研究成果")]
      public string Memo3 { get; set; }

      public long ItemKey1 { get; set; }

      public bool IsDeclare1 { get; set; }

      public long ItemKey2 { get; set; }

      public bool IsDeclare2 { get; set; }

      public long ItemKey3 { get; set; }

      public bool IsDeclare3 { get; set; }


   }


   public class ZisFaz_GerSWOTModel
   {

      [Display(Name = "课堂教学类")]
      public string Goodness1 { get; set; }

      [Display(Name = "科研类")]
      public string Goodness2 { get; set; }

      [Display(Name = "技能类")]
      public string Goodness3 { get; set; }

      [Display(Name = "个性及其他")]
      public string Goodness4 { get; set; }

      [Display(Name = "课堂教学类")]
      public string Weakness1 { get; set; }

      [Display(Name = "科研类")]
      public string Weakness2 { get; set; }

      [Display(Name = "技能类")]
      public string Weakness3 { get; set; }

      [Display(Name = "个性及其他")]
      public string Weakness4 { get; set; }

      [Display(Name = "教研组")]
      public string Opportunity1 { get; set; }

      [Display(Name = "学校内部环境")]
      public string Opportunity2 { get; set; }

      [Display(Name = "政策制度")]
      public string Opportunity3 { get; set; }

      [Display(Name = "教研组")]
      public string Challenge1 { get; set; }

      [Display(Name = "学校内部环境")]
      public string Challenge2 { get; set; }

      [Display(Name = "政策制度")]
      public string Challenge3 { get; set; }

      [Display(Name = "完善、改进提高的措施、办法")]
      public string GaijCuos { get; set; }


      [Display(Name = "是否申报")]
      public bool IsDeclare1 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare2 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare3 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare4 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare5 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare6 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare7 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare8 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare9 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare10 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare11 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare12 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare13 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare14 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare15 { get; set; }

   }


   //public class ZisFaz_GongkKe
   //{
   //   public DateTime Date { get; set; }

   //   public string Location { get; set; }

   //   public string Class { get; set; }

   //   public string TitleOrContent { get; set; }

   //   public string Level { get; set; }

   //   public string Org { get; set; }
   //}


   public class ZisFaz_ZiwFazJihModel
   {

      [Display(Name = "总体目标")]
      public string ZhuanyeFazMub_Memo1 { get; set; }

      [Display(Name = "第一年目标")]
      public string JiedMub_Memo1 { get; set; }

      [Display(Name = "第二年目标")]
      public string JiedMub_Memo2 { get; set; }

      [Display(Name = "第三年目标")]
      public string JiedMub_Memo3 { get; set; }

      [Display(Name = "教学")]
      public string ZhuanyNenglFangm_Memo1 { get; set; }

      [Display(Name = "科研")]
      public string ZhuanyNenglFangm_Memo2 { get; set; }

      [Display(Name = "培训教师")]
      public string ZhuanyNenglFangm_Memo3 { get; set; }

      [Display(Name = "项目任务")]
      public string ZhuanyNenglFangm_Memo4 { get; set; }

      [Display(Name = "知识")]
      public string XuyFangm_Memo1 { get; set; }

      [Display(Name = "技能")]
      public string XuyFangm_Memo2 { get; set; }

      [Display(Name = "其他")]
      public string XuyFangm_Memo3 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare1 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare2 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare3 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare4 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare5 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare6 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare7 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare8 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare9 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare10 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare11 { get; set; }

   }


   public class ZisFaz_ZiwYanxModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "研究内容")]
      public string ContentValue { get; set; }

   }


   public class ZisFaz_JiaoxHuod_JiaoxGongkkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课程内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic2 { get; set; }

   }


   public class ZisFaz_JiaoxHuod_YantkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织部门、负责人")]
      public string Dynamic2 { get; set; }

   }


   public class ZisFaz_JiaoxHuod_JiaoxPingbModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点、年级班级")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "奖项名称、等第")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Dynamic2 { get; set; }

   }


   public class ZisFaz_PeixJiangzModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "课程开设时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "课题名称")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加人数")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Dynamic2 { get; set; }

   }


   public class ZisFaz_PeixJiangz_ZhuantJiangzModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "讲座主题")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "级别/范围")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic1 { get; set; }

   }


   public class ZisFaz_PeixJiangz_DingxxKecModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "负责人")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }
   }


   public class ZisFaz_XuesHuodModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "工作内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门/负责人")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "类型")]
      public long Dynamic9 { get; set; }

   }


   public class ZisFaz_KeyChengg_KetYanjModel : DeclareAchievementModel
   {

      public long DeclareAchievementId { get; set; }

      [Required]
      [Display(Name = "立项、结束时间")]
      public string DateRegion { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "课题（项目）名称")]
      public string NameOrTitle { get; set; }

      [Display(Name = "是否主持")]
      public bool Dynamic1 { get; set; }

      [Required]
      [Display(Name = "负责人")]
      public string Dynamic2 { get; set; }


      [Required]
      [Display(Name = "类型")]
      public long Dynamic6 { get; set; }


      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }

   }


   public class ZisFaz_KeyChengg_FabLunwModel : DeclareAchievementModel
   {

      public long DeclareAchievementId { get; set; }

      [Required]
      [Display(Name = "发表时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "论文名称")]
      public string NameOrTitle { get; set; }

      [Display(Name = "刊物名称")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "作者名次")]
      public string Dynamic2 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }

   }


   public class ZisFaz_KeyChengg_LunzQingkModel : DeclareAchievementModel
   {

      public long DeclareAchievementId { get; set; }

      [Required]
      [Display(Name = "出版时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "著作名称")]
      public string NameOrTitle { get; set; }

      [Display(Name = "出版社")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "独立完成")]
      public bool Dynamic2 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }

   }


   public class ZisFaz_ShiqjHuodModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门/负责人")]
      public string Dynamic2 { get; set; }

   }


   public class ZhidJians_YingxlDeGongzModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "工作内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "组织单位/负责人")]
      public string Dynamic1 { get; set; }

   }


   public class ZhidJians_TesHuodKaizModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "合作单位、部门/负责人")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "类型")]
      public long Dynamic9 { get; set; }
   }


   public class QunLiud_LiurXuexModel
   {

      [Display(Name = "学校名称")]
      public string XuexMingc { get; set; }

      [Display(Name = "教研组长")]
      public string JiaoyZuc { get; set; }

   }


   public class QunLiud_XuekDaitr_KetJiaox_GongkkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "上课地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_XuekDaitr_KetJiaox_SuitkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "上课地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_XuekDaitr_KetJiaox_TingKZhidModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "上课教师")]
      public string Dynamic2 { get; set; }

   }


   public class QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点、参加对象")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参讲老师")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "活动形式")]
      public string Dynamic2 { get; set; }

   }


   public class QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点、参加对象")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主讲")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "指导性意见、建议")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_XuekDaitr_JiaoyKey_JiedxZongjModel
   {

      [Display(Name = "阶段性总结")]
      public string Summary { get; set; }

   }


   public class QunLiud_XuekDaitr_DaijPeix_DaijDuixModel
   {

      [Display(Name = "带教对象")]
      public string Member { get; set; }

      [Display(Name = "带教学科")]
      public string Subject { get; set; }

      [Display(Name = "带教对象发展分析")]
      [StringLength(2000, ErrorMessage = "长度不能超过2000字符")]
      public string Analysis { get; set; }

      [Display(Name = "带教对方案概要")]
      [StringLength(2000, ErrorMessage = "长度不能超过2000字符")]
      public string Summary { get; set; }

   }


   public class QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "指导内容")]
      public string ContentValue { get; set; }

   }


   public class QunLiud_GugJiaos_RenjNianjBanjModel
   {

      [Required]
      [Display(Name = "任教班级")]
      public string Memo1 { get; set; }

      [Required]
      [Display(Name = "任教时间段")]
      public string Memo2 { get; set; }

      [Required]
      [Display(Name = "班主任")]
      public string Memo3 { get; set; }

      [Required]
      [Display(Name = "任教学科")]
      public string Memo4 { get; set; }

   }


   public class QunLiud_GugJiaos_GongkkModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

   }


   public class QunLiud_GugJiaos_TingkZhidModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "课题/内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "年级班级")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "上课教师")]
      public string Dynamic2 { get; set; }

   }


   public class QunLiud_GugJiaos_BeikzHuodModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点、参加对象")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "主题")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参讲教师")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "活动形式")]
      public string Dynamic2 { get; set; }

   }


   public class PeihJiaoyyGongz_JiaoyXinxModel
   {

      [Display(Name = "教研员姓名")]
      public string Memo1 { get; set; }

      [Display(Name = "教研学科")]
      public string Memo2 { get; set; }

      [Display(Name = "开始参与教研工作时间")]
      public DateTime Memo3 { get; set; }

      public string Key1 { get; set; }

      public string Key2 { get; set; }

      public string Key3 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare1 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare2 { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare3 { get; set; }

   }


   public class PeihJiaoyyGongz_XuekJiaoyModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "工作内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

   }


   public class PeihJiaoyyGongz_XuekMingtModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "工作成果(试卷名称)")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "期中/期末")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "年级")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "参加对象、人数")]
      public string Dynamic3 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic4 { get; set; }

   }


   public class PeihJiaoyyGongz_JicXuexTiaoyModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "调研学校")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "调研内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "调研情况总结")]
      [StringLength(2000, ErrorMessage = "长度不能超过2000字符")]
      public string Dynamic1 { get; set; }

   }


   public class NiandZongjModel
   {
      [Display(Name = "年底总结")]
      public string Summary { get; set; }

      public string Key { get; set; }

      [Display(Name = "是否申报")]
      public bool IsDeclare { get; set; }
   }


   public class HistoryDeclareItemModel
   {
      public long Id { get; set; }
      public string Dynamic1 { get; set; }
      public string Dynamic2 { get; set; }
      public string Dynamic3 { get; set; }
      public string Dynamic4 { get; set; }
      public string Dynamic5 { get; set; }
      public string Dynamic6 { get; set; }
      public string Dynamic7 { get; set; }
   }


   public class JiaoyJiaox_JiaoyHuod_FahZuoyModel : DeclareActiveModel
   {

      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "地点")]
      public string Location { get; set; }

      [Required]
      [Display(Name = "课程内容")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "参加对象")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic2 { get; set; }

   }


   public class GerTes_QitShenfModel:DeclareAchievementModel
   {

      public long DeclareAchievementId { get; set; }

      [Required]
      [Display(Name = "起止时间")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "身份名称")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "工作内容")]
      public string NameOrTitle { get; set; }

      [Required]
      [Display(Name = "赋予身份的单位部门")]
      public string Dynamic3 { get; set; }


      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }
   }


   public class GerTes_XueyChengzModel: DeclareAchievementModel
   {
      public long DeclareAchievementId { get; set; }

      [Required]
      [Display(Name = "学员名称")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "单位")]
      public string Dynamic2 { get; set; }

      [Required]
      [Display(Name = "成长情况")]
      public string NameOrTitle { get; set; }

      [Required]
      [Display(Name = "时间")]
      public string Date { get; set; }

      [Required]
      [Display(Name = "备用")]
      public string Dynamic3 { get; set; }

      [Display(Name = "附件路径")]
      public string AttachmentUrl { get; set; }

      [Required]
      [Display(Name = "附件名称")]
      public string AttachmentName { get; set; }
   }

   //其他综合性荣誉
   public class Qit_ZonghxingRongy:DeclareActiveModel
   {
      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "荣誉名称")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "组织单位、部门")]
      public string Dynamic1 { get; set; }

   }

   //基本功展示获奖情况
   public class Qit_JibGongZshiHuoj : DeclareActiveModel
   {
      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "奖项名称")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "奖项类别")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门/负责人")]
      public string Dynamic2 { get; set; }
   }

   public class Qit_JianxJiaosPingx : DeclareActiveModel
   {
      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set; }

      [Required]
      [Display(Name = "奖项名称")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "组织单位、部门/负责人")]
      public string Dynamic1 { get; set; }
   }

   public class Qit_JianxJiaosDasHuoj : DeclareActiveModel
   {
      [Required]
      [Display(Name = "时间")]
      public DateTime Date { get; set; }

      [Required]
      [Display(Name = "级别")]
      public string Level { get; set;}

      [Required]
      [Display(Name = "奖项名称")]
      public string ContentValue { get; set; }

      [Required]
      [Display(Name = "奖项等第")]
      public string Dynamic1 { get; set; }

      [Required]
      [Display(Name = "组织单位、部门/负责人")]
      public string Dynamic2 { get; set; }
   }


}