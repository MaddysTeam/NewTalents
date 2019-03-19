using Business.Cache;
using Symber.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace Business
{

   public static class PicklistCache
   {

      public class CacheUnit
      {

         #region [ Fields ]


         private long _pkid;
         private PicklistItem _defaultItem;
         private readonly List<PicklistItem> _items = new List<PicklistItem>();
         private readonly Dictionary<long, PicklistItem> _idItemDict = new Dictionary<long, PicklistItem>();
         private readonly Dictionary<string, PicklistItem> _nameItemDict = new Dictionary<string, PicklistItem>();


         #endregion


         #region [ Constructors ]


         public CacheUnit(long pkid, List<PicklistItem> list)
         {
            _pkid = pkid;
            _items.AddRange(list);

            list.ForEach(it =>
            {
               if (it.IsDefault)
               {
                  _defaultItem = it;
               }
               _idItemDict.Add(it.PicklistItemId, it);
               _nameItemDict.Add(it.Name, it);
            });
         }


         #endregion


         #region [ Properties ]


         public long PKID => _pkid;


         public PicklistItem DefaultItem => _defaultItem;


         public IReadOnlyList<PicklistItem> Items => _items;


         public IReadOnlyDictionary<long, PicklistItem> IdItemDict => _idItemDict;


         public IReadOnlyDictionary<string, PicklistItem> NameItemDict => _nameItemDict;


         #endregion


         #region [ Methods ]


         public string GetItemName(long itemId) => _idItemDict[itemId].Name;


         public long GetItemId(string itemName) => _nameItemDict[itemName].PicklistItemId;


         #endregion

      }


      public static CacheUnit Cached(string innerKey)
      {
         var unitDict = ThisAppCache.GetCache<Dictionary<string, CacheUnit>>();

         if (unitDict == null)
         {
            ThisAppCache.SetCache(unitDict = new Dictionary<string, CacheUnit>());
         }

         if (!unitDict.ContainsKey(innerKey))
         {
            using (APDBDef db = new APDBDef())
            {
               var t = APDBDef.PicklistItem;
               var tp = APDBDef.Picklist;

               var items = APQuery.select(t.Asterisk)
                  .from(t, tp.JoinInner(t.PicklistId == tp.PicklistId))
                  .where(tp.InnerKey == innerKey)
                  .query(db, t.Map).ToList();

               CacheUnit cacheUnit = new CacheUnit(items.Count > 0 ? items[0].PicklistId : 0, items);
                  unitDict[innerKey] = cacheUnit;
            }
         }

         return unitDict[innerKey];
      }


      public static CacheUnit Cached(long pkid)
      {
         var unitDict = ThisAppCache.GetCache<Dictionary<string, CacheUnit>>();

         if (unitDict == null)
         {
            ThisAppCache.SetCache(unitDict = new Dictionary<string, CacheUnit>());
         }

         foreach (var p in unitDict)
         {
            if (p.Value.PKID == pkid)
               return p.Value;
         }

         using (APDBDef db = new APDBDef())
         {
            var t = APDBDef.PicklistItem;
            var tp = APDBDef.Picklist;

            var innerKey = (string)APQuery.select(tp.InnerKey)
               .from(tp)
               .where(tp.PicklistId == pkid)
               .executeScale(db);

            var items = APQuery.select(t.Asterisk)
               .from(t)
               .where(t.PicklistId == pkid)
               .query(db, t.Map).ToList();

            CacheUnit cacheUnit = new CacheUnit(items.Count > 0 ? items[0].PicklistId : 0, items);
            unitDict[innerKey] = cacheUnit;

            return cacheUnit;
         }
      }


      public static void ClearCache()
         => ThisAppCache.RemoveCache<Dictionary<string, CacheUnit>>();


      public static void RemoveCache(string innerKey)
      {
         var cache = ThisAppCache.GetCache<Dictionary<string, CacheUnit>>();

         if (cache != null && cache.ContainsKey(innerKey))
            cache.Remove(innerKey);
      }

   }

}