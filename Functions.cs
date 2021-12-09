using EllisWeb.Gematria;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    class Functions
    {
        public static double[] CreateLabels (double[] points)
        {

            double x1 = points[0];
            double y1 = points[1];
            double x2 = points[2];
            double y2 = points[3];
            double midX = x1;
            double midY = y1;
            if (x2 > -1 && y2 > -1)
            {
                midX = (x1 + x2) / 2;
                midY = (y1 + y2) / 2;
            }
            double[] ret = { midX, midY };
            return ret;
        }


        public static double[] GetPoints (string lx1, string ly1, string lx2, string ly2, int recordID, string tablename, bool er)
        {
            bool flag = false;
            if (lx2.Contains("NULL") && ly2.Contains("NULL")) flag = true;
            // if one of values is null.
            if (ly2.Contains("NULL") && flag == false) ly2 = ly1;
            if (lx2.Contains("NULL") && flag == false) lx2 = lx1;
            double y1 = -1;
            double x1 = -1;
            double y2 = -1;
            double x2 = -1;
            try
            {
                x1 = double.Parse(lx1);
                y1 = double.Parse(ly1);
                x2 = double.Parse(lx2);
                y2 = double.Parse(ly2);
            }
            catch
            {
                try
                {
                    x1 = double.Parse(lx1);
                    y1 = double.Parse(ly1);
                }
                catch (Exception e)
                {
                    if (er) ErrorLogging(e, tablename, recordID, "location x1,y1,x2,y2");
                    double[] retNull = { 0, 0, 0, 0 };
                    return retNull;
                }
            }

            double[] ret = { x1, y1, x2, y2 };
            return ret;
        }



        public static double[] GetPoints(string lx1, string ly1, string lx2, string ly2, string lx3, string ly3, string lx4, string ly4, int recordID, string tablename)
        {

            double y1 = -1;
            double x1 = -1;
            double y2 = -1;
            double x2 = -1;
            double y3 = -1;
            double x3 = -1;
            double y4 = -1;
            double x4 = -1;
            try
            {
                x1 = double.Parse(lx1);
                y1 = double.Parse(ly1);
                x2 = double.Parse(lx2);
                y2 = double.Parse(ly2);
                x3 = double.Parse(lx3);
                y3 = double.Parse(ly3);
                x4 = double.Parse(lx4);
                y4 = double.Parse(ly4);
            }
            catch (Exception e)
            {
                ErrorLogging(e, tablename, recordID, "Convert point string to double");
            }

            double[] ret = { x1, y1, x2, y2, x3, y3, x4, y4 };
            return ret;
        }




        // create excavation date
        public static DateTime CreateExcDate(string dateInput)
        {
            DateTime parsedDate;
            // replace "/" to dot.
            dateInput = dateInput.Replace("/", ".");

            try
            {
                parsedDate = DateTime.Parse(dateInput);
                
            }
            catch 
            {
                try
                {
                    parsedDate = DateTime.ParseExact(dateInput, "dd.mm.yyyy", CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    dateInput = "1." + dateInput;
                    try
                    {
                        parsedDate = DateTime.Parse(dateInput);
                    }
                    catch
                    {

                        return DateTime.Parse("1.1.1111");
                    }
                }
            }
            return parsedDate;
        }




        // Get str value, delete all non-rellevant chars and convert it to int.
        public static int ConvertStrToInt(string str)
        {
            string string_of_ints = "";
            foreach (char c in str)
            {
                if (Char.IsNumber(c))
                {
                    string_of_ints += c;
                }
            }
            int a = -1;
            try
            {
                 a = int.Parse(string_of_ints);
            } catch
            {
                a = -1;
            }
            return a;
        }



        // Write the error in log file
        public static void ErrorLogging(Exception ex, string table, int rowID, string value)
        {
            string strPath = @"C:\Users\mfuzailo\OneDrive - Intel Corporation\Desktop\stud\project\Bar-Ilan project\Log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                if (ex != null)
                {
                    sw.WriteLine("Error Message: " + ex.Message);
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                }
                sw.WriteLine("Problem Table: " + table);
                sw.WriteLine("Problem row: " + rowID.ToString());
                sw.WriteLine("Problem value: " + value);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
        }

        // Get string of bibliograpy and convert it to string[]
        public static string[] Bibliography(string str, int recordID)
        {
            try
            {
                string[] retVal = new string[5];
                var ha = str.Split(',');
                var haNum = ha[0].Split();
                int haNumL = haNum.Length;
                for (int h = 0; h < haNumL; h++)
                {
                    if (haNum[h] == "" && h < haNumL - 1)
                    {
                        haNum[h] = haNum[h + 1];
                        haNum[h + 1] = "";
                    }
                }
                var pages = ha[1].Split();
                int pNum = pages.Length;
                for (int g = 0; g < pNum; g++)
                {
                    if (pages[g] == "" && g < pNum - 1)
                    {
                        pages[g] = pages[g + 1];
                        pages[g + 1] = "";
                    }
                }
                // check id the values are numbert or hebrew chars
                int itsAnum = ConvertStrToInt(haNum[1]);
                string temp = haNum[1];
                retVal[2] = haNum[1];
                // if its heb, convert it to str
                if (itsAnum == -1)
                {
                    temp = GetGematiaVal(haNum[1]);
                }
                retVal[0] = temp;
                retVal[1] = pages[1];
                retVal[3] = haNum[0];
                retVal[4] = pages[0];

                return retVal;
            } catch (Exception ex)
            {
                ErrorLogging(ex, "HazalaRecords", recordID, "Bibliography");
            }
            string[] ret = { "NULL", "NULL", "NULL", "NULL", "NULL" };
            return ret;
        }



        public static string GetGematiaVal (string str)
        {
            string temp = "";
            string gematria = "";
            var ha_from_to = str.Split('-');
            int numOfFromTo = ha_from_to.Length;
            for (int i = 0; i < numOfFromTo; i++)
            {
                gematria = Calculator.GetGematriaValue(ha_from_to[i]).ToString();
                temp += gematria;
                if (i < numOfFromTo - 1) { temp += "-"; }
            }

            return temp;
        }


        public static string FindYear(List<List<string>> years, string str, string num)
        {
            string heb = str.Split('-')[0];
            string numYear = num.Split('-')[0];
            foreach (List<string> year in years)
            {
                if (year[0] == heb || GetGematiaVal(year[0]) == numYear) 
                {
                    return year[1];
                }
            }

            return "NULL";
        }


        public static string FindPeriodIDByYear(List<List<string>> years, string y)
        {
            foreach (List<string> year in years)
            {
                if (year[1] == y)
                {
                    return year[0];
                }
            }

            return "NULL";
        }


        public static void InsertToConnectionTable(int recordID, int dbID, string tableName)
        {
            using (ProjectDB db = new ProjectDB())
            {
                // Get table id, if it not exist, create new one
                int tableID = GetID.TableID(tableName);
                TableCode tc = null;
                if (tableID == -1)
                {
                    tc = new TableCode();
                    tc.tableName = tableName;
                    db.TableCodes.Add(tc);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "TableCode", recordID, "Table Code");
                    }
                    tableID = tc.tableID;
                }

                // connect
                int connID = GetID.ConnectionTableID(dbID, tableID, recordID);
                if (db.SourceDBConnections.Find(connID) is null)
                {
                    SourceDBConnection sdbc = new SourceDBConnection();
                    sdbc.ID = connID;
                    sdbc.dbID = dbID;
                    sdbc.sourceID = recordID;
                    sdbc.tableID = tableID;
                    db.SourceDBConnections.Add(sdbc);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "ConnectionTableID", recordID, "ConnectionTableID");
                    }
                }

            }
        }


        public static int SerialNumber(string str)
        {
            int strLen = str.Length;
            int retVal = 0;
            for (int i = 0; i < strLen - 1; i++)
            {
                try
                {
                    retVal = int.Parse(str);
                } catch
                {
                    str = str.Substring(1, str.Length - 1);
                }
            }
            return retVal;
        }


    }
}
