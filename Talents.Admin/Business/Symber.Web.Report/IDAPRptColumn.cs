using Symber.Web.Data;
using System;

namespace Symber.Web.Report
{

	public class IDAPRptColumn : APRptColumn
	{

		#region [ Constructors ]


		public IDAPRptColumn(Int64APColumnDef columnDef)
			: base(columnDef)
		{
		}


		public IDAPRptColumn(Int64APColumnDef columnDef, string id, string title)
			: base(columnDef, id, title)
		{
		}


		#endregion


		#region [ Override Implementation of APColumnEx ]


		public override bool CanBeField => false;


		public override APSqlWherePhrase ParseQueryWherePhrase(APRptFilterComparator comparator, params string[] values)
		{
			throw new NotImplementedException();
		}


		#endregion

	}

}