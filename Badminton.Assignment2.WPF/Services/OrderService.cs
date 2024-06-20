using Badminton.Assignment2.WPF.Repository;
using Badminton.Data.Models;

namespace Badminton.Assignment2.WPF.Services
{
    public class OrderService
    {
        OrderRepository _repo = new();
        public List<Order> GetAll() => _repo.GetAll();
        public List<Order> Search(string searchStr) => _repo.GetBySearching(searchStr);
        public void Update(Order order) => _repo.Update(order);
        public void Insert(Order order) => _repo.Insert(order);
        public Order GetById(int id) => _repo.GetById(id);
        public void Delete(int id) => _repo.Delete(id);
        public void SaveChange(string f) => _repo.SaveChange(f);
        public void ChangeSource(string f) => _repo.ChangeSource(f);
    }
}
