[Table("Users")]
public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty; // Ensure default

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty; // Ensure default

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty; // Ensure default

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Remove [Required]

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true; // Remove [Required]

    [MaxLength(50)]
    public string Role { get; set; } = "User"; // Remove [Required]

    public int FailedLoginAttempts { get; set; } = 0; // Remove [Required]

    public DateTime? LockoutEnd { get; set; }

    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } =
        new List<RefreshTokenEntity>();
}
