using Business.Config;
using Symber.Web.Report;
using System;

namespace Business.Helper
{

   public static class DeclareBaseHelper
   {

      static APDBDef.DeclareBaseTableDef t = APDBDef.DeclareBase;

      public static IDAPRptColumn TeacherId { get; } = new IDAPRptColumn(t.TeacherId);
      public static PicklistAPRptColumn DeclareTarget { get; } = new PicklistAPRptColumn(t.DeclareTargetPKID, PicklistKeys.DeclareTarget);
      public static PicklistAPRptColumn DeclareSubject { get; } = new PicklistAPRptColumn(t.DeclareSubjectPKID, PicklistKeys.DeclareSubject);
      public static PicklistAPRptColumn DeclareStage { get; } = new PicklistAPRptColumn(t.DeclareStagePKID, PicklistKeys.DeclareStage);
      public static CheckAPRptColumn AllowFlowToSchool { get; } = new CheckAPRptColumn(t.AllowFlowToSchool);
      public static CheckAPRptColumn AllowFitResearcher { get; } = new CheckAPRptColumn(t.AllowFitResearcher);
      public static CheckAPRptColumn HasTeam { get; } = new CheckAPRptColumn(t.HasTeam);
      public static TextAPRptColumn TeamName { get; } = new TextAPRptColumn(t.TeamName);
      public static Int32APRptColumn MemberCount { get; } = new Int32APRptColumn(t.MemberCount);
      public static Int32APRptColumn ActiveCount { get; } = new Int32APRptColumn(t.ActiveCount);

      public static APRptColumnCollection Columns = new APRptColumnCollection
      {
      };

   }


   public static class DeclareMaterialHelper
   {

      static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;


      public static void AddDeclareMaterial(DeclareContent content, DeclarePeriod period, APDBDef db)
      {
         if (content != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == content.DeclareContentId & dm.PeriodId == period.PeriodId);
            if (content.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = content.DeclareContentId,
                  ParentType = "DeclareContent",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = content.ContentKey == DeclareKeys.NiandZongj_Dien || content.ContentKey == DeclareKeys.NiandZongj_Disn || content.ContentKey == DeclareKeys.NiandZongj_Diyn ?
                            content.ContentKey :
                            SubString(content.ContentValue),
                  Type = content.ContentKey,
                  TeacherId = content.TeacherId,
                  PeriodId = period.PeriodId
               });
         }
      }


      public static void AddDeclareMaterial(DeclareActive active, DeclarePeriod period, APDBDef db, long declareTargetId = 0)
      {
         if (active != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == active.DeclareActiveId & dm.PeriodId == period.PeriodId);
            if (active.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = active.DeclareActiveId,
                  ParentType = "DeclareActive",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = active.ContentValue,
                  Type = active.ActiveKey,
                  TeacherId = active.TeacherId,
                  PeriodId = period.PeriodId,
                  DeclareTargetPKID= declareTargetId
               });
         }
      }


      public static void AddDeclareMaterial(DeclareAchievement achievement, DeclarePeriod period, APDBDef db, long declareTargetId = 0)
      {
         if (achievement != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == achievement.DeclareAchievementId & dm.PeriodId == period.PeriodId);
            if (achievement.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = achievement.DeclareAchievementId,
                  ParentType = "DeclareAchievement",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = achievement.NameOrTitle,
                  Type = achievement.AchievementKey,
                  TeacherId = achievement.TeacherId,
                  PeriodId = period.PeriodId,
                  DeclareTargetPKID= declareTargetId
               });
         }
      }


      public static void AddDeclareMaterial(DeclareResume resume, DeclarePeriod period, APDBDef db)
      {
         if (resume != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == resume.DeclareResumeId & dm.PeriodId == period.PeriodId);
            if (resume.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = resume.DeclareResumeId,
                  ParentType = "DeclareResume",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = resume.Title,
                  Type = DeclareKeys.ZisFaz_GerJianl,
                  TeacherId = resume.TeacherId,
                  PeriodId = period.PeriodId
               });
         }
      }


      public static void AddDeclareMaterial(DeclareOrgConst org, DeclarePeriod period, APDBDef db)
      {
         if (org != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == org.DeclareOrgConstId & dm.PeriodId == period.PeriodId);
            if (org.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = org.DeclareOrgConstId,
                  ParentType = "DeclareOrgConst",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = org.Content,
                  Type = DeclareKeys.ZhidJians_DangaJians,
                  TeacherId = org.TeacherId,
                  PeriodId = period.PeriodId
               });
         }
      }


      public static void AddDeclareMaterial(TeamActive teamActive, DeclarePeriod period, APDBDef db)
      {
         if (teamActive != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == teamActive.TeamActiveId & dm.PeriodId == period.PeriodId);
            if (teamActive.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = teamActive.TeamActiveId,
                  ParentType = "DeclareTeamActive",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = teamActive.Title,
                  Type = PicklistHelper.TeamActiveType.GetName(teamActive.ActiveType),
                  TeacherId = teamActive.TeamId,
                  PeriodId = period.PeriodId
               });
         }
      }


      public static void AddDeclareMaterial(TeamSpecialCourse specialCourse, DeclarePeriod period, APDBDef db)
      {
         if (specialCourse != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == specialCourse.CourseId & dm.PeriodId == period.PeriodId);
            if (specialCourse.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = specialCourse.CourseId,
                  ParentType = "DeclareTeamSpecialCourse",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = specialCourse.Title,
                  Type = TeamKeys.KecShis_Chak,
                  TeacherId = specialCourse.TeamId,
                  PeriodId = period.PeriodId
               });
         }
      }


      public static void AddDeclareMaterial(TeamContent content, DeclarePeriod period, APDBDef db)
      {
         if (content != null && period != null)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == content.TeamContentId & dm.PeriodId == period.PeriodId);
            if (content.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = content.TeamContentId,
                  ParentType = "TeamContent",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = content.ContentKey == TeamKeys.DaijJih_Memo2 ? content.ContentKey : content.ContentValue,
                  Type = content.ContentKey,
                  TeacherId = content.TeamId,
                  PeriodId = period.PeriodId
               });
         }
      }

      private static string SubString(string str)
         => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

   }

}

