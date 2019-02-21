using Business;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class DeclareAchievementDataModel : DeclareAchievement
	{

		public long TargetId { get; set; }

	}

}