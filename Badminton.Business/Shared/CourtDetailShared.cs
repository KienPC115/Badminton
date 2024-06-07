using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business.Shared
{
    public static class CourtDetailShared
    {
        public static List<string> Slot()
        {
            string[] Slot = {"08h00-9h30","09h30-11h00","12h00-13h30","13h30-15h00","15h00-16h30","16h30-18h00","18h00-19h30","19h30-21h00","21h00-22h30" };
            return new List<string>(Slot);
        }
        public static List<string> Status() {
            string[] Status = { "Available", "Booked"};
            return new List<string>(Status);
        }
    }
}
