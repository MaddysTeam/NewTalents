using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

   public partial class DeclareEngine
   {

      public abstract class DeclareEvalUnit : DeclareEvalUnitBase
      {
         public override string EvalView => "";


         public override double FullScroe => 100;


         public override double Proportion => 1;


         public override string ResultView => "";


         public override string RuleView => "";


         public override long Eval(APDBDef db, DecalreEvalParam param, FormCollection fc)
         {
            throw new NotImplementedException();
         }

         public override Dictionary<string, EvalDeclareResultItem> GetResultItem(APDBDef db, DecalreEvalParam param)
         {
            throw new NotImplementedException();
         }
      }

   }

}