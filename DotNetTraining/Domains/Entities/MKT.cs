using System;

namespace BPMaster.Domains.Entities
{
    public class MKT
    {
        public int Id { get; set; }
        public bool Tag_Bool { get; set; }
        public int Tag_Integer { get; set; }
        public decimal Tag_Real { get; set; }
        
        // Common audit fields following project pattern
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
