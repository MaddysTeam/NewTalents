namespace TheSite.Models
{

	public class UserEvalViewModel
	{

		public long PeriodId { get; set; }
		public string PeriodName { get; set; }
		public double SchoolScore { get; set; }
		public double SchoolFullScore { get; set; }
		public bool IsSchoolEval { get { return SchoolFullScore > 0; } }
		public double VolumnScore { get; set; }
		public double VolumnFullScore { get; set; }
		public bool IsVolumnEval { get { return VolumnFullScore > 0; } }
		public double QualityScore { get; set; }
		public double QualityFullScore { get; set; }
		public bool IsQualityEval { get { return QualityFullScore > 0; } }
		public double CharacteristicScore { get; set; }
		public bool IsCharacteristicEval { get { return CharacteristicScore > 0; } }
        public double FullScore { get; set; }
	}

}