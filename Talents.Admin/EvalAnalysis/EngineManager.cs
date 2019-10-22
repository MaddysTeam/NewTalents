using Business;
using System.Collections.Generic;

namespace TheSite.EvalAnalysis
{

   public static class EngineManager
   {

      private static Dictionary<string, EvalAnalysisEngine> engines;


      static EngineManager()
      {
         engines = new Dictionary<string, EvalAnalysisEngine>();

         // 年度考评
         var annual = new AnnualEngine();
         engines.Add(annual.DevelopKey, annual);

         // 称号考评
         var declare = new DeclareEngine();
         engines.Add(declare.DevelopKey, declare);
      }


      public static IReadOnlyDictionary<string, EvalAnalysisEngine> Engines
         => engines;


      public static EvalSchoolResultItem GetEvalResult(Dictionary<string, EvalSchoolResultItem> exist, string key, string defaultValue = "A")
      {
         var defaultResult = new EvalSchoolResultItem { ChooseValue = defaultValue, ResultValue = string.Empty };
         if (exist == null)
            return defaultResult;
         if (exist.ContainsKey(key))
            return exist[key];
         return defaultResult;
      }

      public static EvalQualityResultItem GetEvalResult(Dictionary<string, EvalQualityResultItem> exist, string key, string defaultValue = "1A")
      {
         var defaultResult = new EvalQualityResultItem { ChooseValue = defaultValue, ResultValue = string.Empty };
         if (exist == null)
            return defaultResult;
         if (exist.ContainsKey(key))
            return exist[key];
         return defaultResult;
      }


      public static string GetEvalResultValue(List<EvalSchoolResultItem> exist, string key, string defaultValue = "0分")
      {
         if (exist == null)
            return defaultValue;
         if (exist.Exists(m => m.EvalItemKey == key))
            return exist.Find(m => m.EvalItemKey == key).ResultValue;
         return defaultValue;
      }


      public static string GetEvalChooseValue(Dictionary<string, EvalSchoolResultItem> exist, string key, string defaultValue = "A")
      {
         if (exist == null)
            return defaultValue;
         if (exist.ContainsKey(key))
            return exist[key].ChooseValue;
         return defaultValue;
      }


      public static EvalVolumnResultItem GetVolumnResultItem(Dictionary<string, EvalVolumnResultItem> exist, string key, string defaultValue = "A")
      {
         var defaultResult = new EvalVolumnResultItem { ChooseValue = defaultValue, ResultValue = string.Empty };
         if (exist == null)
            return defaultResult;
         if (exist.ContainsKey(key))
            return exist[key];
         return defaultResult;
      }


      public static string GetEvalChooseValue(Dictionary<string, EvalVolumnResultItem> exist, string key, string defaultValue = "A")
      {
         if (exist == null)
            return defaultValue;
         if (exist.ContainsKey(key))
            return exist[key].ChooseValue;
         return defaultValue;
      }


      public static string GetEvalChooseValue(Dictionary<string, EvalQualityResultItem> exist, string key, string defaultValue = "1A")
      {
         if (exist == null)
            return defaultValue;
         if (exist.ContainsKey(key))
            return exist[key].ChooseValue;
         return defaultValue;
      }


      public static string GetQualityEvalItem(Dictionary<string, EvalQualityResultItem> exist, string key, string defaultValue = "1A")
      {
         if (exist == null)
            return defaultValue;
         if (exist.ContainsKey(key))
            return exist[key].ChooseValue;
         return defaultValue;
      }

      public static string GetDeclareEvalChooseValue(Dictionary<string, EvalDeclareResultItem> exist, string key, string defaultValue = "A")
      {
         if (exist == null)
            return defaultValue;
         if (exist.ContainsKey(key))
            return exist[key].ChooseValue;
         return defaultValue;
      }

      public static EvalDeclareResultItem GetDeclareEvalResult(Dictionary<string, EvalDeclareResultItem> exist, string key, string defaultValue = "A")
      {
         var defaultResult = new EvalDeclareResultItem { ChooseValue = defaultValue, ResultValue = string.Empty };
         if (exist == null)
            return defaultResult;
         if (exist.ContainsKey(key))
            return exist[key];

         return defaultResult;
      }

   }

}