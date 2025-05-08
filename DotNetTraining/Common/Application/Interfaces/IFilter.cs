namespace BPMaster.Common.Application.Interfaces
{
    public class IFilter
    {
        public required string Value { get; set; }
        public required string Statement { get; set; }
        public required string Keyword { get; set; }

    }
}
