﻿namespace Business.Helper
{

	public static class DeclareTargetIds
	{
		public const long GonggJihZhucRen = 999;
		public const long GaofJihZhucRen = 1000;
		public const long GonggJihChengy = 1001;
		public const long ZhongzJihLingxReng = 1002;
		public const long ZhongzJihChengy = 1003;
		public const long GaodJiaoSYanxBanXuey = 1004;
		public const long WaipDaos = 5001;
		public const long GaodLisz = 5002;
		public const long JidZhucr = 5003;
		public const long GongzsZhucr = 5004;
		public const long XuekDaitr = 5005;
		public const long GugJiaos = 5006;
		public const long JiaoxNengs = 5007;
		public const long JiaoxXinx = 5008;
		public const long TezXuey = 5009;
		public const long PutLaos = 5010;

		public static bool AllowZhidJians(long id)
		   => id == GaodLisz || id == JidZhucr || id == GongzsZhucr || id == WaipDaos || id == PutLaos;

		public static bool AllowQunLiud(long id)
		   => id == XuekDaitr || id == GugJiaos;

		public static bool AllowPeihJiaoyy(long id)
		   => id == XuekDaitr;

		public static bool AllowKecShis(long id)
		   => id == GaodLisz || id == JidZhucr || id == GongzsZhucr;

		public static bool HasTeam(long id)
		   => id == GaodLisz || id == JidZhucr || id == GongzsZhucr || id == WaipDaos || id == XuekDaitr || id == GugJiaos || id == PutLaos;

		public static bool AllowEval(long id)
		 => id != WaipDaos && id != TezXuey && id != PutLaos;

		public static bool AllowRule(long id)
		   => id == GaodLisz || id == JidZhucr || id == GongzsZhucr || id == XuekDaitr || id == GugJiaos;

		public static bool AllowXuesHuod(long id)
		   => id == XuekDaitr || id == GugJiaos || id == JiaoxNengs || id == JiaoxXinx || id == PutLaos;

		public static bool AllowDaijChengg(long id)
		   => id == GaodLisz || id == JidZhucr || id == GongzsZhucr || id == XuekDaitr || id == JiaoxNengs || id == JiaoxXinx || id == TezXuey || id == PutLaos;

		public static bool AllowKecZiy(long id)
		   => id == XuekDaitr || id == GugJiaos || id == PutLaos;
	}

}