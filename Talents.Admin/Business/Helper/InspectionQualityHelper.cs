using Business.Config;
using Symber.Web.Report;
using System;

namespace Business.Helper
{

	public static class InspectionQualityHelper
	{

		public static double GetMaxScore(long targetId, InspectionQualityType key, double flod = 1)
		{
			double MaxScore = 0;

			switch (key)
			{
				case InspectionQualityType.DusHuod:
					MaxScore = GetDusHuodMaxScore(targetId, flod);
					break;
				case InspectionQualityType.KaikPingwPingb:
					MaxScore = GetKaikPingwPingbMaxScore(targetId, flod);
					break;
				case InspectionQualityType.Key:
					MaxScore = GetKeyMaxScore(targetId, flod);
					break;
				case InspectionQualityType.PeixKec:
					MaxScore = GetPeixKecMaxScore(targetId, flod);
					break;
				case InspectionQualityType.QitDaijGongz:
					MaxScore = GetQitDaijGongzMaxScore(targetId, flod);
					break;
				case InspectionQualityType.DaijZhid:
					MaxScore = GetDaijZhidMaxScore(targetId, flod);
					break;
				case InspectionQualityType.DaijChengg:
					MaxScore = GetDaijChenggMaxScore(targetId, flod);
					break;
			}

			return MaxScore;
		}


		private static double GetDusHuodMaxScore(long targetId, double fold = 1)
		{
			double MaxScore = 0;
			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 2 * fold;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 2.5 * fold;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 3 * fold;
					break;
			}

			return MaxScore;
		}


		private static double GetKaikPingwPingbMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 4 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 8 * flod;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 10 * flod;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 15 * flod;
					break;
			}

			return MaxScore;
		}


		private static double GetKeyMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 4 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 10 * flod;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 12.5 * flod;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 12 * flod;
					break;
			}

			return MaxScore;
		}


		private static double GetPeixKecMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 20 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
				case DeclareTargetIds.XuekDaitr:
				case DeclareTargetIds.GugJiaos:
					MaxScore = 15 * flod;
					break;
			}

			return MaxScore;
		}


		private static double GetQitDaijGongzMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 4.5 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 3.5 * flod;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 2.5 * flod;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 2 * flod;
					break;
			}

			return MaxScore;
		}


		private static double GetDaijZhidMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 8 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 6 * flod;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 3.5 * flod;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 1.5 * flod;
					break;
			}

			return MaxScore;
		}


		private static double GetDaijChenggMaxScore(long targetId, double flod = 1)
		{
			double MaxScore = 0;

			switch (targetId)
			{
				case DeclareTargetIds.GaodLisz:
				case DeclareTargetIds.JidZhucr:
					MaxScore = 7.5 * flod;
					break;
				case DeclareTargetIds.GongzsZhucr:
					MaxScore = 5.5 * flod;
					break;
				case DeclareTargetIds.XuekDaitr:
					MaxScore = 4 * flod;
					break;
				case DeclareTargetIds.GugJiaos:
					MaxScore = 1.5 * flod;
					break;
			}

			return MaxScore;
		}

	}


	public enum InspectionQualityType
	{
		DusHuod,
		KaikPingwPingb,
		Key,
		PeixKec,
		QitDaijGongz,
		DaijZhid,
		DaijChengg
	}

}

