using Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace temp
{
   class Program
   {
      public static string needConvertExt = ".xlsx,.xls,.ppt,.pptx,.doc,.docx,.csv";
      public static string parentFile = @"D:\deploy\NewTalents.Admin\Attachments";

      static void Main(string[] args)
      {

         var startDate = new DateTime(2016, 1, 1);

         var diretories = Directory.GetDirectories(parentFile);
         string ext = string.Empty;
         Stream pdfStream = null;
         Stream originStream = null;
         foreach (var d in diretories)
         {
            var files = Directory.GetFiles(d);
            foreach (var f in files)
            {
               try
               {
                  ext = Path.GetExtension(f);
                  if (needConvertExt.IndexOf(ext) >= 0)
                  {
                     originStream = File.Open(f, FileMode.Open);
                     pdfStream = originStream.ConvertToPDF(ext);
                     byte[] srcBuf = new Byte[pdfStream.Length];
                     pdfStream.Seek(0, SeekOrigin.Begin);
                     pdfStream.Read(srcBuf, 0, srcBuf.Length);

                     var pdfFile = f.Replace(ext, ".pdf");
                     if (File.Exists(pdfFile))
                        File.Delete(pdfFile);

                     using (var fs = new FileStream(pdfFile, FileMode.Create, FileAccess.Write))
                     {
                        fs.Write(srcBuf, 0, srcBuf.Length);
                        fs.Close();
                     }

                     Console.WriteLine($"文件夹:{d} 中的文件：{f} :转换成功！");

                     Log($"文件夹:{d} 中的文件：{f} :转换成功！");
                  }
               }
               catch
               {
                  Console.WriteLine($"文件夹:{d} 中的文件：{f} :转换失败");

                  Log($"文件夹:{d} 中的文件：{f} :转换失败");
               }
               finally
               {
                  if (pdfStream != null)
                  {
                     pdfStream.Close();
                     pdfStream.Dispose();
                  }
                  if (originStream != null)
                  {
                     originStream.Close();
                     originStream.Dispose();
                  }
               }
            }
         }

         Console.ReadKey();
      }

      static void Log(string log)
      {
         using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"{parentFile}\log.txt", true))
         {
            file.WriteLine(log);
         }
      }
   }
}
