using Badminton.Business.Interface;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business {
    public interface ICourtBusiness {
        Task<IBadmintonResult> GetAllCourts();
    }

    public class CourtBusiness : ICourtBusiness{
        private readonly Net1710_221_8_BadmintonContext _context;

        public async Task<IBadmintonResult> GetAllCourts() {
            var courts = await _context.Courts.ToListAsync();

            return new BadmintonResult(1, "Success", courts);
        }

        // status message data


    }
}
