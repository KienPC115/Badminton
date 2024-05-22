using Badminton.Business.Interface;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business
{
    public interface ICourtDetailBusiness
    {
        public Task<IBadmintonResult> GetAllCourtDetails();
        public Task<IBadmintonResult> GetCourtDetail(int courtDetailId);
        public Task<IBadmintonResult> AddCourtDetail(CourtDetail courtDetail);
        public Task<IBadmintonResult> UpdateCourtDetail(int courtDetailId, CourtDetail courtDetail);
        public Task<IBadmintonResult> DeleteCourtDetail(int courtDetailId);
    }
    public class CourtDetailBusiness : ICourtDetailBusiness
    {
        private readonly Net1710_221_8_BadmintonContext _context;

        public async Task<IBadmintonResult> AddCourtDetail(CourtDetail courtDetail)
        {
            try
            {
                await _context.CourtDetails.AddAsync(courtDetail);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Court Detail added successfully");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCourtDetail(int courtDetailId)
        {
            try
            {
                var courtDetail = await _context.CourtDetails.FindAsync(courtDetailId);
                if (courtDetail == null)
                {
                    return new BadmintonResult(0, $"Court Detail {courtDetailId} not found");
                }

                _context.Remove(courtDetail);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Court Detail deleted successfully");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCourtDetails()
        {
            try
            {
                var courtDetails = await _context.CourtDetails.ToListAsync();

                return new BadmintonResult(1, "Get courts details successfully", courtDetails);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourtDetail(int courtDetailId)
        {
            try
            {
                var courtDetail = await _context.Courts.FindAsync(courtDetailId);
                if (courtDetail == null)
                {
                    return new BadmintonResult(0, $"Court {courtDetailId} not found");
                }
                return new BadmintonResult(1, "Get court successfully", courtDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCourtDetail(int courtDetailId, CourtDetail updateCourtDetail)
        {
            try
            {
                var courtDetail = await _context.CourtDetails.FindAsync(courtDetailId);
                if (courtDetail == null)
                {
                    return new BadmintonResult(0, $"Court Detail {courtDetailId} not found");
                }

                courtDetail.Price = updateCourtDetail.Price;
                courtDetail.Status = updateCourtDetail.Status;
                courtDetail.StartTime = updateCourtDetail.StartTime;
                courtDetail.EndTime = updateCourtDetail.EndTime;
                courtDetail.StaffId = updateCourtDetail.StaffId;
                courtDetail.CourtId = updateCourtDetail.CourtId;

                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Court Detail updated successfully");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }
    }
}
