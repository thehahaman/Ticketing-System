using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blockbuster_Rental_Software.Models
{
    public class Product
    {
        [Required]
        [DisplayName("Product ID")]
        [Range(1, 2147483647)]
        public int Id { get; set; }

        [Required]
        [StringLength(45, MinimumLength = 4)]
        [DisplayName("Ticket Title")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Decsription")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [DisplayName("Opened By")]
        [Range(1, 2147483647)]
        public int openedBy { get; set; }

        public DateTime Date { get; set; }

        public List<Notes> notes = new List<Notes>();
        public int assignedDeveloper { get; set; }

    }
}
