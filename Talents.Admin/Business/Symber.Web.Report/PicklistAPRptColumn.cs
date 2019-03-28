using Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symber.Web.Report
{

   public class PicklistAPRptColumn : APRptColumn
   {

      #region [ Constructors ]


      public PicklistAPRptColumn(Int64APColumnDef columnDef, string innerKey)
         : base(columnDef)
      {
         InnerKey = innerKey;
      }


      public PicklistAPRptColumn(Int64APColumnDef columnDef, string id, string title, string innerKey)
         : base(columnDef, id, title)
      {
         InnerKey = innerKey;
      }


      #endregion


      #region [ Properties ]


      public string InnerKey { get; }


      public bool AjaxLoad { get; set; }


      #endregion


      #region [ Methods ]


      public string GetName(long itemId, string defaultName = "", bool isThrow = true)
      //=> itemId == 0 ? defaultName : PicklistCache.Cached(InnerKey).GetItemName(itemId);
      {
         try
         {
            return itemId == 0 ? defaultName : PicklistCache.Cached(InnerKey).GetItemName(itemId);
         }
         catch (Exception ex)
         {
            if (isThrow)
               throw new Exception(InnerKey + " " + ex.Message);
            return defaultName;
         }
      }


      public long GetStrengthenValue(long itemId)
         => itemId == 0 ? 0 : PicklistCache.Cached(InnerKey).IdItemDict[itemId].StrengthenValue;


      public PicklistItem GetItem(long itemId)
         => itemId == 0 ? null : PicklistCache.Cached(InnerKey).IdItemDict[itemId];


      public long GetItemId(string name)
         => PicklistCache.Cached(InnerKey).GetItemId(name);


      public long GetPKID()
         => PicklistCache.Cached(InnerKey).PKID;


      public IReadOnlyList<PicklistItem> GetItems()
         => PicklistCache.Cached(InnerKey).Items;


      #endregion


      #region [ Override Implementation of APColumnEx ]


      public override APRptFilterType FilterType => APRptFilterType.EnumOrId;


      public override object GetFilterField()
      {
         if (AjaxLoad)
         {
            return new
            {
               id = ID,
               text = Title,
               type = FilterType.ToString(),
               ajax = "/Picklist/JsonNames/" + GetPKID()
            };
         }
         else
         {
            return new
            {
               id = ID,
               text = Title,
               type = FilterType.ToString(),
               items = from p in GetItems()
                       select p.Name
            };
         }
      }


      protected override object TryGetFilterValue(string value)
      {
         if (value == "")
            return 0;

         try
         {
            return PicklistCache.Cached(InnerKey).GetItemId(value);
         }
         catch (Exception ex)
         {
            throw APRptFilterParseException.InvalidValue(value, GetType(), ex);
         }
      }


      protected override APSqlWherePhrase GetQueryWherePhrase(APSqlConditionOperator op, string value)
      {
         object v = null;

         if (value == "NULL")
         {
            v = DBNull.Value;
         }
         else
         {
            if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
               value = value.Substring(1, value.Length - 2);
            v = TryGetFilterValue(value);
         }

         return new APSqlConditionPhrase(SelectExpr, op, v);
      }


      public override APSqlWherePhrase ParseQueryWherePhrase(APRptFilterComparator comparator, params string[] values)
      {
         if (values.Length == 0)
            throw APRptFilterParseException.ValuesCountCannotBeZero();

         APSqlOperateExpr expr = SelectExpr;

         if (comparator == APRptFilterComparator.Equals)
         {
            List<APSqlWherePhrase> wlist = new List<APSqlWherePhrase>();
            foreach (string value in values)
               wlist.Add(GetQueryWherePhrase(APSqlConditionOperator.Equals, value));

            if (wlist.Count == 1)
               return wlist[0];

            return wlist.JoinOr();
         }
         else if (comparator == APRptFilterComparator.NotEqual)
         {
            List<APSqlWherePhrase> wlist = new List<APSqlWherePhrase>();
            foreach (string value in values)
               wlist.Add(GetQueryWherePhrase(APSqlConditionOperator.NotEqual, value));

            if (wlist.Count == 1)
               return wlist[0];

            return wlist.JoinAnd();
         }
         else if (comparator == APRptFilterComparator.LessThan
            || comparator == APRptFilterComparator.LessOrEqual
            || comparator == APRptFilterComparator.GreaterThan
            || comparator == APRptFilterComparator.GreaterOrEqual)
         {
            if (values.Length > 1)
               throw APRptFilterParseException.UnsupportMultiValues(comparator);

            return GetQueryWherePhrase((APSqlConditionOperator)comparator, values[0]);
         }
         else if (comparator == APRptFilterComparator.Between)
         {
            if (values.Length != 2)
               throw APRptFilterParseException.BetweenMustHaveTwoValues();

            object v1 = TryGetFilterValue(values[0]);
            object v2 = TryGetFilterValue(values[1]);

            return new APSqlConditionPhrase(expr, APSqlConditionOperator.Between, new object[2] { v1, v2 });
         }

         throw APRptFilterParseException.UnsupportFilterComparator(GetType(), comparator);
      }


      protected override object DefaultJson(System.Data.IDataReader reader)
      {
         return GetName((long)base.DefaultJson(reader));
      }


      #endregion

   }

}