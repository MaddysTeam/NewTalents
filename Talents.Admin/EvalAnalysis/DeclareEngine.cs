using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

   public partial class DeclareEngine : EvalAnalysisEngine
   {
      protected const string ViewPath = "../EvalModel/Declare";

      public override string AnalysisName
         => "称号申请考核表 Ver 1.0";


      public override string DevelopKey
         => "Declare_Ver_1.0";

      public override double FullScore
          => 100;


      public override IReadOnlyDictionary<long, DeclareEvalUnitBase> DeclareEvals
      {
         get
         {
            return new List<DeclareEvalUnitBase>
            {
               new DeclareEvalUnit5004 { ViewPath=ViewPath }
            }
            .ToDictionary(x => x.TargetId);
         }
      }

   }

}