using Badminton.Data.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace Badminton.Assignment2.WPF.Repository
{
    public class OrderRepository
    {
        private static string jsonFilePath = "../../../../Badminton.Assignment2.WPF/Data/Orders.json";
        private static string xmlFilePath = "../../../../Badminton.Assignment2.WPF/Data/Orders.xml";
        //private static string jsonFilePath = "D:\\now_semester\\PRN221\\Project_PRN221\\Badminton\\Badminton.Assignment2.WPF\\Data\\Orders.json";
        //private static string xmlFilePath = "D:\\now_semester\\PRN221\\Project_PRN221\\Badminton\\Badminton.Assignment2.WPF\\Data\\Orders.xml";
        private static List<Order> orders = new List<Order>();
        private static void LoadOrdersFromJson()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                
                orders = JsonSerializer.Deserialize<List<Order>>(jsonString);

            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void LoadOrdersFromXml()
        {
            try
            {
                orders = new List<Order>();
                using Stream s1 = File.OpenRead(xmlFilePath);
                var xs = new XmlSerializer(typeof(List<Order>));
                orders = (List<Order>)xs.Deserialize(s1);
                s1.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file '{xmlFilePath}' was not found: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"There was an error deserializing the XML data: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }
        private static void SaveOrdersToXml()
        {
            try
            {
                var xs = new XmlSerializer(typeof(List<Order>));
                using (Stream s1 = File.Create(xmlFilePath))
                {
                    xs.Serialize(s1, orders);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ChangeSource(string f)
        {
            if (f == "Json")
            {
                LoadOrdersFromJson();
                return;
            }
            LoadOrdersFromXml();
        }
        public void SaveChange(string f)
        {
            if(f == "Json")
            {
                SaveOrdersToJson();
                return;
            }
            SaveOrdersToXml();
        }
        private static void GetIndex(int id, out int num)
        {
            num = 0;
            foreach (var o in orders)
            {
                if (o.OrderId == id) num = orders.IndexOf(o);
            }
        }
        
        private static bool SaveOrdersToJson()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(jsonFilePath, jsonString);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<Order> GetAll()
        {
            return orders;
        }

        public List<Order> GetBySearching(string searchString)
        {
            return orders.Where(o => o.OrderNotes.Trim().ToUpper() == searchString.Trim().ToUpper()).ToList();
        }

        public Order GetById(int id) => orders.FirstOrDefault(o => o.OrderId == id);
        
        public void Update(Order order)
        {
            GetIndex(order.OrderId, out int num);
            order.Customer.Orders = null;
            order.OrderDetails = null;
            orders[num] = order;
        }
        
        public void Insert(Order order)
        {
            var o = orders.FindLast(o => true);
            order.OrderId = ++o.OrderId;
            order.Customer.Orders = null;
            order.OrderDetails = null;
            orders.Add(order);
        }

        public void Delete(int id)
        {
            GetIndex(id, out int num);
            orders.RemoveAt(num);
        }
    }
}
