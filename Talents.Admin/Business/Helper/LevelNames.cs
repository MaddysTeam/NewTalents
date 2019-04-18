using System.Linq;

namespace Business.Helper
{

	public static class LevelNames
	{

		public const string Guojj = "国家级";
		public const string Shij = "市级";
		public const string Quj = "区级";
		public const string Xiaoj = "校级";

		public const string XuekMingt_Qiz = "期中";
		public const string XuekMingt_Qim = "期末";

		public const string PeixJiangz_Shij = "市级";
		public const string PeixJiangz_Weikc = "区级（微课程）";
		public const string PeixJiangz_YanxYit = "区级（研训一体课程）";

		public const string A = "A";
		public const string B = "B";
		public const string C = "C";
		public const string D = "D";

		public const string A1 = "1A";
		public const string B1 = "1B";
		public const string C1 = "1C";
		public const string D1 = "1D";

		public const string A2 = "2A";
		public const string B2 = "2B";
		public const string C2 = "2C";
		public const string D2 = "2D";


		//	特色活动开展

		public static string[] TesHuodKaiz = new string[]
		{
			Guojj,
			Shij,
			Quj
		};


		//	学科类专题讲座

		public static string[] ZhuantJiangz = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


		//	学术活动

		public static string[] XuesHuod = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


		//	教学活动.研讨课

		public static string[] JiaoxHuod_Yantk = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


		//	教学活动.教学公开课

		public static string[] JiaoxHuod_JiaoxGongkk = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


      //	教学活动.教学公开课

      public static string[] PeixJiangz_DingxxKec = new string[]
      {
         Guojj,
         Shij,
         Quj,
         Xiaoj
      };


      //	教学活动.教学公开课

      public static string[] JiaoxHuod_JiaoxPingb = new string[]
      {
         Guojj,
         Shij,
         Quj,
         Xiaoj
      };


      //	科研成果.课题研究

      public static string[] KeyChengg_KetYanj = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


		//	科研成果.发表论著

		public static string[] KeyChengg_FabLunw = new string[]
		{
			Guojj,
			Shij,
			Quj,
			Xiaoj
		};


		public static string[] XuekMingt = new string[]
		{
			XuekMingt_Qiz,
			XuekMingt_Qim
		};


		public static string[] PeixJiangz = new string[]
		{
			PeixJiangz_Shij,
			PeixJiangz_Weikc,
			PeixJiangz_YanxYit,
         Xiaoj
      };

	}

}