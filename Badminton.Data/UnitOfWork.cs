using Badminton.Data.Models;
using Badminton.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data {
    public class UnitOfWork {
        private Net1710_221_8_BadmintonContext _unitOfWorkContext;

        private CourtRepository _court;

        public UnitOfWork() {
        }

        public CourtRepository CourtRepository { 
            get 
            { 
                return _court ??= new CourtRepository(); 
            } 
        }
    }
}
