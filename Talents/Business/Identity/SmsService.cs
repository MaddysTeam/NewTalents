using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Business.Identity
{

	public class SmsService : IIdentityMessageService
	{

		public Task SendAsync(IdentityMessage message)
		{
			// 在此处插入 SMS 服务可发送短信。
			return Task.FromResult(0);
		}

	}

}
