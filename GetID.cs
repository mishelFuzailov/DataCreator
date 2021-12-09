using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    class GetID
    {
        //check if the sitename is already exist. if not return new value.
        public static int CreateSiteNameId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (SitesName sitesName in db.SitesNames)
                {
                    int comp = String.Compare(sitesName.txtName, str, new CultureInfo("he"), CompareOptions.None);

                    // if its site name is already exist return it's id
                    if (comp == 0)
                    {
                        return sitesName.siteNameID;
                    }
                    if (maxId < sitesName.siteNameID)
                    {
                        maxId = sitesName.siteNameID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }



        // excavation id by: excaNameInHeb, in eng and siteID, biblio

        public static int CreateExcavationID(string excaNameInHeb, string excaNameInEng, int siteID, string bib) 
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Excavation e in db.Excavations)
                {
                    int comp1 = String.Compare(e.excavationHeb, excaNameInHeb, new CultureInfo("he"), CompareOptions.None);
                    int comp2 = String.Compare(e.excavationEng, excaNameInEng, new CultureInfo("he"), CompareOptions.None);
                    bool comp3 = (siteID == e.siteID);
                    int comp4 = String.Compare(e.bibliography, bib, new CultureInfo("he"), CompareOptions.None);
                    // if its Resercher is already exist return it's id
                    if (comp1 == 0 || comp2 == 0)
                    {
                        if (comp3 && comp4 == 0)
                            return e.excavationID;
                    }
                    if (maxId < e.excavationID)
                    {
                        maxId = e.excavationID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }



        //check if the Resercheris already exist. if not return new value.
        public static int CreateResercherId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Researcher r in db.Researchers)
                {
                    int comp = String.Compare(r.researcherNameHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its Resercher is already exist return it's id
                    if (comp == 0)
                    {
                        return r.researcherID;
                    }
                    if (maxId < r.researcherID)
                    {
                        maxId = r.researcherID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }


        //check if the Author already exist. if not return new value.
        public static int CreateAuthorId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Author a in db.Authors)
                {
                    int comp = String.Compare(a.nameHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its Resercher is already exist return it's id
                    if (comp == 0)
                    {
                        return a.authorID;
                    }
                    if (maxId < a.authorID)
                    {
                        maxId = a.authorID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }

        //check if the PERIODICAL already exist. if not return new value.
        public static int CreatePeriodicalId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Periodical p in db.Periodicals)
                {
                    int comp = String.Compare(p.periodicalNameHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its Resercher is already exist return it's id
                    if (comp == 0)
                    {
                        return p.periodicalID;
                    }
                    if (maxId < p.periodicalID)
                    {
                        maxId = p.periodicalID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }




        //check if the SiteType already exist. if not return new value.
        public static int CreateFindingSiteTypeId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (SiteType st in db.SiteTypes)
                {
                    int comp = String.Compare(st.findingSiteTypeHeb, str, new CultureInfo("he"), CompareOptions.None);
                         // if its FindingSiteType is already exist return it's id
                    if (comp == 0)
                    { 
                        return st.findingSiteTypeID;
                    }
                    if (maxId < st.findingSiteTypeID)
                    {
                        maxId = st.findingSiteTypeID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }



        //check if the Publication already exist. if not return new value.
        public static int CreatePublicationId(string str, string siteName, string intro)
        {
            using (ProjectDB db = new ProjectDB())
            {
                //check if intro exist
                bool intro_b = false;
                if (intro != "") intro_b = true;
                int comp2 = 2;
                int maxId = 0;
                foreach (Publication p in db.Publications)
                {
                    int comp = String.Compare(p.bibliography, str, new CultureInfo("he"), CompareOptions.None);
                    int comp1 = String.Compare(p.articleTitle, siteName, new CultureInfo("he"), CompareOptions.None);
                    if (intro_b)
                    {
                        comp2 = String.Compare(p.abstractHeb, intro, new CultureInfo("he"), CompareOptions.None);
                    }
                    // if this publication is already exist return it's id
                    if (comp == 0 && comp1 == 0)
                    {
                        if (!intro_b)
                            return p.publicationID;
                        else
                        {
                            // if the intro indotnation is not exist or its the same, return this publication id.
                            if (comp2 == 0 || p.abstractHeb is null) return p.publicationID;
                        }
                    }
                    if (maxId < p.publicationID)
                    {
                        maxId = p.publicationID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }


        public static int CreatePublicationIdPark(string publicationName, string title, string year)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Publication p in db.Publications)
                {
                    int comp = String.Compare(p.abstractEng, publicationName, new CultureInfo("he"), CompareOptions.None);
                    int comp1 = String.Compare(p.abstractHeb, publicationName, new CultureInfo("he"), CompareOptions.None);
                    if (comp == 0|| comp1 == 0)
                    {
                        int comp2 = String.Compare(p.articleTitle, title, new CultureInfo("he"), CompareOptions.None);
                        int comp3 = String.Compare(p.year, year, new CultureInfo("he"), CompareOptions.None);
                        if (comp2 == 0 && comp3 == 0)
                        {
                            return p.publicationID;
                        }
                    }
                    
                    if (maxId < p.publicationID)
                    {
                        maxId = p.publicationID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }

        public static int CreatePublicationIdNew()
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Publication p in db.Publications)
                {

                    if (maxId < p.publicationID)
                    {
                        maxId = p.publicationID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }


        //check if the SiteType already exist. if not return new value.
        public static int CreateFindingTypeId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (FindingType ft in db.FindingTypes)
                {
                    int comp = String.Compare(ft.findingTypeDescriptionHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its FindingTyp is already exist return it's id
                    if (comp == 0)
                    {
                        return ft.findingTypeID;
                    }
                    if (maxId < ft.findingTypeID)
                    {
                        maxId = ft.findingTypeID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }





        //check if the SiteNature already exist. if not return new value.
        public static int CreateFindingSiteNatureId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (SiteNature sn in db.SiteNatures)
                {
                    int comp = String.Compare(sn.findingSiteNatureHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its FindingSiteNature is already exist return it's id
                    if (comp == 0)
                    {
                        return sn.findingSiteNatureID;
                    }
                    if (maxId < sn.findingSiteNatureID)
                    {
                        maxId = sn.findingSiteNatureID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }




        //option 1 - check if the SiteNature already exist. if not return new value.
        public static int CreateFindingId(int siteTypeID, int siteNatureID, int findingTypeID, int excavationID ,string date)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Finding f in db.Findings)
                {
                    // if its site name is already exist return it's id
                    if (f.siteTypeID == siteTypeID &&
                        f.siteNatureID == siteNatureID &&
                        f.findingTypeID == findingTypeID&&
                        f.excavationID == excavationID)
                    {
                        // if the date in finding is null, convert it to this date and return it.
                        if (f.findingDate is null)
                        {
                            if (date != "NULL")
                            {
                                f.findingDate = date;
                            }
                            return f.findingID;
                        }
                        // if this date is not null and the date in f is not null, compare them.
                        if (date != "Null")
                        {
                            int comp = String.Compare(f.findingDate, date, new CultureInfo("he"), CompareOptions.None);
                            // if the dates are not the same, create new id.
                            if (comp == 1)
                            {
                                if (maxId < f.findingID)
                                {
                                    maxId = f.findingID;
                                }
                                return maxId + 1;
                            }
                        }

                        return f.findingID;
                    }
                    if (maxId < f.findingID)
                    {
                        maxId = f.findingID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }




        //check if the Area name is already exist. if not return new value.
        public static int CreateAreaId(string str)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Area a in db.Areas)
                {
                 
                    int comp = String.Compare(a.areaHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its area is already exist return it's id
                    if (comp == 0)
                    {
                        return a.areaID;
                    }

                    if (maxId < a.areaID)
                    {
                        maxId = a.areaID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }


        public static string deleteSpaces (string period)
        {
            var perStr = period.Split();
            bool flag1 = true;
            bool flag2 = true;
            string retVal = "";
            for (int i = 0; i < perStr.Length; i ++)
            {
                var word = perStr[i];
                flag2 = true;
                if (String.Compare(word, "") == 0) flag2 = false;
                if (flag2) { 
                    if (!flag1) retVal += " ";
                    retVal += perStr[i];
                    if (flag1) flag1 = false;
                }
            }
            return retVal;
        }
        //check if the period name is already exist. if not return new value.
        public static int CreatePeriodId(string str, bool flag)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                string newStr = deleteSpaces(str);
                newStr = newStr.Replace("\t", "");
                foreach (Period p in db.Periods)
                {
                    int comp;
                    if (flag) comp = String.Compare(p.nameEng, newStr);
                    else comp = String.Compare(p.nameHeb, newStr, new CultureInfo("he"), CompareOptions.None);
                    // if its period is already exist return it's id
                    if (comp == 0)
                    {
                        return p.periodID;
                    }

                    if (maxId < p.periodID)
                    {
                        maxId = p.periodID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }






        //check if the publicationType is already exist. if not return new value.
        public static int CreatePublicationTypeId(string str)
        {
            if (str == "") return -1;
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (PublicationType p in db.PublicationTypes)
                {
                    int comp = String.Compare(p.publicationTypeHeb, str, new CultureInfo("he"), CompareOptions.None);
                    // if its publicationType is already exist return it's id
                    if (comp == 0)
                    {
                        return p.publicationTypeID;
                    }

                    if (maxId < p.publicationTypeID)
                    {
                        maxId = p.publicationTypeID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }





        //check if the sitename is already exist. if not return new value.
        public static int CreateConnectionID(int table1ID, int table2ID, string connectionTableName)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;

                // create connection to sites_site_name table
                if (connectionTableName == "sites_site_name")
                {
                    foreach (SitesSiteName ssName in db.SitesSiteNames)
                    {
                        // if its site name is already exist return it's id
                        if (ssName.siteID == table1ID && ssName.siteNameID == table2ID)
                        {
                            return ssName.sitesSiteNamesID;
                        }
                        if (maxId < ssName.sitesSiteNamesID)
                        {
                            maxId = ssName.sitesSiteNamesID;
                        }
                    }
                }


                if (connectionTableName == "sites_periods")
                {
                    foreach (SitesPeriod sp in db.SitesPeriods)
                    {
                        // if its site name is already exist return it's id
                        if (sp.siteID == table1ID && sp.periodID == table2ID)
                        {
                            return sp.sitesPeriodsID;
                        }
                        if (maxId < sp.sitesPeriodsID)
                        {
                            maxId = sp.sitesPeriodsID;
                        }
                    }
                }


                //FindingsPeriods
                if (connectionTableName == "findings_periods")
                {
                    foreach (FindingsPeriod fp in db.FindingsPeriods)
                    {
                        // if its site name is already exist return it's id
                        if (fp.findingID == table1ID && fp.periodID == table2ID)
                        {
                            return fp.findingPeriodID;
                        }
                        if (maxId < fp.findingPeriodID)
                        {
                            maxId = fp.findingPeriodID;
                        }
                    }
                }



                if (connectionTableName == "researcher_excavations")
                {
                    foreach (ResearchersExcavation se in db.ResearchersExcavations)
                    {
                        // if its site name is already exist return it's id
                        if (se.researcherID == table1ID && se.excavationID == table2ID)
                        {
                            return se.researcherExcavationID;
                        }
                        if (maxId < se.researcherExcavationID)
                        {
                            maxId = se.researcherExcavationID;
                        }
                    }
                }


                if (connectionTableName == "excavations_publications")
                {
                    foreach (ExcavationsPublication se in db.ExcavationsPublications)
                    {
                        // if its site name is already exist return it's id
                        if (se.excavationID == table1ID && se.publicationID == table2ID)
                        {
                            return se.excavationPublicationID;
                        }
                        if (maxId < se.excavationPublicationID)
                        {
                            maxId = se.excavationPublicationID;
                        }
                    }
                }


                if (connectionTableName == "researcher_excavations")
                {
                    foreach (ResearchersExcavation se in db.ResearchersExcavations)
                    {
                        // if its site name is already exist return it's id
                        if (se.researcherID == table1ID && se.excavationID == table2ID)
                        {
                            return se.researcherExcavationID;
                        }
                        if (maxId < se.researcherExcavationID)
                        {
                            maxId = se.researcherExcavationID;
                        }
                    }
                }



                if (connectionTableName == "authors_publication")
                {
                    foreach (AuthorsPublication ap in db.AuthorsPublications)
                    {
                        // if its site name is already exist return it's id
                        if (ap.authorID == table1ID && ap.publicationID == table2ID)
                        {
                            return ap.authorPublicationID;
                        }
                        if (maxId < ap.authorPublicationID)
                        {
                            maxId = ap.authorPublicationID;
                        }
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }

        }



        //check if the Area name is already exist. if not return new value.
        public static int RecordSourchID(int ID, string tableName)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (RecordSource rs in db.RecordSources)
                {
                    // if its site name is already exist return it's id
                    if (rs.recordSourceTable == tableName &&
                        rs.sourceID == ID)
                    {
                        return rs.recordID;
                    }
                    if (maxId < rs.recordID)
                    {
                        maxId = rs.recordID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }




        //check if the Area name is already exist. if not return new value.
        public static int ConnectionTableID(int ID, int tableID, int rsID)
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (SourceDBConnection sdbc in db.SourceDBConnections)
                {
                    // if it is already exist return it's id
                    if (sdbc.tableID == tableID &&
                        sdbc.dbID == ID &&
                        sdbc.sourceID == rsID)
                    {
                        return sdbc.ID;
                    }
                    if (maxId < sdbc.ID)
                    {
                        maxId = sdbc.ID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }


        // TableID
        public static int TableID(string tableName)
        {
            using (ProjectDB db = new ProjectDB())
            {
                foreach (TableCode tc in db.TableCodes)
                {
                    // if table name is already exist return it's id
                    if (tc.tableName == tableName)
                    {
                        return tc.tableID;
                    }
                }
                // if not, return new id.
                return -1;
            }
        }



        // one site name can include several siteID's.
        private static List<int> FindSiteIDBySiteName(int siteNameID)
        {
            // init list of id's.
            List<int> sitesID = new List<int>();
            using (ProjectDB db = new ProjectDB())
            {
                foreach (SitesSiteName ssn in db.SitesSiteNames)
                {
                    if (ssn.siteNameID == siteNameID)
                    {
                        sitesID.Add(ssn.siteID);
                    }
                }
                // if it new site name return null.
                if (sitesID.Count == 0) return null;
                // if not, return new id.
                return sitesID;
            }
        }




        // get location and name.
        // return old site id if exist and if not return new one.
        public static int GetSiteID (double[] points)
        {
            Polygon polygon = null;
            Point point = null;
            if (points[2] == -1)
            {
                polygon = new Polygon(points);
            } else
            {
                point = new Point(points[0], points[1]);
            } 
            using (ProjectDB db = new ProjectDB())
            {
                // if its a polygon
                if (polygon != null)
                {
                    // for each site, check if site location is in range.
                    foreach (Site s in db.Sites)
                    {
                        //if in range return this id.
                        if (s.NIG_Info.Contains("POLYGON"))
                        {
                            if (polygon.DistanceFromPolygonInRange(new Polygon(s.NIG_Info)))
                            {
                                return s.siteID;
                            }
                        }
                        else
                        {
                            if (polygon.PointIsIn(new Point(s.NIG_Info)))
                            {
                                return s.siteID;
                            }
                        }
                    }
                } else
                {
                    foreach (Site s in db.Sites)
                    {
                        if (s.NIG_Info.Contains("POINT"))
                        {
                            //if in range return this id.
                            if (point.InRange(new Point(s.NIG_Info)))
                            {
                                return s.siteID;
                            }
                        }
                        // if its a polygon
                        else
                        {
                            if (new Polygon(s.NIG_Info).PointInRange(point)) return s.siteID;
                        }
                    }
                }
                // create new site id
                return CreateNewSiteID();

            }
        }



        private static int CreateNewSiteID()
        {
            using (ProjectDB db = new ProjectDB())
            {
                int maxId = 0;
                foreach (Site rs in db.Sites)
                {
                    if (maxId < rs.siteID)
                    {
                        maxId = rs.siteID;
                    }
                }
                // if not, return new id.
                return maxId + 1;
            }
        }

    }

}
