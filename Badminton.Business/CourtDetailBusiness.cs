using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Badminton.Business.Shared;

namespace Badminton.Business
{
    public interface ICourtDetailBusiness
    {
        public Task<IBadmintonResult> GetAllCourtDetails();
        public Task<IBadmintonResult> GetAllCourtDetailsIncludeCourt();

        public Task<IBadmintonResult> GetCourtDetail(int courtDetailId);
        public Task<IBadmintonResult> AddCourtDetail(CourtDetail courtDetail);
        public Task<IBadmintonResult> UpdateCourtDetail(int courtDetailId, CourtDetail courtDetail,string msg);
        public Task<IBadmintonResult> DeleteCourtDetail(int courtDetailId);
    }
    public class CourtDetailBusiness : ICourtDetailBusiness
    {
        /*private readonly K17221goodmintonContext _context;*/
        /*private readonly CourtDetailDAO _dao;*/
        private readonly UnitOfWork _unitOfWork;
        public CourtDetailBusiness()
        {
            /*_dao = new CourtDetailDAO();*/
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBadmintonResult> AddCourtDetail(CourtDetail courtDetail)
        {
            try
            {

                _unitOfWork.CourtDetailRepository.PrepareCreate(courtDetail);
                var result = await _unitOfWork.CourtDetailRepository.SaveAsync();

                return result < 1 ? new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG)
                                  : new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCourtDetail(int courtDetailId)
        {
            try
            {
                
                /*var courtDetail = await _context.CourtDetails.FindAsync(courtDetailId);*/
                var courtDetail = await _unitOfWork.CourtDetailRepository.GetByIdAsync(courtDetailId);
                if (courtDetail == null)
                {
                    return new BadmintonResult(0, $"Court Detail {courtDetailId} not found");
                }
                var result = await _unitOfWork.CourtDetailRepository.RemoveAsync(courtDetail);

                return result is true ? new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG)
                                      : new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCourtDetails()
        {
            try
            {
                /*var courtDetails = await _context.CourtDetails.ToListAsync();*/
                var courtDetails = await _unitOfWork.CourtDetailRepository.GetAllAsync();
                return courtDetails.Count  > 0 ? new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courtDetails)
                                               : new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBadmintonResult> GetAllCourtDetailsIncludeCourt()
        {
            try
            {
                var courtDetails = await _unitOfWork.CourtDetailRepository.GetAllCourtDetailsIncludeCourtAsync();
                return courtDetails.Count > 0 ? new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courtDetails)
                                               : new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBadmintonResult> GetCourtDetail(int courtDetailId)
        {
            try
            {
                /*var courtDetail = await _context.Courts.FindAsync(courtDetailId);*/
                var courtDetail = await _unitOfWork.CourtDetailRepository.GetByIdAsync(courtDetailId);
                return courtDetail != null ? new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courtDetail)
                                               : new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCourtDetail(int courtDetailId, CourtDetail updateCourtDetail, string msg)
        {
            try
            {
                /*var courtDetail = await _context.CourtDetails.FindAsync(courtDetailId);*/
                var courtDetail = await _unitOfWork.CourtDetailRepository.GetByIdAsync(courtDetailId);
                if (courtDetail == null)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }

                courtDetail.CourtId = updateCourtDetail.CourtId;
                courtDetail.Slot = updateCourtDetail.Slot;
                courtDetail.Price = updateCourtDetail.Price;
                courtDetail.Status = updateCourtDetail.Status;
                var result = await _unitOfWork.CourtDetailRepository.UpdateAsync(courtDetail);
                if (msg.ToLower().Contains(CourtDetailShared.DELETE))
                {
                    return result > 0
                        ? new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG)
                        : new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }
                return  result > 0
                    ? new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG)
                    : new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
