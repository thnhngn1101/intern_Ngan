

namespace Common.Domains.Entities
{
    public abstract class SystemLogEntity<T> : BaseEntity<T>
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
