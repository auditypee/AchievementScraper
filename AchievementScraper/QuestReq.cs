//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AchievementScraper
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuestReq
    {
        public int QuestReq1 { get; set; }
        public string Quest { get; set; }
        public int AchievementId { get; set; }
    
        public virtual Achievement Achievement { get; set; }
    }
}
