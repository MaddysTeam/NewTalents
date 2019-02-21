﻿using System.Configuration;

namespace Business.Config
{

	public class ThisApp
	{

		// 系统开发商 ID
		public const long AppUser_Designer_Id = 2;
		// 系统管理员 ID
		public const long AppUser_Admin_Id = 1;

		// 管理员角色 ID
		public const long AppRole_Admin_Id = 1;

		//	教师类型
		public const string Teacher = "Teacher";

		// 稳定的数据缓存时间（分钟）
		public const int StableCacheMinutes = 20;

		// 不稳定数据缓存时间（分钟）
		public const int UnstableCacheMinutes = 2;

		//	缓存当前用户
		public const string UserProfile = "UserProfile";

		//	每页显示多小数据
		public const int PageSize = 10;

		//	图片上传路径
		public const string UploadFilePath = "/Attachments/";

		//	图片宽度
		public const int ImageWidth = 900;

		// 图片高度
		public const int ImageHeight = 400;

		// 缺省用户密码
		public const string DefaultPassword = "Win@123";

		// 缺省邮箱后缀
		public const string DefaultEmailSuffix = "@rctd.hkedu.sh.cn";

		//	权限session
		public const string Approve = "SignApprove";

      // 日志数据库连接字符串
      public const string LogDBConnctString = "Data Source=10.1.1.8;Initial Catalog=NewTalents;User ID=sa;Password=Kd2017.com;";

      // 日志文件地址
      public const string LogFilePath = "";

      //当前数据库名称
      public const string DBName = "NewTalents";

      //当前站点域名和端口
      public const string SiteDomainAndPort = "http://rctd.hkedu.sh.cn:8000/";
   }
}