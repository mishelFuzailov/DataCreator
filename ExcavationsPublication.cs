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
    
    public partial class ExcavationsPublication
    {
        public int excavationPublicationID { get; set; }
        public int excavationID { get; set; }
        public int publicationID { get; set; }
    
        public virtual Excavation Excavation { get; set; }
        public virtual Publication Publication { get; set; }
    }
}