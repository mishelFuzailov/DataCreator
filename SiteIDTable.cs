using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    class SiteIDTable
    {
        // insert to SiteID table
        public static void InsertToSiteIDTable(int siteID, string oldNIG_Info, string newNIG_Info, int recordID)
        {
            // if the nig_info isnt changed just return.
            if (String.Equals(oldNIG_Info, newNIG_Info)) return;
            using (ProjectDB db = new ProjectDB())
            {
                foreach (var sID in db.SitesIDs)
                {
                    if (sID.recordID == recordID)
                    {
                        return;
                    }
                }
                SitesID sites = new SitesID();
                sites.siteID = siteID;
                sites.oldNIG_Info = oldNIG_Info;
                sites.newNIG_Info = newNIG_Info;
                sites.recordID = recordID;
                db.SitesIDs.Add(sites);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Functions.ErrorLogging(ex, "SiteIDTable", 0, "SiteIDTable");
                }
            }
        }
    }
}
