using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data.Repository
{
    public class CourtDetailRepository : GenericRepository<CourtDetail>
    {
        public CourtDetailRepository()
        {
        }
        public CourtDetailRepository(Net1710_221_8_BadmintonContext context) => _context = context;

        public async Task<List<CourtDetail>> GetAllCourtDetailsIncludeCourtAsync()
        {
            var result =await _context.CourtDetails
                .Where(cd => cd.Status.ToLower() != "delete")
                .Include(cd => cd.Court)
                .OrderBy(cd => cd.Court.Name)
                .ToListAsync();
            return result;
        }

    }
}
