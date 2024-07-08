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

        public async Task<List<CourtDetail>> GetAllCourtDetailsIncludeCourtAsync(string? Search)
        {
            IQueryable<CourtDetail> query = _context.CourtDetails;
            query = query.Where(cd => cd.Status.ToLower() != "delete")
                .Include(cd => cd.Court)
                .OrderBy(cd => cd.Court.Name);
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.Price.ToString().Contains(Search.ToLower())
                                         || x.Status.Contains(Search.ToLower())
                                         || x.Court.Name.ToLower().Contains(Search.ToLower()));
            }

            return await query.ToListAsync();
        }

        public CourtDetail GetCourtDetailIncludeCourt(int id)
        {
            var courtDetail = _context.CourtDetails.FirstOrDefault(c => c.CourtDetailId == id);
            courtDetail.Court = _context.Courts.FirstOrDefault(c => c.CourtId == courtDetail.CourtId);
            courtDetail.Court.CourtDetails = null;
            return courtDetail;
        }
    }
}