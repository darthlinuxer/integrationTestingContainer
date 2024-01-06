using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model;
// modify this Person class so it can be scaffolded by aspnet-codegenerator
[Table("Person")]
[Serializable]
public class Person
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id {get; set;} = 0;
    public string? Name { get; set; }
    public int Age { get; set; } = 0;
}
