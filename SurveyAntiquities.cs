using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_Final
{
    class SurveyAntiquities
    {
        private static int[] RecordSourceTable(Dictionary<string, string> EngRow, Dictionary<string, string> HebRow)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int rowID = Functions.SerialNumber(EngRow["Serial Number"]);
                int engRecordID = GetID.RecordSourchID(rowID, "SurveyAntiquitiesEng");
                RecordSource ers = db.RecordSources.Find(engRecordID);
                var dateEng = Functions.CreateExcDate(EngRow["Time Downloaded"]);
                if (ers is null)
                {
                    ers = new RecordSource();
                    ers.date = dateEng;
                    ers.recordID = engRecordID;
                    ers.sourceID = rowID;
                    ers.recordSourceTable = "SurveyAntiquitiesEng";
                    db.RecordSources.Add(ers);
                }
                ers.updateDate = dateEng;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "SurveyAntiquitiesEng");
                }



                int hebRecordID = GetID.RecordSourchID(rowID, "SurveyAntiquitiesHeb");
                RecordSource hrs = db.RecordSources.Find(hebRecordID);
                var dateHeb = Functions.CreateExcDate(HebRow["Time Downloaded"]);
                if (hrs is null)
                {
                    hrs = new RecordSource();
                    hrs.date = dateHeb;
                    hrs.recordID = hebRecordID;
                    hrs.sourceID = rowID;
                    hrs.recordSourceTable = "SurveyAntiquitiesHeb";
                    db.RecordSources.Add(hrs);
                }
                hrs.updateDate = dateHeb;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "SurveyAntiquitiesHeb");
                }
                int[] ret = { engRecordID, hebRecordID };
                return ret;
            }
        }


        public static void Insert(List<Dictionary<string, string>> EngDB, List<Dictionary<string, string>> HebDB)
        {
            int engRowsNum = EngDB.Count();
            int hebRowsNum = HebDB.Count();
            if (engRowsNum != hebRowsNum)
            {
                Functions.ErrorLogging(null, "SurveyAntiquitiesInsert", 0, "Rows in hebDB and engDB are different");
            }
            else
            {

                for (int i = 0; i < engRowsNum; i++)
                {
                    Dictionary<string, string> engRow = EngDB[i];
                    Dictionary<string, string> hebRow = HebDB[i];

                    int[] ID = RecordSourceTable(engRow, hebRow);
                    int engID = ID[0];
                    int hebID = ID[1];
                    using (ProjectDB db = new ProjectDB())
                    {
                        //Heb SiteName
                        string siteNameHeb = hebRow["Site Name"];
                        int siteNameID = GetID.CreateSiteNameId(siteNameHeb);
                        SitesName sn = db.SitesNames.Find(siteNameID);
                        if (sn is null)
                        {
                            sn = new SitesName();
                            sn.siteNameID = siteNameID;
                            sn.txtName = siteNameHeb;
                            sn.languageID = 1;
                            db.SitesNames.Add(sn);
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "SurveyAntiquitiesHeb", i, "SitesName - heb");
                        }
                        Functions.InsertToConnectionTable(hebID, siteNameID, "SiteName");

                        // Eng SiteName
                        string siteNameEng = engRow["Site Name"];
                        int siteNameEngID = GetID.CreateSiteNameId(siteNameEng);
                        SitesName sne = db.SitesNames.Find(siteNameEngID);
                        if (sne is null)
                        {
                            sne = new SitesName();
                            sne.siteNameID = siteNameEngID;
                            sne.txtName = siteNameEng;
                            sne.languageID = 2;
                            db.SitesNames.Add(sne);
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "SurveyAntiquitiesEng", i, "SitesName - eng");
                        }

                        Functions.InsertToConnectionTable(engID, siteNameEngID, "SiteName");


                        //Site
                        // create labels
                        double[] points = Functions.GetPoints(engRow["ITMLatitude"], engRow["ITMLongitude"], " ", " ", i, "SurveyAntiquitiesEng", false);
                        double[] labales = Functions.CreateLabels(points);
                        double x = labales[0];
                        double y = labales[1];


                        // if the site name already exist in table, find the siteID with this name, else, create new Site
                        int siteID = GetID.GetSiteID(points);
                        Site s = db.Sites.Find(siteID);
                        //if it new site
                        if (s is null)
                        {
                            s = new Site();
                            s.siteID = siteID;
                            s.LabelX = x;
                            s.LabelY = y;
                            //NIG_info
                            s.NIG_Info = GeographicP.GetNIG_info(points);

                            // GS84_Info
                            points = Functions.GetPoints(engRow["WGS84Latitude"], engRow["WGS84Longitude"], "", "", i, "SurveyAntiquitiesEng", true);
                            s.WGS84_Info = GeographicP.GetWGS_info(points);

                            db.Sites.Add(s);
                        }
                        

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "SurveyAntiquitiesEngEngorHeb", i, "Sites");
                        }

                        Functions.InsertToConnectionTable(engID, siteID, "Sites");

                        //SiteSiteName
                        //HEB
                        int ssnIDheb = GetID.CreateConnectionID(siteID, siteNameID, "sites_site_name");
                        SitesSiteName ssnH = db.SitesSiteNames.Find(ssnIDheb, siteID, siteNameID);
                        if (ssnH is null)
                        {
                            ssnH = new SitesSiteName();
                            ssnH.sitesSiteNamesID = ssnIDheb;
                            ssnH.siteID = siteID;
                            ssnH.siteNameID = siteNameID;
                            db.SitesSiteNames.Add(ssnH);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "SurveyAntiquitiesEngHeb", i, "SitesSiteName Heb");
                            }
                        }

                        Functions.InsertToConnectionTable(hebID, ssnIDheb, "SiteSiteNames");


                        //ENG
                        int ssnIDeng = GetID.CreateConnectionID(siteID, siteNameEngID, "sites_site_name");
                        SitesSiteName ssnE = db.SitesSiteNames.Find(ssnIDeng, siteID, siteNameEngID);
                        if (ssnH is null)
                        {
                            ssnE = new SitesSiteName();
                            ssnE.sitesSiteNamesID = ssnIDeng;
                            ssnE.siteID = siteID;
                            ssnE.siteNameID = siteNameEngID;
                            db.SitesSiteNames.Add(ssnE);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "SurveyAntiquitiesEngEng", i, "SitesSiteName Eng");
                            }
                        }

                        Functions.InsertToConnectionTable(engID, ssnIDeng, "SiteSiteNames");


                        // FindingType
                        List<int> rellevantFtID = new List<int>();
                        string[] finding_types = hebRow["Remains"].Split(',');
                        foreach (string ft_str in finding_types)
                        {
                            int ftID = GetID.CreateFindingTypeId(ft_str);
                            // save all ftID in one list
                            rellevantFtID.Add(ftID);
                            FindingType ft = db.FindingTypes.Find(ftID);
                            if (ft is null)
                            {
                                ft = new FindingType();
                                ft.findingTypeID = ftID;
                                ft.findingTypeDescriptionHeb = ft_str;
                                db.FindingTypes.Add(ft);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Functions.ErrorLogging(ex, "SurveyAntiquitiesEngEng", i, "FindingTypeHeb");
                                }
                            }
                            Functions.InsertToConnectionTable(hebID, ftID, "FindingType");
                        }


                        string biblio = hebRow["Bibliography"];
                        //Excavations
                        int eID = GetID.CreateExcavationID(siteNameHeb, siteNameEng, siteID, biblio);
                        Excavation e = db.Excavations.Find(eID);
                        if (e is null)
                        {
                            e = new Excavation();
                            e.excavationID = eID;
                            e.excavationHeb = siteNameHeb;
                            e.excavationEng = siteNameEng;
                            e.bibliography = biblio;
                            e.siteID = siteID;
                            db.Excavations.Add(e);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Excavations- siteNameHeb&Eng, siteID, bibilio, date");
                            }
                        }

                        //NIG_info
                        e.excavationNIG_Info = GeographicP.GetNIG_info(points);

                        // GS84_Info
                        points = Functions.GetPoints(engRow["WGS84Latitude"], engRow["WGS84Longitude"], "", "", i, "SurveyAntiquitiesEng", true);
                        e.excavationWGS84_Info = GeographicP.GetWGS_info(points);

                        Functions.InsertToConnectionTable(hebID, eID, "Excavations");

                        // SiteType
                        SiteType st = db.SiteTypes.Find(0);
                        SiteNature snat = db.SiteNatures.Find(0);
                        if (st is null)
                        {
                            st = new SiteType();
                            st.findingSiteTypeID = 0;
                            st.findingSiteTypeEng = "TEMP";
                            db.SiteTypes.Add(st);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "siteType");
                            }
                        }

                        Functions.InsertToConnectionTable(hebID, 0, "FindingSiteType");

                        // site Nature
                        if (snat is null)
                        {
                            snat = new SiteNature();
                            snat.findingSiteNatureID = 0;
                            snat.findingSiteNatureEng = "TEMP";
                            db.SiteNatures.Add(snat);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "siteNature");
                            }
                        }

                        Functions.InsertToConnectionTable(hebID, 0, "FindingSiteNature");

                        //Findings
                        int j = 0;
                        List<int> findingsID = new List<int>();
                        foreach (int f_typeID in rellevantFtID)
                        {
                            int fID = GetID.CreateFindingId(0, 0, f_typeID, eID, "");
                            findingsID.Add(fID);
                            Finding f = db.Findings.Find(fID);
                            if (f is null)
                            {
                                f = new Finding();
                                f.findingID = fID;
                                f.findingTypeID = f_typeID;
                                f.siteTypeID = 0;
                                f.siteNatureID = 0;
                                f.excavationID = eID;
                                // comment???
                                f.descriptionHeb = finding_types[j];
                                db.Findings.Add(f);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Findings");
                                }
                            }
                            j++;

                            Functions.InsertToConnectionTable(hebID, fID, "Findings");
                        }


                        // Periods
                        //split the period by ","
                        string period = engRow["Period"];
                        List<int> periodsID = new List<int>();
                        if (period != "")
                        {
                            string[] split1 = period.Split(',');
                            string[] split2 = hebRow["Period"].Split(',');
                            j = 0;
                            foreach (string p in split1)
                            {
                                var newP = p;
                                if (p.Contains("Age"))
                                {
                                    newP = p.Replace("Age ", "");
                                }
                                int perID = GetID.CreatePeriodId(newP, true);
                                periodsID.Add(perID);
                                Period per = db.Periods.Find(perID);
                                if (per is null)
                                {
                                    per = new Period();
                                    per.periodID = perID;
                                    per.nameEng = newP;
                                    per.nameHeb = split2[j];
                                    db.Periods.Add(per);
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Periods");
                                    }

                                }

                                j++;
                            }




                            // Finding periods
                            foreach (int fID in findingsID)
                            {
                                foreach (int perID in periodsID)
                                {
                                    int fpID = GetID.CreateConnectionID(fID, perID, "findings_periods");
                                    FindingsPeriod fp = db.FindingsPeriods.Find(fpID, fID, perID);
                                    if (fp is null)
                                    {
                                        fp = new FindingsPeriod();
                                        fp.findingPeriodID = fpID;
                                        fp.findingID = fID;
                                        fp.periodID = perID;
                                        db.FindingsPeriods.Add(fp);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Findings Periods");
                                        }
                                    }

                                    Functions.InsertToConnectionTable(hebID, fpID, "FindingsPeriods");
                                }
                            }


                            //Periodical
                            Periodical periodical = db.Periodicals.Find(2);
                            if (periodical is null)
                            {
                                periodical = new Periodical();
                                periodical.periodicalID = 2;
                                periodical.periodicalNameHeb = "אתר הסקר הארכיאולוגי של ישראל, רשות העתיקות";
                                periodical.periodicalNameEng = "The Archeological Survey of Israel, Israel Antiquities Authority";
                                db.Periodicals.Add(periodical);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Periodicals - ha");
                                }

                            }


                            // Publication
                            int pubID = GetID.CreatePublicationId(biblio, siteNameHeb, hebRow["Description"]);
                            Publication pub = db.Publications.Find(pubID);
                            if (pub is null)
                            {
                                pub = new Publication();
                                pub.publicationID = pubID;
                                pub.periodicalID = 2;
                                pub.articleTitle = siteNameHeb;
                                pub.bibliography = biblio;
                                db.Publications.Add(pub);

                            }
                            pub.abstractEng = engRow["Description"];
                            pub.abstractHeb = hebRow["Description"];


                            pub.URL = hebRow["Article's Link"];
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Publication (cols- Intro, URL, year, volume, pubType, periodicalsID)");
                            }

                            Functions.InsertToConnectionTable(hebID, pubID, "Publications");




                            // Excavations_publications
                            int epID = GetID.CreateConnectionID(eID, pubID, "excavations_publications");
                            ExcavationsPublication ep = db.ExcavationsPublications.Find(epID, eID, pubID);
                            if (ep is null)
                            {
                                ep = new ExcavationsPublication();
                                ep.excavationID = eID;
                                ep.excavationPublicationID = epID;
                                ep.publicationID = pubID;
                                db.ExcavationsPublications.Add(ep);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Functions.ErrorLogging(ex, "SurveyEngOrHeb", i, "Excavations-Publications");
                                }
                            }
                            Functions.InsertToConnectionTable(hebID, epID, "ExcavationsPublications");



                            // Sites Periods
                            foreach (int periID in periodsID)
                            {
                                int sitePerID = GetID.CreateConnectionID(siteID, periID, "sites_periods");
                                SitesPeriod sp = db.SitesPeriods.Find(sitePerID, siteID, periID);
                                if (sp is null)
                                {
                                    sp = new SitesPeriod();
                                    sp.sitesPeriodsID = sitePerID;
                                    sp.siteID = s.siteID;
                                    sp.periodID = periID;
                                    db.SitesPeriods.Add(sp);
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Functions.ErrorLogging(ex, "SurveyEngOrHeb", hebID, "Sites Periods");
                                    }
                                }

                                Functions.InsertToConnectionTable(hebID, sitePerID, "SitesPeriods");
                            }
                        }


                    }
                }
            }
        }
    }
}
