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
    
    public partial class SiteType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SiteType()
        {
            this.Findings = new HashSet<Finding>();
        }
    
        public int findingSiteTypeID { get; set; }
        public string findingSiteTypeEng { get; set; }
        public string findingSiteTypeHeb { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Finding> Findings { get; set; }
    }
}
