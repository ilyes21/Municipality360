using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Municipality360.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
