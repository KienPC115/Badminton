using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Badminton.Business {

    public interface ICustomerBusiness {
        public Task<IBadmintonResult> GetAllCustomers();
        public Task<IBadmintonResult> GetCustomerById(int CustomerId);
        public Task<IBadmintonResult> AddCustomer(Customer Customer);
        public Task<IBadmintonResult> GetAllCustomersWithSearchKey(string Search);
        public Task<IBadmintonResult> UpdateCustomer(int CustomerId, Customer Customer);
        public Task<IBadmintonResult> DeleteCustomer(int CustomerId);
        Task<IBadmintonResult> CheckLogin(string email, string password);
    }

    public class CustomerBusiness : ICustomerBusiness {
        //private readonly CustomerDAO _DAO;
        private readonly UnitOfWork _unitOfWork;

        public CustomerBusiness() {
            //_DAO = new CustomerDAO();
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBadmintonResult> AddCustomer(Customer Customer) {
            try {
                _unitOfWork.CustomerRepository.PrepareCreate(Customer);
                int result = await _unitOfWork.CustomerRepository.SaveAsync();
                if (result < 1) {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            } catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCustomersWithSearchKey(string Search)
        {
            var Customers = await _unitOfWork.CustomerRepository.GetAllCustomerBySearchKeyAsync(Search);
            return Customers.Count >= 1
                ? new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Customers)
                : new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
        }

        public async Task<IBadmintonResult> GetAllCustomers() {
            try {
                //var Customers = await _DAO.GetAllAsync();
                var Customers = await _unitOfWork.CustomerRepository.GetAllAsync();
                if (Customers == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Customers!);
            } catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> CheckLogin(string email, string password)
        {
            try
            {
                //var Customers = await _DAO.GetAllAsync();
                var Customers = await _unitOfWork.CustomerRepository.GetAllAsync();
                var customer = Customers.FirstOrDefault(c => c.Email.Equals(email) && c.Password.Equals(password));
                if (customer == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, customer);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCustomerById(int CustomerId) {
            try {
                //var Customer = await _DAO.GetByIdAsync(CustomerId);\
                var Customer = await _unitOfWork.CustomerRepository.GetByIdAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Customer);
            } catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCustomer(int CustomerId, Customer updateCustomer) {
            try {
                //var Customer = await _DAO.GetByIdAsync(CustomerId);
                var Customer = await _unitOfWork.CustomerRepository.GetByIdAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                Customer.Name = updateCustomer.Name;
                Customer.DateOfBirth = updateCustomer.DateOfBirth;
                Customer.Email = updateCustomer.Email;
                Customer.Phone = updateCustomer.Phone;
                Customer.Address = updateCustomer.Address;
                Customer.Password = updateCustomer.Password;

                if (await _unitOfWork.CustomerRepository.UpdateAsync(Customer) > 0) {
                    return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }

                return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            } catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBadmintonResult> DeleteCustomer(int CustomerId) {
            try {
                var Customer = await _unitOfWork.CustomerRepository.GetByIdAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                if (await _unitOfWork.CustomerRepository.RemoveAsync(Customer)) {
                    return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                }

                return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            } catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
