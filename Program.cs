using Microsoft.Office.Interop.Excel;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using EllisWeb.Gematria;
using System.Windows.Controls;
using System.Security.Policy;
using System.Device.Location;

namespace Project_Final
{
    class Program
    {
        static void Main(string[] args)
        {
            //var a = "    mishel      h";
            //Console.WriteLine(a);
            //Console.WriteLine(GetID.deleteSpaces(a));
            var Periods = FileReader.ExcelReader(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\periods1.xlsx");

            var Years = FileReader.ExcelReader(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\years.xlsx");
            var HA = FileReader.ExcelReader(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\HazalaRecordsWithNoaFixes.xlsx");
            //InsertToTable.RecordSourceTable(Periods, null, "P");
            InsertToTable.RecordSourceTable(HA, Years, "HR");


            var HebDBNews = FileReader.ReadFromCSVtoMAP(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\Archeological News\Filtered\HebrewDatabase.csv");
            var EngDBNews = FileReader.ReadFromCSVtoMAP(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\Archeological News\Filtered\EnglishDatabase.csv");
            ArchNews.Insert(EngDBNews, HebDBNews);



            var EngDBS = FileReader.ReadFromCSVtoMAP(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\Survey Antiquities\Filtered\EnglishDatabase.csv");
            var HebDBS = FileReader.ReadFromCSVtoMAP(@"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\Survey Antiquities\Filtered\HebrewDatabase.csv");
            SurveyAntiquities.Insert(EngDBS, HebDBS);


            var PARK = FileReader.ReadFromCSVtoMAP(@"ArcheologicalPark_Database.csv");
            ArcheologicalPark.Insert(PARK);

            var ATIKOT = FileReader.ReadFromCSVtoMAP(@"Atiqot_EnglishDatabase.csv");
            Atikot.Insert(ATIKOT);

            var JSTOR_NEWS_61_98 = FileReader.ReadFromCSVtoMAP(@"JSTOR_ArcheologicalNews(1961-1998)_EnglishDatabase.csv");
            JSTOR_news_61_98.Insert(JSTOR_NEWS_61_98);

            var JSTOR_NEWS_99_19 = FileReader.ReadFromCSVtoMAP(@"JSTOR_ArcheologicalNews(1999-2019)_EnglishDatabase.csv");
            JSTOR_news_61_98.Insert(JSTOR_NEWS_99_19);


            var JSTOR_ATIKOT_55_90 = FileReader.ReadFromCSVtoMAP(@"JSTOR_Atiqot(1955-1990)_EnglishDatabase.csv");
            JSTOR_Atikot_55_90.Insert(JSTOR_ATIKOT_55_90);

            var JSTOR_ARIKOT_91 = FileReader.ReadFromCSVtoMAP(@"JSTOR_Atiqot(1991-)_EnglishDatabase.csv");
            JSTOR_Atikot_91.Insert(JSTOR_ARIKOT_91);

            var JSTOR = FileReader.ReadFromCSVtoMAP(@"JSTOR_BASOR_EnglishDatabase.csv");
            JSTOR_BASOR.Insert(JSTOR);

            //for (int i = 0; i < EngDBS.Count; i++)
            //{
            //    Dictionary<string, string> engRow = EngDBS[i];
            //    Dictionary<string, string> hebRow = HebDBS[i];

            //    using (ProjectDB db = new ProjectDB())
            //    {

            //        // Periods
            //        //split the period by ","
            //        string period = engRow["Period"];
            //        var a = db.Periods.Find(76);
            //        var b = db.Periods.Find(77);
            //        List<int> periodsID = new List<int>();
            //        if (period != "")
            //        {
            //            string[] split1 = period.Split(',');
            //            string[] split2 = hebRow["Period"].Split(',');
            //            int j = 0;
            //            foreach (string p in split1)
            //            {
            //                var newP = p;
            //                if (p.Contains("Age"))
            //                {
            //                    newP = p.Replace("Age ", "");
            //                }
            //                int perID = GetID.CreatePeriodId(newP);
            //                periodsID.Add(perID);
            //                Period per = db.Periods.Find(perID);
            //                if (per is null)
            //                {
            //                    per = new Period();
            //                    per.periodID = perID;
            //                    per.nameEng = newP;
            //                    per.nameHeb = split2[j];
            //                    db.Periods.Add(per);
            //                    try
            //                    {
            //                        db.SaveChanges();
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Periods");
            //                    }

            //                }

            //                j++;
            //            }
            //        }
            //    }

            //double[] d = { 1, 4000, 4000, 1 };
            //double[] d1 = { 5, 4, 8, 1 };
            //double[] d2 = { 0, 5, 5, 0 };
            //System.Data.Entity.Spatial.DbGeography dbGeography = Converters.ITMtoWGS84_info(d);
            //System.Data.Entity.Spatial.DbGeography dbGeography1 = Converters.ITMtoWGS84_info(d);
            //System.Data.Entity.Spatial.DbGeography dbGeography2 = Converters.ITMtoWGS84_info(d1);
            //System.Data.Entity.Spatial.DbGeography dbGeography3 = Converters.ITMtoWGS84_info(d2);

            //Console.WriteLine(dbGeography);
            //Console.WriteLine(dbGeography2);
            //var a = dbGeography2.Union(dbGeography);

            ////Console.WriteLine(a);
            //while (true) { };

            //int a;
            //Dictionary<string, int> sitesName = new Dictionary<string, int>();
            //Console.OutputEncoding = Encoding.GetEncoding("Windows-1255");
            //sitesName.Add("מישל", 0);
            //sitesName.Add("דניאל", 0);
            //sitesName.Add("ג'ור", 7);
            //Console.WriteLine(sitesName.ContainsKey("גכ"));


            //while (true) { };
        }

    }
    
  

}
