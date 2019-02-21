using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasUtility
{

	/// <summary>
	/// 用户登录信息，实际开发时可以继承该类，用以获取用户附加的数据
	/// </summary>
	public class CasUserInfo
	{

		/// <summary>
		/// 用户名
		/// </summary>
		public string User { get; set; }

	}

}
