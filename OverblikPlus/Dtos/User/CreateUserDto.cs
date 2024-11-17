using System.ComponentModel.DataAnnotations;

namespace OverblikPlus.Dtos.User;

public class CreateUserDto
{
    [Required(ErrorMessage = "Fornavn er påkrævet.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Efternavn er påkrævet.")]
    public string LastName { get; set; }

    public string MedicationDetails { get; set; }

    [Required(ErrorMessage = "Email er påkrævet.")]
    [EmailAddress(ErrorMessage = "Ugyldig email.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Adgangskode er påkrævet.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Rolle er påkrævet.")]
    public string Role { get; set; }
}