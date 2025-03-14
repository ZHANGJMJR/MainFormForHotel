using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormForHotel
{
    class DaModel
    {
        public int id { get; set; }

    }
    public class GuestCheck()
    {
        public int id { get; set; }
        public int guestcheckid { get; set; }
        public DateTime busdate { get; set; }
        public int locationid { get; set; }
        public int revenuecenterid { get; set; }
        public int checkNum { get; set; }
        public DateTime openDateTime { get; set; }
        public decimal checkTotal { get; set; }
        public int numItems { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int is_download { get; set; }
        //public DateTime downoad_datetime;
        public string getdatadate { get; set; }
    }
}
