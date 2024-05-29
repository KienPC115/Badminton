using Badminton.Data.Models;
using Badminton.Data.Repositories;
using Badminton.Data.Repository;

namespace Badminton.Data {
    public class UnitOfWork {
        private Net1710_221_8_BadmintonContext _unitOfWorkContext;
        private CourtRepository _court;
        private CourtDetailRepository _courtDetail;
        private CustomerRepository _customer;

        public CourtRepository CourtRepository {
            get {
                return _court ??= new CourtRepository();
            }
        }
        public CourtDetailRepository CourtDetailRepository {
            get {
                return _courtDetail ??= new CourtDetailRepository();
            }
        }

        public CustomerRepository CustomerRepository {
            get {
                return _customer ??= new CustomerRepository();
            }
        }
    }
}