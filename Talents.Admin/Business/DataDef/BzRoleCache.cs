using Business.Cache;
using System.Collections.Generic;
using System.Linq;

namespace Business
{

	public static class BzRoleCache
	{

		public static Dictionary<long, BzRole> Cached
		{
			get
			{
				var cache = ThisAppCache.GetCache<Dictionary<long, BzRole>>();

				if (cache == null)
				{
					using (APDBDef db = new APDBDef())
					{
						cache = db.BzRoleDal.ConditionQuery(null, null, null, null)
							.ToDictionary(m => m.Id);

						ThisAppCache.SetCache(cache);
					}
				}

				return cache;
			}
		}


        public static BzRole FindRole(string role)
            =>Cached.Values.FirstOrDefault(x => x.Name == role);


		public static void ClearCache()
			=> ThisAppCache.RemoveCache<Dictionary<long, BzRole>>();

	}

}