using Badminton.Data.Base;
using Badminton.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data.Repository {
    public class CourtRepository : GenericRepository<Court> {
        public CourtRepository()
        {
        }

        public CourtRepository(Net1710_221_8_BadmintonContext context) => _context = context;
    }
}
