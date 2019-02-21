using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSite.Models;

namespace TheSite.Models
{

	public class VolumnEvalOverviewModels
	{

		public long TargetId { get; set; }
		public string TargetName { get; set; }
		public int TotalCount { get; set; }
		public int EvalCount { get; set; }
		public EvalStatus Status { get; set; }

	}

}
