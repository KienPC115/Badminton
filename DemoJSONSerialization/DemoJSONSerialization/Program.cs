using System.Text.Json;
using System.Text.Json.Serialization;

public class Employee
{
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
    public Employee()
    {
        
    }

    public Employee(decimal initialSalary)
    {
        Salary = initialSalary;
    }
    class Program
    {
        public static void Main(string[] args)
        {
            var emp1 = new Employee(1000M);
            emp1.Name = "Jack";
            emp1.Email = "jack@gmail.com";

            var option = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine("*****Serialize*****");
            string jsonData = JsonSerializer.Serialize(emp1, option);
            Console.WriteLine(jsonData);
            Console.WriteLine("*****Deserialize*****");
            var emp2 = JsonSerializer.Deserialize<Employee>(jsonData);
            Console.WriteLine($"Name:{emp2.Name}, Email:{emp2.Email}, Salary:{emp2.Salary}");
            Console.WriteLine();
        }
    }
}