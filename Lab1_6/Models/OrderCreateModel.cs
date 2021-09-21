using System.ComponentModel.DataAnnotations;

namespace Lab1_6.Controllers.Models
{
    public class OrderCreateModel
    {
        [Required]
        public string RequestId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Sum { get; set; }
    }
}