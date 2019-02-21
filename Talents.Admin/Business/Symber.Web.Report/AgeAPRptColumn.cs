using Symber.Web.Data;
using System;

namespace Symber.Web.Report
{

	public class AgeAPRptColumn : APRptColumn
	{

		#region [ Constructors ]


		public AgeAPRptColumn(DateTimeAPColumnDef columnDef)
			: base(columnDef)
		{
		}


		#endregion


		#region [ Override Implementation of APColumnEx ]


		public override APRptFilterType FilterType => APRptFilterType.Number;


		public override APSqlWherePhrase ParseQueryWherePhrase(APRptFilterComparator comparator, params string[] values)
		{
			throw new NotImplementedException();
		}


		public override APSqlOperateExpr SelectExpr
		{
			get
			{
				return new APSqlRawExpr("DATEDIFF(yyyy, Birthday, GETDATE())");
			}
		}

		protected override object DefaultJson(System.Data.IDataReader reader)
		{
			return (long)base.DefaultJson(reader);
		}


		#endregion

	}

}