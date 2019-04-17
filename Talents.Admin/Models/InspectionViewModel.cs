using Business.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class ZisFaz
	{
		
		public double DusHuod_Score { get; set; }

		public double Kaik_Score { get; set; }

		public double Pingw_Score { get; set; }

		public double Pingb_Score { get; set; }

		public double FabLunw_Score { get; set; }

		public double LixKet_Score { get; set; }

		public double XiangmYanj_Score { get; set; }

	}

	public class PeixKec
	{
		public double KaisKec_Score { get; set; }

		public double JiangzBaog_Score { get; set; }

		public double KecZiy_Score { get; set; }
	}

	public class DaijJiaos
	{
		public double RicGongxlZhid_Score { get; set; }

		public double TingkZhid_Score { get; set; }

		public double JiaoalXiugZhid_Score { get; set; }

		public double LunwHuoKetXiugZhid_Score { get; set; }

		public string HuodAnp { get; set; }

		public string JiangfZhid { get; set; }

		public double XueyShul_Score { get; set; }

		public double DaijXiey_Score { get; set; }

		public double DaijFanga_Score { get; set; }

		public double DaijXiaoj_Score { get; set; }

		public double XueyChengzFenx_Score { get; set; }

		public double KaisZhansk_Score { get; set; }

		public double FabLunwHuoCanyKetYanj_Score { get; set; }

		public double JiaoyJiaoxPingbi_Score { get; set; }
	}

	public class InspectionSchool
	{
		public string Shid { get; set; }

		public double VolumnScore { get; set; }

		public string Volumn_Str { get { return this.VolumnScore + "/15"; } }

		public double QualityScore { get; set; }

		public string Quality_Str {	get { return this.QualityScore + "/15"; } }
	}

	public class InspectionVolumn
	{
		public long TargetId { get; set; }


		#region [ 自身发展 ]

		public ZisFaz ZisFaz { get; set; } = new ZisFaz();

		public string DusHuod_Str { get { return string.Format("{0}/{1}", this.ZisFaz.DusHuod_Score, 1); } }

		public string KaikPingwPingb_Str { get { return string.Format("{0}/{1}", this.KaikPingwPingb_Score, 2); } }

		public string Key_Str { get { return string.Format("{0}/{1}", this.Key_Score, 2); } }

		public double ZisFaz_Score { get { return this.ZisFaz.DusHuod_Score + this.KaikPingwPingb_Score + this.Key_Score; } }

		public string ZisFaz_Str
		{
			get
			{
				return string.Format("{0}/{1}", ZisFaz_Score, 5);
			}
		}

		public double KaikPingwPingb_Score
		{
			get
			{
				var totalScore = this.ZisFaz.Kaik_Score + this.ZisFaz.Pingw_Score + this.ZisFaz.Pingb_Score;
				return totalScore > 2 ? 2 : totalScore;
			}
		}

		public double Key_Score
		{
			get
			{
				var totalScore = this.ZisFaz.FabLunw_Score + this.ZisFaz.LixKet_Score + this.ZisFaz.XiangmYanj_Score;
				return totalScore > 2 ? 2 : totalScore;
			}
		}

		#endregion


		#region [培训讲座]

		public PeixKec PeixKec { get; set; } = new PeixKec();

		public string KaisKec_Str { get { return string.Format("{0}/{1}", this.PeixKec.KaisKec_Score, 5); } }

		public string JiangzBaog_Str { get { return string.Format("{0}/{1}", this.PeixKec.JiangzBaog_Score, 5); } }

		public string KecZiy_Str { get { return string.Format("{0}/{1}", this.PeixKec.KecZiy_Score, 5); } }

		public string PeixKec_Str { get { return string.Format("{0}/{1}", this.PeixKec_Score, 5); } }

		public double PeixKec_Score
		{
			get
			{
				var totalScore = this.PeixKec.KaisKec_Score + this.PeixKec.JiangzBaog_Score + this.PeixKec.KecZiy_Score;
				if (TargetId == DeclareTargetIds.GaodLisz || TargetId == DeclareTargetIds.JidZhucr)
				{
					totalScore = this.PeixKec.JiangzBaog_Score > 0 ? totalScore : 0;
				}

				return totalScore > 5 ? 5 : totalScore;
			}
		}

		#endregion


		#region [带教教师]

		public DaijJiaos DaijJiaos { get; set; } = new DaijJiaos();

		public string DaijZhid_Str { get { return string.Format("{0}/{1}", this.DaijZhid_Score, 2); } }

		public string QitDaijGongz_Str { get { return string.Format("{0}/{1}", this.QitDaijGongz_Score, 5); } }

		public string DaijChengg_Str { get { return string.Format("{0}/{1}", this.DaijChengg_Score, 3); } }

		public string DaijJiaos_Str { get { return string.Format("{0}/{1}", this.DaijJiaos_Score, 10); } }

		public double DaijZhid_Score
		{
			get
			{
				var totalScore =	this.DaijJiaos.RicGongxlZhid_Score + this.DaijJiaos.TingkZhid_Score + this.DaijJiaos.JiaoalXiugZhid_Score + this.DaijJiaos.LunwHuoKetXiugZhid_Score;
				return totalScore > 2 ? 2 : totalScore;
			}
		}

		public string HuodJingfZhid
		{
			get
			{
				if (this.DaijJiaos.HuodAnp == "达标" && this.DaijJiaos.JiangfZhid == "达标") { return "达标"; } else { return "未达标"; }
			}
		}

		public double QitDaijGongz_Score
		{
			get
			{
				return this.DaijJiaos.XueyShul_Score + this.DaijJiaos.DaijXiey_Score + this.DaijJiaos.DaijFanga_Score + this.DaijJiaos.DaijXiaoj_Score + this.DaijJiaos.XueyChengzFenx_Score;
			}
		}

		public double DaijChengg_Score
		{
			get
			{
				var totalScore = this.DaijJiaos.KaisZhansk_Score + this.DaijJiaos.FabLunwHuoCanyKetYanj_Score + this.DaijJiaos.JiaoyJiaoxPingbi_Score;
				return totalScore > 3 ? 3 : totalScore;
			}
		}

		public double DaijJiaos_Score { get { return this.DaijZhid_Score + this.QitDaijGongz_Score + this.DaijChengg_Score; } }

		#endregion
	}

	public class InspectionQualityDataModel
	{
		public long TeacherId { get; set; }

		public long PeriodId { get; set; }

		public long ResultId { get; set; }

		public long DeclareTargetPKID { get; set; }

		public long GroupId { get; set; }

		public double Characteristic { get; set; }

		public string EvalItemKey { get; set; }

		public double ResultValue { get; set; }

		public double AdjustScore { get; set; }
	}

	public class InspectionQuality
	{
		public long TargetId { get; set; }


		#region [ 自身发展 ]

		public ZisFaz ZisFaz { get; set; } = new ZisFaz();


		public string DusHuod_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.ZisFaz.DusHuod_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DusHuod));
			}
		}

		public double KaikPingwPingb_Score{ get; set; }

		public string KaikPingwPingb_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.KaikPingwPingb_Score,
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.KaikPingwPingb));
			}
		}

		public string FabLunw_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.ZisFaz.FabLunw_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.Key));
			}
		}

		public string LixKet_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.ZisFaz.LixKet_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.Key));
			}
		}

		public string XiangmYanj_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.ZisFaz.XiangmYanj_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.Key));
			}
		}

		public double Key_Score { get; set; }

		public string Key_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.Key_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.Key));
			}
		}

		public double ZisFaz_Score
		{
			get
			{
				return this.ZisFaz.DusHuod_Score + this.KaikPingwPingb_Score + this.Key_Score;				
			}
		}

		public string ZisFaz_Str
		{
			get
			{
				var maxScore = InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DusHuod) + 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.KaikPingwPingb) + 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.Key);
				return string.Format("{0}/{1}", this.ZisFaz_Score, maxScore);
			}
		}

		#endregion


		#region [ 培训讲座 ]

		public PeixKec PeixKec { get; set; } = new PeixKec();

		public string KaisKec_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.PeixKec.KaisKec_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.PeixKec));
			}
		}

		public string KecZiy_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.PeixKec.KecZiy_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.PeixKec));
			}
		}

		public string JiangzBaog_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.PeixKec.JiangzBaog_Score,
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.PeixKec));
			}
		}

		public double PeixKec_Score { get; set; }

		public string PeixKec_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.PeixKec_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.PeixKec));
			}
		}

		#endregion


		#region [ 带教教师 ]

		public DaijJiaos DaijJiaos { get; set; } = new DaijJiaos();

		public double DaijZhid_Score { get; set; }

		public string DaijZhid_Str
		{
			get
			{
				return string.Format("{0}/{1}", DaijZhid_Score, InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DaijZhid));
			}
		}

		public double QitDaijGongz_Score { get { return this.DaijJiaos.XueyShul_Score + this.DaijJiaos.DaijFanga_Score + this.DaijJiaos.XueyChengzFenx_Score; } }

		public string QitDaijGongz_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.QitDaijGongz_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.QitDaijGongz));
			}
		}

		public double DaijChengg_Score { get; set; }

		public string DaijChengg_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.DaijChengg_Score, 
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DaijChengg));
			}
		}

		public double DaijJiaos_Score
		{
			get
			{
				return DaijZhid_Score + this.QitDaijGongz_Score + this.DaijChengg_Score;
			}
		}

		public string DaijJiaos_Str
		{
			get
			{
				var totalScore = DaijZhid_Score + this.QitDaijGongz_Score + this.DaijChengg_Score;
				var maxScore = InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DaijZhid) +
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.QitDaijGongz) +
					InspectionQualityHelper.GetMaxScore(TargetId, InspectionQualityType.DaijChengg);
				return string.Format("{0}/{1}", totalScore, maxScore);
			}
		}

		public double TesGongz_Score { get; set; }

		public string TesGongz_Str
		{
			get
			{
				return string.Format("{0}/{1}", this.TesGongz_Score, 5);
			}
		}

		public double Adjust_Score { get; set; }

		#endregion
	}

	public class InspectionViewModel
	{

		public InspectionViewModel()
		{
			School = new InspectionSchool();
			Volumn = new InspectionVolumn();
			Quality = new InspectionQuality();
			Volumn.TargetId = TargetId;
			Quality.TargetId = TargetId;
			
		}

		public long TeacherId { get; set ; }

		private long targetId;

		public long TargetId { get { return this.targetId; } set {
				this.Quality.TargetId = value; this.targetId = value; } }

		public string TargetName { get; set; }

		public string SubjectName { get; set; }

		public string TeacherName { get; set; }

		public string CompanyName { get; set; }

		public InspectionSchool School { get; }

		public InspectionVolumn Volumn { get; }

		public InspectionQuality Quality { get; }

		public double SchoolScore { get { return this.School.VolumnScore + this.School.QualityScore; } }

		public string SchoolStr { get{ return string.Format("{0}/{1}", SchoolScore, 30); } }

		public double VolumnScore { get { return this.Volumn.ZisFaz_Score + this.Volumn.PeixKec_Score + this.Volumn.DaijJiaos_Score; } }

		public string VolumnStr { get { return string.Format("{0}/{1}", VolumnScore, 20); } }

		public double QualityScore { get { return this.Quality.ZisFaz_Score + this.Quality.PeixKec_Score + this.Quality.DaijJiaos_Score + this.Quality.Adjust_Score; } }

		public string QualityStr { get { return string.Format("{0}/{1}", QualityScore, 50); } }

		public double Total { get { return this.VolumnScore + this.QualityScore + this.SchoolScore + this.Quality.TesGongz_Score; } }

		public string TotalStr { get { return string.Format("{0}/{1}", this.Total, 105); } }
	}

	public class InspectionUserInfoModel
	{
		public long TeacherId { get; set; }

		public string TeahcerName { get; set; }

		public long TargetId { get; set; }

		public string Target { get; set; }

		public string Subject { get; set; }

		public string Stage { get; set; }

		public string CompanyName { get; set; }
	}

	public class InspectionSchoolModel
	{
		public long TeacherId { get; set; }

		public string Morality { get; set; }

		public string EvalItemKey { get; set; }

		public string ResultValue { get; set; }
	}

   public class InsepctionDeclareProfile
   {
      public long Id { get; set; }

      [Display(Name = "姓名")]
      public string RealName { get; set; }
      [Display(Name = "性别")]
      public string Gender { get; set; }
      [Display(Name = "单位")]
      public string Company { get; set; }
      [Display(Name = "出生日期")]
      public string Birthday { get; set; }
      [Display(Name = "民族")]
      public string Nation { get; set; }
      [Display(Name = "评聘日期")]
      public string HireDate { get; set; }
      [Display(Name = "师训编号")]
      public string TrainNo { get; set; }
      [Display(Name = "职务")]
      public string RankTitle { get; set; }
      [Display(Name = "学历")]
      public string EduBg { get; set; }
      [Display(Name = "学位")]
      public string EduDegree { get; set; }
      [Display(Name = "现任职称")]
      public string SkillTitle { get; set; }
      [Display(Name = "任教学段")]
      public string EduStage { get; set; }
      [Display(Name = "任教学科")]
      public string EduSubject { get; set; }
      [Display(Name = "周课时数")]
      public string WeekCount { get; set; }
      [Display(Name = "手机")]
      public string Mobile { get; set; }
      [Display(Name = "办公室电话")]
      public string Phone { get; set; }
      [Display(Name = "邮箱")]
      public string EMail { get; set; }
      [Display(Name = "是否愿意区内流动")]
      public string IsAllowFlowToSchool { get; set; }
      [Display(Name = "是否允许低一层级评选")]
      public string IsAllowDowngrade { get; set; }
   }

   public class InsepctionDeclareReview
   {

      public long Id { get; set; }

      [Display(Name = "姓名")]
      public string RealName { get; set; }
      [Display(Name = "性别")]
      public string Gender { get; set; }
      [Display(Name = "出生日期")]
      public string Birthday { get; set; }
      [Display(Name = "现任职称")]
      public string SkillTitle { get; set; }

      [Display(Name = "申报称号")]
      public string DeclareTarget { get; set; }
      [Display(Name = "申报单位")]
      public string DeclareCompany { get; set; }
      [Display(Name = "申报学科")]
      public string DeclareSubject { get; set; }

      [Display(Name = "任教学科")]
      public string EduSubject { get; set; }
      [Display(Name = "手机")]
      public string Mobile { get; set; }
      [Display(Name = "是否愿意区内流动")]
      public string IsAllowFlowToSchool { get; set; }
      [Display(Name = "是否允许低一层级评选")]
      public string IsAllowDowngrade { get; set; }

      [Display(Name = "年度考核优秀(年份)")]
      public string GoodYear { get; set; }
      [Display(Name = "是否职称破格")]
      public string IsDeclareBroke { get; set; }
      [Display(Name = "是否材料破格")]
      public string IsMaterialBroke { get; set; }

   }

}