using Business;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class DeclareActiveDataModel : DeclareActive
	{

		public long TargetId { get; set; }

	}

}