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
    
    public partial class RecordSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RecordSource()
        {
            this.SourceDBConnections = new HashSet<SourceDBConnection>();
        }
    
        public int recordID { get; set; }
        public int sourceID { get; set; }
        public string recordSourceTable { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public string datails { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SourceDBConnection> SourceDBConnections { get; set; }
    }
}
