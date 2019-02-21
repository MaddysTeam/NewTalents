using Business;
using Business.XOrg;
using System.Web.Mvc;
using System.Web.Routing;

namespace Business.BasicExtinsions
{

	public static class DoubleExtensions
	{

		public static double EnsureInRange(this double value, double min, double max)
		 => value > max ? max :
			 value < min ? min :
			 value;

	}

}