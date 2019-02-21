using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Symber.Web.Identity
{

	public class RoleStore<TUser, TRole, TContext, TKey> :
		 IQueryableRoleStore<TRole, TKey>
		 where TUser : IdentityUser<TKey>
		 where TRole : IdentityRole<TKey>
		 where TContext : IdentityDbContext<TUser, TRole, TKey>
		 where TKey : IEquatable<TKey>
	{

		public RoleStore(TContext context)
		{
			Context = context;
		}


		public virtual TContext Context { get; private set; }


		#region [ IQueryableRoleStore ]


		public virtual IQueryable<TRole> Roles
			=> Context.RoleDal.GetAll();


		public async virtual Task CreateAsync(TRole role)
			=> await Task.Run(() => Context.RoleDal.Create(role));


		public async virtual Task UpdateAsync(TRole role)
			=> await Task.Run(() => Context.RoleDal.Update(role));


		public async virtual Task DeleteAsync(TRole role)
			=> await Task.Run(() => Context.RoleDal.Delete(role.Id));


		public async virtual Task<TRole> FindByIdAsync(TKey roleId)
			=> await Task.Run(() => Context.RoleDal.FindById(roleId));


		public async virtual Task<TRole> FindByNameAsync(string roleName)
			=> await Task.Run(() => Context.RoleDal.FindByName(roleName));


		#endregion


		#region [ IDisposable ]


		public void Dispose() { }


		#endregion

	}

}
