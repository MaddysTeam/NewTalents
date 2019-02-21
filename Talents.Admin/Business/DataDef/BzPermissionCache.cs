using Business.Cache;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class BzPermissionCache
    {

        public static Dictionary<long, BzPermission> Cached
        {
            get
            {
                var cache = ThisAppCache.GetCache<Dictionary<long, BzPermission>>();

                if (cache == null)
                {
                    using (APDBDef db = new APDBDef())
                    {
                        cache = db.BzPermissionDal.ConditionQuery(null, null, null, null)
                            .ToDictionary(m => m.Id);

                        ThisAppCache.SetCache(cache);
                    }
                }

                return cache;
            }
        }


        public static Dictionary<long, BzRolePermission> RolePermissionCached
        {
            get
            {
                var cache = ThisAppCache.GetCache<Dictionary<long, BzRolePermission>>();

                if (cache == null)
                {
                    using (APDBDef db = new APDBDef())
                    {
                        cache = db.BzRolePermissionDal.ConditionQuery(null, null, null, null)
                            .ToDictionary(m => m.RolePermissionId);

                        ThisAppCache.SetCache(cache);
                    }
                }

                return cache;
            }
        }


        public static BzPermission FindPermission(string permisson)
           => Cached.Values.FirstOrDefault(x => x.Name == permisson);


        public static BzRolePermission FindRolePermission(long roleId,long permissonId )
         => RolePermissionCached.Values.FirstOrDefault(x => x.RoleId == roleId && x.PermissionId == permissonId);


        public static void ClearCache()
        => ThisAppCache.RemoveCache<Dictionary<long, BzPermission>>();


        public static void ClearRolePermissonCache()
        => ThisAppCache.RemoveCache<Dictionary<long, BzRolePermission>>();
    }
}