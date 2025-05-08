namespace BPMaster.Domains.Dtos
{
    public class TagWriteRequest
    {
        public string TagName { get; set; }
        public object Value { get; set; }
    }

    public class MultipleTagWriteRequest
    {
        public Dictionary<string, object> Tags { get; set; }
    }
}
