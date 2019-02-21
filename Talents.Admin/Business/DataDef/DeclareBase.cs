using Business.Helper;

namespace Business
{

	public partial class DeclareBase
	{

		public string DeclareTarget => DeclareBaseHelper.DeclareTarget.GetName(DeclareTargetPKID);

		public string DeclareSubject => DeclareBaseHelper.DeclareSubject.GetName(DeclareSubjectPKID);

		public string DeclareStage => DeclareBaseHelper.DeclareStage.GetName(DeclareStagePKID);

		public string RealName { get; set; }

	}

}