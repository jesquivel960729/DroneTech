using System.ComponentModel.DataAnnotations;

namespace DroneTech.Models
{
    public class Medicine
    {
        [Required]
        [RegularExpression("^[0-9a-zA-Z-_]*$",ErrorMessage = "EL nombre solo debe contener letras, números y '-' o '_'")]
        public string Name { get; set; }

        [Required]
        public float Weigth { get; set; }

        [Required]
        [RegularExpression("^[0-9A-Z_]*$",ErrorMessage = "EL código solo debe contener letras mayúsculas, números y '_'")]
        public string Code { get; set; }
        public string? Image { get; set; }
    }
}