using System.ComponentModel.DataAnnotations;

namespace GestionPrestataires.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    [Required]

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}
