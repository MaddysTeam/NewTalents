using Symber.Web.Data;
using System.Collections.Generic;
using System.Data;

namespace Symber.Web.Report
{

	public class BaseLookupAPRptColumn : LookupAPRptColumn
	{

		#region [ Constructors ]


		public BaseLookupAPRptColumn(APSqlOperateExpr selectExpr,
			APTableDef joinTable, APSqlJoinType joinType, APRelationDef relationDef, APRptColumn relationShowColumn)
			: base(selectExpr, joinTable, joinType, relationDef, relationShowColumn)
		{
		}


		public BaseLookupAPRptColumn(APSqlOperateExpr selectExpr, string id, string title,
			APTableDef joinTable, APSqlJoinType joinType, APRelationDef relationDef, APRptColumn relationShowColumn)
			: base(selectExpr, id, title, joinTable, joinType, relationDef, relationShowColumn)
		{
		}


		#endregion


		#region [ Override Implementation of APColumnEx ]


		public override APSqlOrderPhrase GetQueryOrderByPhrase(APSqlOrderAccording according)
		{
			return RelationShowColumn.GetQueryOrderByPhrase(according);
		}


		public override void AddToQueryGroupPhrases(List<APSqlExprPhrase> phrases)
		{
			base.AddToQueryGroupPhrases(phrases);
			RelationShowColumn.AddToQueryGroupPhrases(phrases);
		}


		protected override object DefaultJson(IDataReader reader)
		{
			return new
			{
				id = (long)reader[DataName],
				text = reader[RelationShowColumn.DataName],
			};
		}


		#endregion

	}

}