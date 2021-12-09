using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    class ArcheologicalPark
    {
        private static int RecordSourceTable(Dictionary<string, string> row)
        {
            using (ProjectDB db = new ProjectDB())
            {

                // insert row into
                int rowID = Functions.SerialNumber(row["Serial Number"]);
                int recordID = GetID.RecordSourchID(rowID, "ArchPark");
                RecordSource rs = db.RecordSources.Find(recordID);
                var date = Functions.CreateExcDate(row["Time Downloaded"]);
                if (rs is null)
                {
                    rs = new RecordSource();
                    rs.date = date;
                    rs.recordID = recordID;
                    rs.sourceID = rowID;
                    rs.recordSourceTable = "ArchPark";
                    db.RecordSources.Add(rs);
                }
                rs.updateDate = date;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "RecordSourchTable", rowID, "ArchNewsEng");
                }

                return recordID;
            }
        }


        public static void Insert(List<Dictionary<string, string>> DATA)
        {
            int rowsNum = DATA.Count();

            for (int i = 0; i < rowsNum; i++)
            {
                Dictionary<string, string> row = DATA[i];

                int rowID = RecordSourceTable(row);
                using (ProjectDB db = new ProjectDB())
                {

                    //PublicationType
                    int ptID = GetID.CreatePublicationTypeId(row["Type"]);
                    if (ptID != -1)
                    {
                        PublicationType pt = db.PublicationTypes.Find(ptID);
                        if (pt is null)
                        {
                            pt = new PublicationType();
                            pt.publicationTypeID = ptID;
                            pt.publicationTypeEng = row["Type"];
                            db.PublicationTypes.Add(pt);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Functions.ErrorLogging(ex, "Park", i, "PublicationType");
                            }
                        }
                    }
                    Functions.InsertToConnectionTable(rowID, ptID, "PublicationType");


                    //Publications
                    int pubID = GetID.CreatePublicationIdNew();
                    Publication pub = db.Publications.Find(pubID);
                    if (pub is null)
                    {
                        pub = new Publication();
                        pub.publicationID = pubID;
                        pub.periodicalID = 1;
                        pub.articleTitle = row["Article's Name"];
                        pub.reference = row["Pages"];
                        pub.year = row["Year"];
                        pub.comment = row["Location"];
                        db.Publications.Add(pub);

                    }
                    pub.URL = row["url"];
                    if (ptID > 0) pub.publicationTypeID = ptID;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "ArchPark", i, "Publication (cols- Intro, URL, year, pubType, periodicalsID)");
                    }

                    Functions.InsertToConnectionTable(rowID, pubID, "Publications");


                    //Author
                    string authorName = row["Publisher"];
                    int authorID = GetID.CreateAuthorId(authorName);
                    Author a = db.Authors.Find(authorID);
                    if (a is null)
                    {
                        a = new Author();
                        a.authorID = authorID;
                        a.nameEng = authorName;
                        db.Authors.Add(a);
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Functions.ErrorLogging(ex, "PARK", i, "Publisher-Author");
                    }

                    Functions.InsertToConnectionTable(rowID, authorID, "Authors");



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
                            Functions.ErrorLogging(ex, "PARK", i, "Author-Publication");
                        }
                    }

                    Functions.InsertToConnectionTable(rowID, auPubID, "AuthorsPublications");


                }
            }
        }
    }

}
