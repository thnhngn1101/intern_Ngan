using System;

namespace BPMaster.Domains.Dtos
{
    public class MKTDto
    {
        public int Id { get; set; }
        public bool Tag_Bool { get; set; }
        public int Tag_Integer { get; set; }
        public decimal Tag_Real { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CreateMKTRequest
    {
        public bool Tag_Bool { get; set; }
        public int Tag_Integer { get; set; }
        public decimal Tag_Real { get; set; }
    }

    public class UpdateMKTRequest
    {
        public bool Tag_Bool { get; set; }
        public int Tag_Integer { get; set; }
        public decimal Tag_Real { get; set; }
    }
}
