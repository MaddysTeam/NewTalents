using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Business.Identity
{

	public class EmailService : IIdentityMessageService
	{

		public Task SendAsync(IdentityMessage message)
		{
			// 在此处插入电子邮件服务可发送电子邮件。
			return Task.FromResult(0);
		}

	}

}
