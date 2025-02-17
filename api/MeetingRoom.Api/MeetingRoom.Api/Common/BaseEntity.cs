namespace MeetingRoom.Api.Common
{
    public abstract class BaseEntity : IAuditableEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? TenantId { get; set; }

        public virtual void SetCreated(string createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
            SetModified(createdBy);
        }

        public virtual void SetModified(string modifiedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = modifiedBy;
        }

        public virtual void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            SetModified(deletedBy);
        }
    }
}
