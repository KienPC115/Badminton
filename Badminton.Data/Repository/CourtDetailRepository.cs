using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.VisualBasic;

namespace Badminton.Data.Repository
{
    public class CourtDetailRepository : GenericRepository<CourtDetail>
    {
        public CourtDetailRepository()
        {
        }
        public CourtDetailRepository(Net1710_221_8_BadmintonContext context) => _context = context;

        public async Task<CourtDetail> GetTopBookedCourt()
        {
            return await _context.CourtDetails.Include(cd => cd.Court).OrderByDescending(cd => cd.BookingCount).FirstOrDefaultAsync();
        }
        public async Task<CourtDetail> GetByIdAsync(int id)
        {
            return await _context.CourtDetails.Where(cd => cd.CourtDetailId == id).Include(cd => cd.Court).FirstOrDefaultAsync();
        }
        public async Task<List<CourtDetail>> GetAllCourtDetailsIncludeCourtAsync(string? slot,int? capacity, double? minPrice, double? maxPrice)
        {
            IQueryable<CourtDetail> query = _context.CourtDetails;
            query = query.Where(cd => cd.Status.ToLower() != "delete")
                .Include(cd => cd.Court)
                .OrderBy(cd => cd.Status == "Available" ? 0 : 1)
                .ThenByDescending(cd => cd.BookingCount)
                .ThenBy(cd => cd.Court.Name);
            
            if (!string.IsNullOrEmpty(slot))
            {
                query = query.Where(x => x.Slot.ToLower().Contains(slot.ToLower()));
            }

            if (capacity != null)
            {
                query = query.Where(x => x.Capacity == capacity.Value);
            }

            if (minPrice != null)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
            }

            if (maxPrice != null)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
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
        public int RefreshCourtDetailStatus()
        {
            try
            {
                var courtDetails = _context.CourtDetails.Where(c => c.Status.Equals("Booked"));
                if (courtDetails == null || courtDetails.Count() == 0)
                {
                    return 1;
                }
                foreach (var item in courtDetails)
                {
                    item.Status = "Available";
                    _context.CourtDetails.Update(item);
                }
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}