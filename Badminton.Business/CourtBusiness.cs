using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business
{
    public interface ICourtBusiness
    {
        public Task<IBadmintonResult> GetAllCourts();
        public Task<IBadmintonResult> GetCourtById(int courtId);
        public Task<IBadmintonResult> AddCourt(Court court);
        public Task<IBadmintonResult> UpdateCourt(int courtId, Court court);
        public Task<IBadmintonResult> DeleteCourt(int courtId);
    }

    public class CourtBusiness : ICourtBusiness
    {
        //private readonly CourtDAO _DAO;
        private readonly UnitOfWork _unitOfWork;

        public CourtBusiness()
        {
            //_DAO = new CourtDAO();
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBadmintonResult> AddCourt(Court court)
        {
            try
            {
                _unitOfWork.CourtRepository.PrepareCreate(court);
                int result = await _unitOfWork.CourtRepository.SaveAsync();
                if (result < 1)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCourts()
        {
            try
            {
                //var courts = await _DAO.GetAllAsync();
                var courts = await _unitOfWork.CourtRepository.GetAllAsync();
                if (courts == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courts!);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourtById(int courtId)
        {
            try
            {
                //var court = await _DAO.GetByIdAsync(courtId);\
                var court = await _unitOfWork.CourtRepository.GetByIdAsync(courtId);
                if (court == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, court);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCourt(int courtId, Court updateCourt)
        {
            try
            {
                //var court = await _DAO.GetByIdAsync(courtId);
                var court = await _unitOfWork.CourtRepository.GetByIdAsync(courtId);
                if (court == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                court.Name = updateCourt.Name;
                court.Status = updateCourt.Status;
                court.Description = updateCourt.Description;
                court.DayPrice = updateCourt.DayPrice;
                court.WeekendDayPrice = updateCourt.WeekendDayPrice;
                court.NightPrice = updateCourt.NightPrice;
                court.WeekendNightPrice = updateCourt.WeekendNightPrice;

                if (await _unitOfWork.CourtRepository.UpdateAsync(court) > 0)
                {
                    return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }

                return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBadmintonResult> DeleteCourt(int courtId)
        {
            try
            {
                var court = await _unitOfWork.CourtRepository.GetByIdAsync(courtId);
                if (court == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                if (await _unitOfWork.CourtRepository.RemoveAsync(court))
                {
                    return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                }

                return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
