using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Badminton.Business.Interface;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Badminton.Business {

    public interface ICustomerBusiness {
        public Task<IBadmintonResult> GetAllCustomers();
        public Task<IBadmintonResult> GetCustomer(int CustomerId);
        public Task<IBadmintonResult> AddCustomer(Customer Customer);
        public Task<IBadmintonResult> UpdateCustomer(int CustomerId, Customer Customer);
        public Task<IBadmintonResult> DeleteCustomer(int CustomerId);
    }

    internal class CustomerBusiness : ICustomerBusiness {
        private readonly Net1710_221_8_BadmintonContext _context;

        public async Task<IBadmintonResult> AddCustomer(Customer Customer) {
            try {
                await _context.Customers.AddAsync(Customer);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Customer added successfully");
            } catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllCustomers() {
            try {
                var Customers = await _context.Customers.ToListAsync();

                return new BadmintonResult(1, "Get Customers successfully", Customers);
            } catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetCustomer(int CustomerId) {
            try {
                var Customer = await _context.Customers.FindAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(0, $"Customer {CustomerId} not found");
                }
                return new BadmintonResult(1, "Get Customer successfully", Customer);
            } catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateCustomer(int CustomerId, Customer updateCustomer) {
            try {
                var Customer = await _context.Customers.FindAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(0, $"Customer {CustomerId} not found");
                }

                Customer.Name = updateCustomer.Name;
                Customer.Phone = updateCustomer.Phone;
                Customer.Email = updateCustomer.Email;
                Customer.Address = updateCustomer.Address;
                Customer.DateOfBirth = updateCustomer.DateOfBirth;

                _context.Customers.Update(Customer);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Customer updated successfully");
            } catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteCustomer(int CustomerId) {
            try {
                var Customer = await _context.Customers.FindAsync(CustomerId);
                if (Customer == null) {
                    return new BadmintonResult(0, $"Customer {CustomerId} not found");
                }

                _context.Remove(Customer);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Customer deleted successfully");
            } catch (Exception ex) {
                return new BadmintonResult(-1, ex.Message);
            }
        }

    }
}
