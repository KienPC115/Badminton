using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Court>> GetCourtsByStatusAsync(string status) {
            return await _context.Courts
                .Where(x => x.Status.ToLower() == status.ToLower())
                .ToListAsync();
        }

        /*public async Task<List<Court>> GetCourtsByKeyword(string key) {
            if(string.IsNullOrEmpty(key))
                return await _context.Courts.ToListAsync();

            key = key.Trim();
            return await _context.Courts
                .Where(x => x.Name.ToLower().Contains(key.ToLower())
                || x.Description.ToLower().Contains(key.ToLower())
                || x.Price.ToString().ToLower().Contains(key.ToLower())
                || x.Status.ToLower().Contains(key.ToLower()))
                .ToListAsync();
        }*/

        public async Task<Court> GetCourtIdByName(string name)
        {
            return await _context.Courts.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
