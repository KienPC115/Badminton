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
    public interface ICustomerBusiness {
        public Task<IBadmintonResult> GetAllCustomers();
        public Task<IBadmintonResult> GetCustomerById(int customerId);
        public Task<IBadmintonResult> AddCustomer(Customer customer);
        public Task<IBadmintonResult> UpdateCustomer(int customerId, Customer customer);
        public Task<IBadmintonResult> DeleteCustomer(int customerId);
    }

    public class CustomerBusiness : ICustomerBusiness {
        private readonly CustomerDAO _DAO;

        public CustomerBusiness() {
            _DAO = new CustomerDAO();
        }

        public async Task<IBadmintonResult> AddCustomer(Customer customer) {
            try {
                int result = await _DAO.CreateAsync(customer);
                if (result < 1) {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCustomers() {
            try {
                var customers = await _DAO.GetAllAsync();
                if (customers == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, customers);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCustomerById(int customerId) {
            try {
                var customer = await _DAO.GetByIdAsync(customerId);
                if (customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, customer);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCustomer(int customerId, Customer updateCustomer) {
            try {
                var customer = await _DAO.GetByIdAsync(customerId);
                if (customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                customer.Name = updateCustomer.Name;
                customer.Email = updateCustomer.Email;
                customer.Phone = updateCustomer.Phone;
                customer.DateOfBirth = updateCustomer.DateOfBirth;
                customer.Address = updateCustomer.Address;

                if (await _DAO.UpdateAsync(customer) > 0) {
                    return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }

                return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            catch (Exception ex) {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCustomer(int customerId) {
            try {
                var customer = await _DAO.GetByIdAsync(customerId);
                if (customer == null) {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                if (await _DAO.RemoveAsync(customer)) {
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
