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

      public class DeclareEvalUnit5004 : DeclareEvalUnit
      {

         public override long TargetId => 5004;


         protected override void AnalysisResult(FormCollection fc,EvalDeclareResult result,Dictionary<string,EvalDeclareResultItem> items)
         {
            // implement analyize result

            var score1 = fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk_Def];

            result.Score = 1;
         }

      }

   }

}