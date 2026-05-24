using System.ComponentModel.DataAnnotations;

namespace ITManager.Web.Models
{
    public class EditSampleModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description must be 300 characters or fewer.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
