using Badminton.Business.Interface;
using Badminton.Business.Shared;
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
        public Task<IBadmintonResult> GetCourtsByStatus(string status);
        public Task<IBadmintonResult> GetCourtById(int courtId);
        public Task<IBadmintonResult> AddCourt(Court court);
        public Task<IBadmintonResult> UpdateCourt(int courtId, Court court);
        public Task<IBadmintonResult> DeleteCourt(int courtId);
        public Task<IBadmintonResult> GetCourtIdByName(string name);
        public Task<IBadmintonResult> GetCourtsWithCondition(string? key, string sortOrder, string filterType, string filterYardType, string filterStatus, string filterLocation);
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
                court.CreatedTime = DateTime.UtcNow;
                court.UpdatedTime = DateTime.UtcNow;
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
                court.Location = updateCourt.Location;
                court.Type = updateCourt.Type;
                court.SpaceType = updateCourt.SpaceType;
                court.YardType = updateCourt.YardType;
                court.CreatedTime = updateCourt.CreatedTime;
                court.UpdatedTime = DateTime.UtcNow;
                court.Price = updateCourt.Price;
                
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

        public async Task<IBadmintonResult> GetCourtIdByName(string name)
        {
            try
            {
                var court = await _unitOfWork.CourtRepository.GetCourtIdByName(name);
                if (court == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG,court!);
                }
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourtsByStatus(string status) {
            try {
                //var courts = await _DAO.GetAllAsync();
                var courts = await _unitOfWork.CourtRepository.GetCourtsByStatusAsync(status);
                if (courts == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courts!);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCourtsWithCondition(string? key, string sortOrder, string filterType, string filterYardType, string filterStatus, string filterLocation) {
            try {
                var predicate = PredicateBuilder.True<Court>();

                if (!string.IsNullOrEmpty(key)) {
                    key = key.Trim();
                    predicate = predicate.And(x =>
                        x.Name.Contains(key)
                        || x.Price.ToString().Contains(key)
                        || x.Description.Contains(key)
                        || x.SpaceType.Contains(key)
                        || x.CreatedTime.ToString().Contains(key)
                        || x.UpdatedTime.ToString().Contains(key)
                        );
                }

                if (!string.IsNullOrEmpty(filterType) && filterType != "All") {
                    predicate = predicate.And(x => x.Type == filterType);
                }

                if (!string.IsNullOrEmpty(filterYardType) && filterYardType != "All") {
                    predicate = predicate.And(x => x.YardType == filterYardType);
                }

                if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "All") {
                    predicate = predicate.And(x => x.Status == filterStatus);
                }

                if (!string.IsNullOrEmpty(filterLocation) && filterLocation != "All") {
                    predicate = predicate.And(x => x.Location == filterLocation);
                }

                var query = await _unitOfWork.CourtRepository.FindAll(predicate);

                switch (sortOrder) {
                    case "name_desc":
                        query = query.OrderByDescending(x => x.Name);
                        break;
                    case "Price":
                        query = query.OrderBy(x => x.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(x => x.Price);
                        break;
                    case "CreatedTime":
                        query = query.OrderBy(x => x.CreatedTime);
                        break;
                    case "create_desc":
                        query = query.OrderByDescending(x => x.CreatedTime);
                        break;
                    case "UpdatedTime":
                        query = query.OrderBy(x => x.UpdatedTime);
                        break;
                    case "update_desc":
                        query = query.OrderByDescending(x => x.UpdatedTime);
                        break;
                    default:
                        query = query.OrderBy(x => x.Name);
                        break;
                }

                var courts = await query.ToListAsync();
                if (courts == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, courts!);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
