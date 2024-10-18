using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp.Models
{
    //Chiedere: dovrebbe essere public/private o intern?
    public class CurrentLocationEntry
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string CompleteAddress { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
