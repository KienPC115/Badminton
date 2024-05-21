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
        public Task<IBadmintonResult> GetAllCourts();
        public Task<IBadmintonResult> GetCourt(int courtId);
        public Task<IBadmintonResult> AddCourt(Court court);
        public Task<IBadmintonResult> UpdateCourt(int courtId, Court court);
        public Task<IBadmintonResult> DeleteCourt(int courtId);
    }

    public class CourtBusiness : ICourtBusiness{
        private readonly Net1710_221_8_BadmintonContext _context;

        public async Task<IBadmintonResult> AddCourt(Court court) {
            try {
                await _context.Courts.AddAsync(court);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1,"Court added successfully");
            }
            catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCourts() {
            try {
                var courts = await _context.Courts.ToListAsync();

                return new BadmintonResult(1, "Get courts successfully", courts);
            }
            catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourt(int courtId) {
            try {
                var court = await _context.Courts.FindAsync(courtId);
                if(court == null) {
                    return new BadmintonResult(0, $"Court {courtId} not found");
                }
                return new BadmintonResult(1, "Get court successfully", court);
            }
            catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCourt(int courtId, Court updateCourt) {
            try {
                var court = await _context.Courts.FindAsync(courtId);
                if(court == null) {
                    return new BadmintonResult(0, $"Court {courtId} not found");
                }

                court.Name = updateCourt.Name;
                court.Status = updateCourt.Status;
                court.Description = updateCourt.Description;
                court.DayPrice = updateCourt.DayPrice;
                court.WeekendDayPrice = updateCourt.WeekendDayPrice;
                court.NightPrice = updateCourt.NightPrice;
                court.WeekendNightPrice = updateCourt.WeekendNightPrice;

                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Court updated successfully");
            }
            catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCourt(int courtId) {
            try {
                var court = await _context.Courts.FindAsync(courtId);
                if (court == null) {
                    return new BadmintonResult(0, $"Court {courtId} not found");
                }

                _context.Remove(court);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Court deleted successfully");
            }
            catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }
    }
}
