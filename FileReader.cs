using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Project_Final
{
    public class FileReader
    {
        public static List<List<string>> ExcelReader(string path)
        {
            List<List<string>> retList = new List<List<string>>();
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(path);
            Excel.Worksheet worksheet = excelWorkbook.Sheets[1];

            Excel.Range sheetRange = worksheet.UsedRange;

            int sheetRowCount = sheetRange.Rows.Count;
            int sheetColCount = sheetRange.Columns.Count;

            //iterate over the rows and columns and save the values in list of strings
            //excel is not zero based!!
            for (int i = 2; i <= sheetRowCount; i++)
            {
                List<string> tempRow = new List<string>();
                for (int j = 1; j <= sheetColCount; j++)
                {
                    //new line
                    if (j == 1)
                    {
                        retList.Add(tempRow);
                        tempRow.Clear();
                    }
                    //write the value to the console

                    if (sheetRange.Cells[i, j] != null && sheetRange.Cells[i, j].Value2 != null)
                    {
                        tempRow.Add(sheetRange.Cells[i, j].Value2.ToString() + "\t");
                    }
                }

            }


            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(sheetRange);


            //close and release
            excelWorkbook.Close();
            Marshal.ReleaseComObject(excelWorkbook);

            //quit and release
            excelApp.Quit();
            Marshal.ReleaseComObject(excelApp);

            return retList;

        }


        /**
        * Read from csv.
        */
        public static List<List<string>> ReadFromCSV(string fileName)
        {
            const char delimiter = '|';

            List<List<string>> records = new List<List<string>>();

            List<string> lines = File.ReadAllLines(fileName, Encoding.UTF8).ToList();

            foreach (string recordLine in lines.Skip(1))
            {
                List<string> line = recordLine.Split(delimiter).ToList();
                records.Add(line);
            }
            return records;
        }



        public static List<Dictionary<string, string>> ReadFromCSVtoMAP(string fileName)
        {
            const char delimiter = '|';

            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();

            List<string> allLines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8).ToList();
            List<string> h = allLines[0].Split(delimiter).ToList();
            List<string> headerLine = new List<string>();
            foreach (string k in h)
            {
                headerLine.Add(k.Replace(",", ""));
            }
            int j = 0;

            foreach (string oneLine in allLines)
            {
                if (j >= 1)
                {
                    List<string> line = oneLine.Split(delimiter).ToList();
                    if (line.Count() < headerLine.Count)
                    {
                        int dif = headerLine.Count() - line.Count();
                        for (int t = 0; t < dif; t ++)
                        {
                            line.Add("");
                        }
                    }
                    Dictionary<string, string> record = new Dictionary<string, string>();
                    int i = 0;
                    foreach (string header in headerLine)
                    {
                        try
                        {
                            record[header] = line[i];
                            i++;
                        }
                        catch
                        {
                            Console.WriteLine(fileName);
                            Console.WriteLine(j);
                        }
                    }
                    records.Add(record);
                }
                j++;
            }
            return records;
        }
    
    }
}
