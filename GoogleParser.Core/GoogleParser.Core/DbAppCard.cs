//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoogleParser.Core
{
    using System; using Svyaznoy.Core.Model;
    using System.Collections.Generic;
    
    public partial class DbAppCard : ISupportHistoryEntity
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string DeveloperName { get; set; }
        public string DeveloperEmail { get; set; }
        public System.DateTime CreatedUtcDate { get; set; }
        public Nullable<System.DateTime> UpdatedUtcDate { get; set; }
        public bool IsDeleted { get; set; }
        public string DeveloperPhysicalAddress { get; set; }
    }
}
