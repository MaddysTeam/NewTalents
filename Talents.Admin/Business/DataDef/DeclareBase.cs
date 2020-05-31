using Business.Helper;
using System;

namespace Business
{

   public partial class DeclareBase
   {

      public string DeclareTarget => DeclareBaseHelper.DeclareTarget.GetName(DeclareTargetPKID);

      public string DeclareSubject => DeclareBaseHelper.DeclareSubject.GetName(DeclareSubjectPKID);

      public string DeclareStage => DeclareBaseHelper.DeclareStage.GetName(DeclareStagePKID);

      public string RealName { get; set; }

   }


   public partial class DeclarePeriod
   {

      public bool IsInReviewPeriod => DateTime.Now >= ReveiwStartDate && DateTime.Now <= ReveiwEndDate;

      public bool IsInDeclarePeriod => DateTime.Now >= DeclareStartDate && DateTime.Now <= DeclareEndDate;

      public string AnalysisType => "Declare_Ver_1.0";

   }


   public partial class EvalPeriod
   {
      public bool IsInEvalPeriod => DateTime.Now >= AccessBeginDate && DateTime.Now <= AccessEndDate;
   }


}