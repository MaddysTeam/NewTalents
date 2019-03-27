using Business.Helper;
using System.Collections.Generic;

namespace Business
{

   public partial class DeclareProfile
   {

      public string Gender => BzUserProfileHelper.Gender.GetName(GenderPKID);

      public string PoliticalStatus => BzUserProfileHelper.PoliticalStatus.GetName(PoliticalStatusPKID);

      public string Nationality => BzUserProfileHelper.Nationality.GetName(NationalityPKID);

      public string EduSubject => BzUserProfileHelper.EduSubject.GetName(EduSubjectPKID);

      public string EduStage => BzUserProfileHelper.EduStage.GetName(EduStagePKID);

      public string SkillTitle => BzUserProfileHelper.SkillTitle.GetName(SkillTitlePKID);

      public string RankTitle => BzUserProfileHelper.RankTitle.GetName(RankTitlePKID);

      public string EduBg => BzUserProfileHelper.EduBg.GetName(EduBgPKID);

      public string EduDegree => BzUserProfileHelper.EduDegree.GetName(EduDegreePKID);

      public bool IsExtLogined { get; set; }

      public bool IsDeclare { get; set; }

      public bool IsMaster { get; set; }

      public bool IsMember { get; set; }

      public bool IsExpert { get; set; }

      public bool IsSystemAdmin { get; set; }

      public bool IsSchoolAdmin { get; set; }

      public long TargetId { get; set; }

      public long[] PrevioursDeclareTargets { get; set; }


      public List<Target> GetAllTargets5002 =>
       new List<Target>
       {
                new Target {Name = DeclareKeys.GaofJihZhucRen, Id = DeclareTargetIds.GaofJihZhucRen  },
                new Target {Name = DeclareKeys.GonggJihZhucRen, Id =DeclareTargetIds.GonggJihZhucRen },
                new Target {Name = Target.Prefix+DeclareKeys.GaodLisz, Id =DeclareTargetIds.GaodLisz },
                new Target {Name = Target.Prefix+DeclareKeys.JidZhucr, Id =DeclareTargetIds.JidZhucr },
       };

      public List<Target> GetAllTargets5003 =>
       new List<Target>
         {
                new Target {Name = DeclareKeys.GaofJihZhucRen, Id = DeclareTargetIds.GaofJihZhucRen  },
                new Target {Name = DeclareKeys.GonggJihZhucRen, Id =DeclareTargetIds.GonggJihZhucRen },
                new Target {Name = Target.Prefix+DeclareKeys.GaodLisz, Id =DeclareTargetIds.GaodLisz },
                new Target {Name = Target.Prefix+DeclareKeys.JidZhucr, Id =DeclareTargetIds.JidZhucr },
         };

      public List<Target> GetAllTargets5004 =>
        new List<Target>
          {
                new Target {Name = DeclareKeys.GonggJihChengy, Id = DeclareTargetIds.GonggJihChengy  },
                new Target {Name = DeclareKeys.ZhongzJihLingxReng, Id =DeclareTargetIds.ZhongzJihLingxReng },
                new Target {Name = Target.Prefix+DeclareKeys.GongzsZhucr, Id =DeclareTargetIds.GongzsZhucr },
                new Target {Name = Target.Prefix+DeclareKeys.XuekDaitr, Id =DeclareTargetIds.XuekDaitr },
                new Target {Name = Target.Prefix+DeclareKeys.GugJiaos, Id =DeclareTargetIds.GugJiaos  },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs },
                new Target {Name = DeclareKeys.GaodJiaoSYanxBanXuey, Id =DeclareTargetIds.GaodJiaoSYanxBanXuey  },
          };

      public List<Target> GetAllTargets5005 =>
          new List<Target>
            {
                new Target {Name = DeclareKeys.GonggJihChengy, Id = DeclareTargetIds.GonggJihChengy  },
                new Target {Name = DeclareKeys.ZhongzJihLingxReng, Id =DeclareTargetIds.ZhongzJihLingxReng },
                new Target {Name = Target.Prefix+DeclareKeys.GongzsZhucr, Id =DeclareTargetIds.GongzsZhucr },
                new Target {Name = Target.Prefix+DeclareKeys.XuekDaitr, Id =DeclareTargetIds.XuekDaitr },
                new Target {Name = Target.Prefix+DeclareKeys.GugJiaos, Id =DeclareTargetIds.GugJiaos  },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs },
                new Target {Name = DeclareKeys.GaodJiaoSYanxBanXuey, Id =DeclareTargetIds.GaodJiaoSYanxBanXuey  },
            };

      public List<Target> GetAllTargets5006 =>
          new List<Target>
            {
                new Target {Name = DeclareKeys.GonggJihChengy, Id = DeclareTargetIds.GonggJihChengy },
                new Target {Name = DeclareKeys.ZhongzJihLingxReng, Id =DeclareTargetIds.ZhongzJihLingxReng },
                new Target {Name = DeclareKeys.ZhongzJihChengy, Id =DeclareTargetIds.ZhongzJihChengy },
                new Target {Name = Target.Prefix+DeclareKeys.GugJiaos, Id = DeclareTargetIds.GugJiaos },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxXinx, Id =DeclareTargetIds.JiaoxXinx },
                new Target {Name = DeclareKeys.GaodJiaoSYanxBanXuey, Id =DeclareTargetIds.GaodJiaoSYanxBanXuey },
            };

      public List<Target> GetAllTargets5007 =>
          new List<Target>
            {
                new Target {Name = DeclareKeys.ZhongzJihChengy, Id =DeclareTargetIds.ZhongzJihChengy },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxXinx, Id =DeclareTargetIds.JiaoxXinx }
            };

      public List<Target> GetAllTargets5008 =>
         new List<Target>
           {
                new Target {Name = DeclareKeys.ZhongzJihChengy, Id =DeclareTargetIds.ZhongzJihChengy },
                new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs }
           };

      public List<Target> GetAllTargets5999 =>
      new List<Target>
        {
               new Target {Name = DeclareKeys.GonggJihChengy, Id = DeclareTargetIds.GonggJihChengy  },
               new Target {Name = DeclareKeys.ZhongzJihLingxReng, Id =DeclareTargetIds.ZhongzJihLingxReng },
               new Target {Name = DeclareKeys.ZhongzJihChengy, Id =DeclareTargetIds.ZhongzJihChengy },
               new Target {Name = Target.Prefix+DeclareKeys.GugJiaos, Id = DeclareTargetIds.GugJiaos },
               new Target {Name = Target.Prefix+DeclareKeys.JiaoxNengs, Id =DeclareTargetIds.JiaoxNengs },
               new Target {Name = Target.Prefix+DeclareKeys.JiaoxXinx, Id =DeclareTargetIds.JiaoxXinx },
               new Target {Name = DeclareKeys.GaodJiaoSYanxBanXuey, Id =DeclareTargetIds.GaodJiaoSYanxBanXuey },
        };


   }

   public class Target
   {
      public long Id { get; set; }
      public string Name { get; set; }

      public static string Prefix = "上一轮是";
   }


}