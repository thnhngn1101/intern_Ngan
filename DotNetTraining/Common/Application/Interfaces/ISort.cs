
namespace BPMaster.Common.Application.Interfaces
{
    public class SortOrder
    {
        public static readonly string ASC = "ASC";
        public static readonly string DESC = "DESC";
    }
    public class ISort
    {
        public required string Field { get; set; }
        public required string Order { get; set; }


    }
}
