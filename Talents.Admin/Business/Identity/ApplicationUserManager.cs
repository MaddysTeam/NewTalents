using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Symber.Web.Data;
using Symber.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Identity
{

	// 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
	public class ApplicationUserManager : UserManager<BzUser, long>
	{

		public ApplicationUserManager(IUserStore<BzUser, long> store) : base(store)
		{
        }


		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<BzUser, BzRole, ApplicationDbContext, long>(context.Get<ApplicationDbContext>()));

			// 添加 APDBDef
			manager.DB = context.Get<APDBDef>();

			// 配置用户名的验证逻辑
			manager.UserValidator = new UserValidator<BzUser, long>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// 配置密码的验证逻辑
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				//RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			// 配置用户锁定默认值
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
			// 你可以编写自己的提供程序并将其插入到此处。
			manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<BzUser, long>
			{
				MessageFormat = "你的安全代码是 {0}"
			});
			manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<BzUser, long>
			{
				Subject = "安全代码",
				BodyFormat = "你的安全代码是 {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					 new DataProtectorTokenProvider<BzUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}


		public APDBDef DB { get; set; }


		public virtual Task<IdentityResult> AddToRoleAsync(long userId, long roleId, string scopeType, long scopeId)
		{
			DB.BzUserRoleDal.Insert(new BzUserRole
			{
				UserId = userId,
				RoleId = roleId,
				ScopeType = scopeType,
				ScopeId = scopeId
			});

			return Task.FromResult(IdentityResult.Success);
		}


		public virtual Task<IdentityResult> RemoveFromRoleAsync(long userId, long roleId, string scopeType, long scopeId)
		{
			var ur = APDBDef.BzUserRole;

			APQuery.delete(ur)
				.where(ur.UserId == userId & ur.RoleId == roleId & ur.ScopeType == scopeType & ur.ScopeId == scopeId)
				.execute(DB);

			return Task.FromResult(IdentityResult.Success);
		}


		public virtual Task<IList<BzUserRole>> GetRoleTypes(long userId)
		{
			var ur = APDBDef.BzUserRole;

			IList<BzUserRole> ret = APQuery.select(ur.Asterisk)
				 .from(ur)
				 .where(ur.UserId == userId)
				 .query(DB, ur.Map)
				 .ToList();

			return Task.FromResult(ret);
		}


		public virtual Task<bool> IsInRole(long userId, long roleId, string scopeType, long scopeId)
		{
			var ur = APDBDef.BzUserRole;

			return Task.FromResult((int)APQuery.select(ur.Asterisk.Count())
				.from(ur)
				.where(ur.UserId == userId & ur.RoleId == roleId & ur.ScopeType == scopeType & ur.ScopeId == scopeId)
				.executeScale(DB) > 0);
		}


		public virtual Task<BzUserProfile> GetProfile(long userId)
		{
			return Task.FromResult(DB.BzUserProfileDal.PrimaryGet(userId));
		}

	}

}
