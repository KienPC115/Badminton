using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data.DAO;
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
        public Task<IBadmintonResult> GetCourtById(int courtId);
        public Task<IBadmintonResult> AddCourt(Court court);
        public Task<IBadmintonResult> UpdateCourt(int courtId, Court court);
        public Task<IBadmintonResult> DeleteCourt(int courtId);
    }

    public class CourtBusiness : ICourtBusiness {
        private readonly CourtDAO _DAO;

        public CourtBusiness() {
            _DAO = new CourtDAO();
        }

        public async Task<IBadmintonResult> AddCourt(Court court) {
            try {
                int result = await _DAO.CreateAsync(court);
                if (result < 1) {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCourts() {
            try {
                var courts = await _DAO.GetAllAsync();
                if (courts == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courts!);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourtById(int courtId) {
            try {
                var court = await _DAO.GetByIdAsync(courtId);
                if (court == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, court);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCourt(int courtId, Court updateCourt) {
            try {
                var court = await _DAO.GetByIdAsync(courtId);
                if (court == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                court.Name = updateCourt.Name;
                court.Status = updateCourt.Status;
                court.Description = updateCourt.Description;
                court.DayPrice = updateCourt.DayPrice;
                court.WeekendDayPrice = updateCourt.WeekendDayPrice;
                court.NightPrice = updateCourt.NightPrice;
                court.WeekendNightPrice = updateCourt.WeekendNightPrice;

                if (await _DAO.UpdateAsync(court) > 0) {
                    return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }

                return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCourt(int courtId) {
            try {
                var court = await _DAO.GetByIdAsync(courtId);
                if (court == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                if (await _DAO.RemoveAsync(court)) {
                    return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                }

                return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
