namespace ITManager.Domain.Entities
{
    public class SampleEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public SampleEntity(string name, string description, string category, bool isActive = true)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Category = category;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, string category, bool isActive)
        {
            Name = name;
            Description = description;
            Category = category;
            IsActive = isActive;
        }
    }
}
