using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
namespace Project_Final
{
    class InsertToTable
    {
        public static void RecordSourceTable(List<List<string>> rows, List<List<string>> years, string sourchTableName)
        {
            // check if not null
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (sourchTableName is null)
            {
                throw new ArgumentNullException(nameof(sourchTableName));
            }

            using (ProjectDB db = new ProjectDB())
            {
                int num = rows.Count();
                for (int i = 0; i < num; i++)
                {
                    Boolean rsFlag = false;
                    // insert row into
                    int rowID = int.Parse(rows[i][0]);
                    int recordID = GetID.RecordSourchID(rowID, sourchTableName);
                    rows[i][1] = recordID.ToString();
                    RecordSource rs = db.RecordSources.Find(recordID);
                    if (rs is null)
                    {
                        rsFlag = true;
                        rs = new RecordSource();
                        rs.date = DateTime.Now;
                        rs.recordID = recordID;
                        rs.sourceID = rowID;
                        rs.recordSourceTable = sourchTableName;
                    }
                    rs.updateDate = DateTime.Now;
                    if (rsFlag) { db.RecordSources.Add(rs); }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "rowID");
                    }
                }
            }

            if (sourchTableName == "HR")
            {
                HazalaRecords(rows, years);
            }

            if (sourchTableName == "P")
            {
                Periods(rows);
            }
        }



        private static void HazalaRecords(List<List<string>> rows, List<List<string>> years)
        {

            List<int> sitesIDinThisTable = new List<int>();
            // check if not null
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            using (ProjectDB db = new ProjectDB())
            {
                int num = rows.Count();
                for (int i = 10; i < num; i++)
                {

                    List<string> hazalRow = rows[i];

                    int recordID = int.Parse(hazalRow[1]);
                    // create labels
                    double[] points = Functions.GetPoints(hazalRow[24], hazalRow[25], hazalRow[26], hazalRow[27], i + 1, "HazalaRecords", true);
                    double[] labales = Functions.CreateLabels(points);
                    double x = labales[0];
                    double y = labales[1];
                    string siteName = hazalRow[4];

                    // find if siteID is already exist in site table
                    int siteID = int.Parse(hazalRow[2]);
                    Site s = db.Sites.Find(siteID);
                    if (s is null)
                    {
                        // add this siteID to this list
                        sitesIDinThisTable.Add(siteID);
                        s = new Site();
                        s.siteID = siteID;
                        s.oldSiteID = int.Parse(hazalRow[2]);
                        //LABELS
                        s.LabelX = x;
                        s.LabelY = y;

                        //NIG_info
                        s.NIG_Info = GeographicP.GetNIG_info(points);

                        // GS84_Info
                        s.WGS84_Info = Converters.ITMtoWGS84_info(points);
                        s.siteSize = hazalRow[14];
                        db.Sites.Add(s);

                    }
                    // if its not null, check the location.
                    else
                    {
                        if (points[2] != -1)
                        {
                            double[] points1 = null;
                            Polygon polygon = new Polygon(points);
                            // if it was a polygon and now is polygon
                            if (s.NIG_Info.Contains("POLYGON"))
                            {
                                Polygon old = new Polygon(s.NIG_Info);
                                Polygon newP = old.Include(polygon);
                                points1 = newP.GetPoints();
                                Point middle = newP.getMiddlePoint();
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
                            SiteIDTable.InsertToSiteIDTable(siteID, s.NIG_Info, newNigInfo, recordID);
                            //NIG_info
                            s.NIG_Info = newNigInfo;

                            // GS84_Info
                            s.WGS84_Info = Converters.ITMtoWGS84_info(points1);
                        }  

                        // If this coordinates are point or the old are point, the site coordinates are still as before.
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Sites");
                    }

                    Functions.InsertToConnectionTable(recordID, siteID, "Sites");

                    // find if area is already exist in area table
                    string area_str = hazalRow[6];
                    int areaID = GetID.CreateAreaId(area_str);
                    Area a = db.Areas.Find(areaID);
                    if (a is null)
                    {
                        a = new Area();
                        a.areaID = areaID;
                        db.Areas.Add(a);
                    }
                    a.areaHeb = area_str;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Area");
                    }

                    Functions.InsertToConnectionTable(recordID, areaID, "Areas");

                    string bibliography = hazalRow[18];
                    // find if excavationID is already exist in site table
                    int eID = int.Parse(hazalRow[3]);
                    Excavation e = db.Excavations.Find(eID);
                    if (e is null)
                    {
                        e = new Excavation();
                        e.excavationID = eID;
                        db.Excavations.Add(e);
                    }
                    // excavation date
                    if (String.Compare("NULL", hazalRow[8]) == 1)
                    {
                        e.excavationDate = Functions.CreateExcDate(hazalRow[8]);
                    }
                    e.excavationHeb = siteName;
                    e.bibliography = bibliography;
                    e.siteID = siteID;
                    // insert area to Excavation
                    e.areaID = areaID;
                    e.comment = hazalRow[17];
                    e.excavationSize = hazalRow[15];

                    //excavationNIG_info
                    e.excavationNIG_Info = GeographicP.GetNIG_info(points);

                    //excavationGS84_Info
                    e.excavationWGS84_Info = Converters.ITMtoWGS84_info(points);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Excavation");
                    }

                    Functions.InsertToConnectionTable(recordID, eID, "Excavations");



                    int resercherId = GetID.CreateResercherId(hazalRow[7]);
                    Researcher r = db.Researchers.Find(resercherId);
                    if (r is null)
                    {
                        r = new Researcher();
                        r.researcherID = resercherId;
                        r.researcherNameHeb = hazalRow[7];
                        db.Researchers.Add(r);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Researcher");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, resercherId, "Reserchers");

                    // connect researcher_excavations
                    int reID = GetID.CreateConnectionID(resercherId, eID, "researcher_excavations");
                    ResearchersExcavation re = db.ResearchersExcavations.Find(reID, resercherId, eID);
                    if (re is null)
                    {
                        re = new ResearchersExcavation();
                        re.researcherExcavationID = reID;
                        re.excavationID = eID;
                        re.researcherID = resercherId;
                        db.ResearchersExcavations.Add(re);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Researcher Excavation");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, reID, "ResearchersExcavations");

                    //Periods
                    
                    int perID = GetID.CreatePeriodId(hazalRow[9], false);
                    Period p = db.Periods.Find(perID);
                    if (p is null)
                    {
                        p = new Period();
                        p.periodID = perID;
                        p.nameHeb = hazalRow[9].Replace("\t", "");
                        p.nameEng = "NULL";
                        db.Periods.Add(p);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Periods");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, perID, "Periods");



                    // check if the connection between site and period is already exist
                    int sitePerID = GetID.CreateConnectionID(siteID, perID, "sites_periods");
                    SitesPeriod sp = db.SitesPeriods.Find(sitePerID, siteID, perID);
                    if (sp is null)
                    {
                        sp = new SitesPeriod();
                        sp.sitesPeriodsID = sitePerID;
                        sp.siteID = s.siteID;
                        sp.periodID = perID;
                        db.SitesPeriods.Add(sp);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Sites Periods");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, sitePerID, "SitesPeriods");


                    //siteType
                    int stID = GetID.CreateFindingSiteTypeId(hazalRow[12]);
                    SiteType st = db.SiteTypes.Find(stID);
                    if (st is null)
                    {
                        st = new SiteType();
                        st.findingSiteTypeID = stID;
                        st.findingSiteTypeHeb = hazalRow[12];
                        db.SiteTypes.Add(st);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Site Type");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, stID, "FindingSiteType");

                    //SiteNature
                    int s_natureID = GetID.CreateFindingSiteNatureId(hazalRow[13]);
                    SiteNature s_nat = db.SiteNatures.Find(s_natureID);
                    if (s_nat is null)
                    {
                        // create new value
                        s_nat = new SiteNature();
                        s_nat.findingSiteNatureID = s_natureID;
                        s_nat.findingSiteNatureHeb = hazalRow[13];
                        db.SiteNatures.Add(s_nat);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Site Nature");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, s_natureID, "FindingSiteNature");

                    //FindingType
                    int f_typeID = GetID.CreateFindingTypeId(hazalRow[16]);
                    FindingType ft = db.FindingTypes.Find(f_typeID);
                    if (ft is null)
                    {
                        ft = new FindingType();
                        ft.findingTypeID = f_typeID;
                        ft.findingTypeDescriptionHeb = hazalRow[16];
                        db.FindingTypes.Add(ft);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Finding Type");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, f_typeID, "FindingType");

                    // Option one - Finding
                    int fID = GetID.CreateFindingId(stID, s_natureID, f_typeID, eID, hazalRow[11]);
                    Finding f = db.Findings.Find(fID);
                    if (f is null)
                    {
                        f = new Finding();
                        f.findingID = fID;
                        f.findingTypeID = f_typeID;
                        f.siteTypeID = stID;
                        f.siteNatureID = s_natureID;
                        if (hazalRow[11] != "NULL")
                        {
                            f.findingDate = hazalRow[11];
                        }
                        f.excavationID = eID;
                        // comment???
                        f.descriptionHeb = hazalRow[17];
                        db.Findings.Add(f);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Finding");
                        }
                    }


                    // Finding periods
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
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Findings Periods");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, fID, "FindingsPeriods");



                    //Periodical
                    Periodical per = db.Periodicals.Find(3);
                    if (per is null)
                    {
                        per = new Periodical();
                        per.periodicalID = 3;
                        per.periodicalNameHeb = "חדשות ארכיאולוגיות מהדורה מודפסת";
                        per.periodicalNameEng = "Excavations and Surveys in Israel, Printed version";
                        db.Periodicals.Add(per);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Periodicals - ha");
                        }

                    }

                    //PublicationType
                    PublicationType pt = db.PublicationTypes.Find(-1);
                    if (pt is null)
                    {
                        pt = new PublicationType();
                        pt.publicationTypeID = -1;
                        pt.publicationTypeHeb = "לא ידוע";
                        pt.publicationTypeEng = "Unknown";
                        db.PublicationTypes.Add(pt);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "PublicationType");
                        }
                    }

                    // Publications
                    //split the bibliography to ha and pages
                    string[] bArr = Functions.Bibliography(bibliography, recordID);
                    string haNum = bArr[0];
                    string pages = bArr[1];
                    string heb = bArr[2];
                    string ha_str = bArr[3];
                    string pages_str = bArr[4];

                    string bib = ha_str + " " + haNum + "," + " " + pages_str + " " + pages;
                    int pubID = GetID.CreatePublicationId(bib, hazalRow[4], "");
                    Publication pub = db.Publications.Find(pubID);
                    if (pub is null)
                    {
                        pub = new Publication();
                        pub.publicationID = pubID;
                        db.Publications.Add(pub);

                    }
                    pub.year = Functions.FindYear(years, heb, haNum);
                    pub.periodicalID = 3;
                    pub.reference = bib;
                    pub.articleTitle = hazalRow[4];
                    pub.publicationTypeID = -1;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Publications - ha");
                    }
                    Functions.InsertToConnectionTable(recordID, pubID, "Publications");


                    //Authors
                    int authID = GetID.CreateAuthorId(hazalRow[7]);
                    Author au = db.Authors.Find(authID);
                    if (au is null)
                    {
                        au = new Author();
                        au.authorID = authID;
                        au.nameHeb = hazalRow[7];
                        db.Authors.Add(au);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "Authors");
                        }
                    }
                    Functions.InsertToConnectionTable(recordID, authID, "Authors");


                    //AuthorsPublications
                    int apID = GetID.CreateConnectionID(authID, pubID, "authors_publication");
                    AuthorsPublication ap = db.AuthorsPublications.Find(apID, authID, pubID);
                    if (ap is null)
                    {
                        ap = new AuthorsPublication();
                        ap.authorPublicationID = apID;
                        ap.authorID = authID;
                        ap.publicationID = pubID;
                        db.AuthorsPublications.Add(ap);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "AuthorsPublications");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, apID, "AuthorsPublications");



                    //PublicationsExcavations
                    int pubExId = GetID.CreateConnectionID(eID, pubID, "excavations_publications");
                    ExcavationsPublication ep = db.ExcavationsPublications.Find(pubExId, eID, pubID);
                    if (ep is null)
                    {
                        ep = new ExcavationsPublication();
                        ep.excavationPublicationID = pubExId;
                        ep.excavationID = eID;
                        ep.publicationID = pubID;
                        db.ExcavationsPublications.Add(ep);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", i + 1, "ExcavationsPublications");
                        }
                    }

                    Functions.InsertToConnectionTable(recordID, pubExId, "ExcavationsPublications");
                }

                //////////////------------------------------------------------/////////////////////////////
                // CreateSitesNamesTable
                Dictionary<int, string> sitesNames = new Dictionary<int, string>();
                Dictionary<string, int> excavations_names = new Dictionary<string, int>();
                foreach (int sID in sitesIDinThisTable)
                {
                    excavations_names.Clear();
                    // for each excavation,
                    foreach (Excavation e in db.Excavations)
                    {
                        if (e.siteID == sID)
                        {
                             // if this name is already exist in the dictionary, just add 1 for count.
                             if (excavations_names.ContainsKey(e.excavationHeb))
                             {
                                excavations_names[e.excavationHeb]++;
                             } else
                             {
                                excavations_names.Add(e.excavationHeb, 1);
                             }
                        }
                    }

                    sitesNames.Add(sID, excavations_names.Keys.Max());
                }


                foreach (KeyValuePair<int, string> sName in sitesNames)
                {
                    int siteID = sName.Key;
                    string name = sName.Value;
                    int sitenameID = GetID.CreateSiteNameId(name);
                    SitesName sn = db.SitesNames.Find(sitenameID);
                    // if this sitename does not exist, create new one.
                    if (sn is null)
                    {
                        sn = new SitesName();
                        sn.siteNameID = sitenameID;
                        sn.languageID = 1;
                        sn.txtName = name;
                        db.SitesNames.Add(sn);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", -1 , "SitesName" + name);
                        }
                    }

                    // now connect between sitename and site
                    // check if the connection between sitename and site is already exist
                    int ssnID = GetID.CreateConnectionID(siteID, sitenameID, "sites_site_name");
                    SitesSiteName ssn = db.SitesSiteNames.Find(ssnID, siteID, sitenameID);
                    if (ssn is null)
                    {
                        ssn = new SitesSiteName();
                        ssn.sitesSiteNamesID = ssnID;
                        ssn.siteID = siteID;
                        ssn.siteNameID = sitenameID;
                        db.SitesSiteNames.Add(ssn);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Functions.ErrorLogging(ex, "HazalaRecords", -1 , "Site site-names siteID:" + siteID + " SiteNameID:" + sitenameID);
                        }
                    }

                }
            }
        }




        private static void Periods(List<List<string>> rows)
        {
            // check if not null
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            using (ProjectDB db = new ProjectDB())
            {
                int num = rows.Count();
                for (int i = 0; i < num; i++)
                {
                    List<string> perRow = rows[i];
                    int pID = int.Parse(perRow[0]);
                    Period p = db.Periods.Find(pID);
                    // If its a new value
                    if (p is null)
                    {
                        p = new Period();
                        p.periodID = pID;
                        // Add it to db
                        db.Periods.Add(p);
                    }
                    p.nameEng = perRow[3].Replace("\t", "");
                    int cmp1 = String.Compare("NULL", perRow[5]);
                    int cmp2 = String.Compare("NULL", perRow[6]);
                    if (cmp1 != 0) p.fromYear = Functions.ConvertStrToInt(perRow[5]);
                    if (cmp2 != 0) p.upToYear = Functions.ConvertStrToInt(perRow[6]);
                    p.periodType = perRow[7];
                    p.nameHeb = perRow[8].Replace("\t", "");
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "Periods", pID, "Periods");
                    }
                }


            }

        }

        public static void CreateLanguage()
        {
            using (ProjectDB db = new ProjectDB())
            {
                Language Heb = db.Languages.Find(1);
                if (Heb is null)
                {
                    Heb = new Language();
                    Heb.languageID = 1;
                    Heb.languageHeb = "עברית";
                    Heb.languageEng = "Hebrew";
                    db.Languages.Add(Heb);
                    db.SaveChanges();
                }
                Language Eng = db.Languages.Find(2);
                if (Eng is null)
                {
                    Eng = new Language();
                    Eng.languageID = 2;
                    Eng.languageHeb = "אנגלית";
                    Eng.languageEng = "English";
                    db.Languages.Add(Eng);
                    db.SaveChanges();
                }
            }
        }

    }
}
