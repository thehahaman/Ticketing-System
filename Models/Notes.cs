
namespace Blockbuster_Rental_Software.Models
{
    public class Notes
    {
        public int pID { get; set; }
        public string text { get; set; }
        public DateTime date {get; set;}

        public int userID { get; set; }
        public string userName { get; set; }

    }
}
