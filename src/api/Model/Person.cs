using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model;
[Table("Person")]
public class Person
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id {get; set;} = 0;
    
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 50 characters")]
    public string SurName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Range(minimum:0, maximum:120, ErrorMessage = "Age must be between 0 and 120")]
    public int Age { get; set; }

    public bool TryValidate<T>(T model, out List<ValidationResult> validationResults)
    {
        var validationContext = new ValidationContext(model!);
        validationResults = new List<ValidationResult>();
        return Validator.TryValidateObject(model!,
                                           validationContext,
                                           validationResults,
                                           true);
    }
}
