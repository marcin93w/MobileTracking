//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public int Id { get; set; }
        public Nullable<int> RouteId { get; set; }
        public Nullable<System.DateTime> Datetime { get; set; }
        public System.Data.Spatial.DbGeography Point { get; set; }
    
        public virtual Route Route { get; set; }
    }
}