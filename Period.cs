//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_Final
{
    using System;
    using System.Collections.Generic;
    
    public partial class Period
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Period()
        {
            this.FindingsPeriods = new HashSet<FindingsPeriod>();
            this.SitesPeriods = new HashSet<SitesPeriod>();
        }
    
        public int periodID { get; set; }
        public string nameHeb { get; set; }
        public string nameEng { get; set; }
        public Nullable<int> fromYear { get; set; }
        public Nullable<int> upToYear { get; set; }
        public string periodType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FindingsPeriod> FindingsPeriods { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SitesPeriod> SitesPeriods { get; set; }
    }
}
