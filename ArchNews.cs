using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace Project_Final
{
    class ArchNews
    {
        private static int[] RecordSourceTable(Dictionary<string, string> EngRow, Dictionary<string, string> HebRow)
        {
            using (ProjectDB db = new ProjectDB())
            {

                // insert row into
                int rowID = Functions.SerialNumber(EngRow["Serial Number"]);
                int engRecordID = GetID.RecordSourchID(rowID, "ArchNewsEng");
                RecordSource ers = db.RecordSources.Find(engRecordID);
                var dateEng = Functions.CreateExcDate(EngRow["Time Downloaded"]);
                if (ers is null)
                {
                    ers = new RecordSource();
                    ers.date = dateEng;
                    ers.recordID = engRecordID;
                    ers.sourceID = rowID;
                    ers.recordSourceTable = "ArchNewsEng";
                    db.RecordSources.Add(ers);
                }
                ers.updateDate = dateEng;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "ArchNewsEng");
                }


                int hebRecordID = GetID.RecordSourchID(rowID, "ArchNewsHeb");
                RecordSource hrs = db.RecordSources.Find(hebRecordID);
                var dateHeb = Functions.CreateExcDate(HebRow["Time Downloaded"]);
                if (hrs is null)
                {
                    hrs = new RecordSource();
                    hrs.date = dateHeb;
                    hrs.recordID = hebRecordID;
                    hrs.sourceID = rowID;
                    hrs.recordSourceTable = "ArchNewsHeb";
                    db.RecordSources.Add(hrs);
                }
                hrs.updateDate = dateHeb;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "ArchNewsHeb");
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
                Functions.ErrorLogging(null, "ArchNewsInsert", 0, "Rows in hebDB and engDB are different");
            }
            else
            {

                for (int i = 94; i < engRowsNum; i++)
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
                            Functions.ErrorLogging(ex, "ArchNewsHeb", i, "SitesName - heb");
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
                            Functions.ErrorLogging(ex, "ArchNewsEng", i, "SitesName - eng");
                        }

                        Functions.InsertToConnectionTable(engID, siteNameEngID, "SiteName");


                        //Site
                        // create labels
                        double[] points = Functions.GetPoints(engRow["ITM Latitude"], engRow["ITM Longitude"], " ", " ", i, "NewsEng", false);
                        if (engRow["ITMx1"] != "")
                        {
                            points = Functions.GetPoints(engRow["ITMx1"], engRow["ITMy1"], engRow["ITMx2"],
                                engRow["ITMy2"], i, "NewsEng", true);
                        }

                        // try google points
                        if (points[0] == 0)
                        {
                            points = Functions.GetPoints(engRow["Google Latitude"], engRow["Google Longitude"], " ", " ", i, "NewsEng", true);
                        }
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
                            if (engRow["WGS84x1"] != "")
                            {
                                points = Functions.GetPoints(engRow["WGS84x1"], engRow["WGS84y1"], engRow["WGS84x2"],
                                    engRow["WGS84y2"], i, "NewsEng", true);
                            }
                            else
                            {
                                points = Functions.GetPoints(engRow["WGS84 Latitude"], engRow["WGS84 Longitude"], "", "", i, "NewsEng", true);
                            }
                            s.WGS84_Info = GeographicP.GetWGS_info(points);

                            db.Sites.Add(s);
                        }

                        else
                        {
                            // if this is a polygon
                            if (points[2] != -1)
                            {
                                Polygon polygon = new Polygon(points);
                                double[] points1 = null;
                                // if it was a polygon and now its a polygon too
                                if (s.NIG_Info.Contains("POLYGON"))
                                {
                                    Polygon p = new Polygon(s.NIG_Info);
                                    points1 = p.Include(polygon).GetPoints();
                                    Point middle = p.getMiddlePoint();
                                    s.LabelX = middle.getX();
                                    s.LabelY = middle.getY();
                                }
                                else
                                {
                                    // if now is polygon and was a point
                                    points1 = polygon.GetPoints();
                                    s.LabelX = polygon.getMiddlePoint().getX();
                                    s.LabelY = polygon.getMiddlePoint().getY();
                                }
                                var newNigInfo = GeographicP.GetNIG_info(points1);
                                SiteIDTable.InsertToSiteIDTable(siteID, s.NIG_Info, newNigInfo, engID);
                                //NIG_info
                                s.NIG_Info = newNigInfo;

                                // GS84_Info
                                s.WGS84_Info = Converters.ITMtoWGS84_info(points1);
                                // if its was a point and now its a polygon
                            }

                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "NewsEngorHeb", i, "Sites");
                        }

                        Functions.InsertToConnectionTable(engID, siteID, "Sites");

                        InsertToLocationTable(siteID, engRow["Locationiq Latitude"], engRow["Locationiq Longitude"], engRow["Google Latitude"], engRow["Google Longitude"], engRow["Coordinate Source"], i);

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
                                Functions.ErrorLogging(ex, "NewsHeb", i, "SitesSiteName Heb");
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
                                Functions.ErrorLogging(ex, "NewsEng", i, "SitesSiteName Eng");
                            }
                        }

                        Functions.InsertToConnectionTable(engID, ssnIDeng, "SiteSiteNames");

                        //Author in HEB
                        string authorName = hebRow["Publisher"];
                        int authorID = GetID.CreateAuthorId(authorName);
                        Author a = db.Authors.Find(authorID);
                        if (a is null)
                        {
                            a = new Author();
                            a.authorID = authorID;
                            a.nameHeb = authorName;
                            db.Authors.Add(a);
                        }
                        if (a.nameEng is null)
                        {
                            a.nameEng = engRow["Publisher"];
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Publisher-Author");
                        }

                        Functions.InsertToConnectionTable(hebID, authorID, "Authors");


                        //PublicationType
                        int ptID = GetID.CreatePublicationTypeId(hebRow["Type"]);
                        if (ptID != -1)
                        {
                            PublicationType pt = db.PublicationTypes.Find(ptID);
                            if (pt is null)
                            {
                                pt = new PublicationType();
                                pt.publicationTypeID = ptID;
                                pt.publicationTypeHeb = hebRow["Type"];
                                pt.publicationTypeEng = engRow["Type"];
                                db.PublicationTypes.Add(pt);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "PublicationType");
                                }
                            }
                        }
                        Functions.InsertToConnectionTable(hebID, ptID, "PublicationType");


                        // Periodical
                        //Periodical
                        Periodical per = db.Periodicals.Find(1);
                        if (per is null)
                        {
                            per = new Periodical();
                            per.periodicalID = 1;
                            per.periodicalNameHeb = "חדשות ארכיאולוגיות";
                            per.periodicalNameEng = "Hadashot Arkheologiyot: Excavations and Surveys in Israel";
                            db.Periodicals.Add(per);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Periodicals - ha");
                            }

                        }

                        // Publication
                        string biblio = "גיליון" + " " + hebRow["Volume"] + " לשנת" + " " + hebRow["Year"];
                        int pubID = GetID.CreatePublicationId(biblio, siteNameHeb, hebRow["Intro"]);
                        Publication pub = db.Publications.Find(pubID);
                        if (pub is null)
                        {
                            pub = new Publication();
                            pub.publicationID = pubID;
                            pub.periodicalID = 1;
                            pub.articleTitle = siteNameHeb;
                            pub.reference = biblio;
                            pub.year = hebRow["Year"];
                            db.Publications.Add(pub);

                        }
                        pub.abstractEng = engRow["Intro"];
                        pub.abstractHeb = hebRow["Intro"];
                        pub.URL = hebRow["url"];
                        if (ptID > 0) pub.publicationTypeID = ptID;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Publication (cols- Intro, URL, year, volume, pubType, periodicalsID)");
                        }

                        Functions.InsertToConnectionTable(hebID, pubID, "Publications");


                        //AuthorsPublications
                        int auPubID = GetID.CreateConnectionID(authorID, pubID, "authors_publication");
                        AuthorsPublication ap = db.AuthorsPublications.Find(auPubID, authorID, pubID);
                        if (ap is null)
                        {
                            ap = new AuthorsPublication();
                            ap.authorPublicationID = auPubID;
                            ap.authorID = authorID;
                            ap.publicationID = pubID;
                            db.AuthorsPublications.Add(ap);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Author-Publication");
                            }
                        }

                        Functions.InsertToConnectionTable(hebID, auPubID, "AuthorsPublications");

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

                            //excavationNIG_info
                            e.excavationNIG_Info = GeographicP.GetNIG_info(points);

                            //excavationGS84_Info
                            e.excavationWGS84_Info = Converters.ITMtoWGS84_info(points);

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
                        e.excavationDate = Functions.CreateExcDate(engRow["Date"]);
                        Functions.InsertToConnectionTable(hebID, eID, "Excavations");


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
                                Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "Excavations-Publications");
                            }
                        }
                        Functions.InsertToConnectionTable(hebID, epID, "ExcavationsPublications");

                    }

                }
            }
        }

        // locationAPI
        private static void InsertToLocationTable(int siteID, string loc1, string loc2, string google1, string google2, string cs, int i)
        {
            using (ProjectDB db = new ProjectDB())
            {
                LocationAPIResult l = db.LocationAPIResults.Find(siteID);
                if (l is null)
                {
                    double l1 = 0;
                    double l2 = 0;
                    double go1 = 0;
                    double go2 = 0;
                    try
                    {
                        l1 = double.Parse(loc1);
                    }
                    catch { }
                    try
                    {
                        l2 = double.Parse(loc2);
                    }
                    catch { }
                    try
                    {
                        go1 = double.Parse(google1);
                    }
                    catch { }
                    try
                    {
                        go2 = double.Parse(google2);
                    }
                    catch { }

                    if (l1 > 0 || l2 > 0 || go1 > 0 || go2 > 0)
                    {
                        l = new LocationAPIResult();
                        l.siteID = siteID;
                        l.coordinateSource = cs;
                        if (l1 > 0) l.locationiqLat = l1;
                        if (l2 > 0) l.locationiqLon = l2;
                        if (go1 > 0) l.googleLat = go1;
                        if (go2 > 0) l.googleLon = go2;
                        db.LocationAPIResults.Add(l);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "NewsEngOrHeb", i, "LocationAPIResult");
                        }
                    }
                }
            }
        }
    }
}
