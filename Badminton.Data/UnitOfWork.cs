using Badminton.Data.Models;
using Badminton.Data.Repository;

namespace Badminton.Data {
    public class UnitOfWork {
        private Net1710_221_8_BadmintonContext _unitOfWorkContext;
        private CourtRepository _court;
        private CourtDetailRepository _courtDetail;

        public CourtRepository CourtRepository
        {
            get
            {
                return _court ??= new CourtRepository();
            }
        }
        public CourtDetailRepository CourtDetailRepository
        {
            get
            {
                return _courtDetail ??= new CourtDetailRepository();
            }
        }
    }
}