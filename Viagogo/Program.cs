using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viagogo
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    public class IgnoreCaseComparator : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) =>
            x.Equals(y, StringComparison.OrdinalIgnoreCase);
      
        public int GetHashCode(string obj) =>
            obj[0].GetHashCode();

    }
    public class Program
    {
        static void Main(string[] args)
        {
            const int noOfEvents = 5;
            var events = new List<Event>
            {
                new Event { Name = "Phantom of the Opera", City = "New York" },
                new Event { Name = "Metallica", City = "Los Angeles" },
                new Event { Name = "Metallica", City = "New York" },
                new Event { Name = "Metallica", City = "Boston" },
                new Event { Name = "LadyGaGa", City = "New York" },
                new Event { Name = "LadyGaGa", City = "Boston" },
                new Event { Name = "LadyGaGa", City = "Chicago" },
                new Event { Name = "LadyGaGa", City = "San Francisco" },
                new Event { Name = "LadyGaGa", City = "Washington" }
            };

            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            var customers = new List<Customer>
            {
                new Customer { Name = "Nathan", City = "New York" },
                new Customer { Name = "Bob", City = "Boston" },
                new Customer { Name = "Cindy", City = "Chicago" },
                new Customer { Name = "Lisa", City = "Los Angeles" }
            };


            // #1. find out all events that are in cities of customers for a multiple customers 
            // then add to email.
            // i Used Custom EquityComparator 'IgnorCaseComparator()'  b/c sting comparing might be tricky due to typo mistake

            Console.WriteLine("Q #1 -Find out all events that are in cities of customers for a multiple customers then add to email");
            events.Join(customers, e => e.City, c => c.City, (e, c) =>
                    new { Event = e, Customer = c },
                    new IgnoreCaseComparator())
                .OrderBy(r => r.Customer.Name)
                .ToList()
                .ForEach(r => AddToEmail(r.Customer, r.Event));
            Console.WriteLine("----------------------------END END Q#1 Multiple Customers------------------------");
            Console.WriteLine("\n");


            // #1. find out all events that are in cities of customer for a single customer
            // then add to email.
            Console.WriteLine("Q #1 - Find out all events that are in cities of a customer for a a single customer then add to email");
            events.Where(e => e.City.Trim().Equals(customer.City, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(e => AddToEmail(customer, e));
            Console.WriteLine("----------------------------END Q#1 Single Customer------------------------");
            Console.WriteLine("\n");


            // #2. finding all events at customer city and nearby cities and maximum of 5 events per customer
            //    for a single customer
            Console.WriteLine("Q #2 - finding all events at customer city and nearby cities and maximum of 5 events per customer for a single customer then add to email");
            events
                .Select(e => new { Customer = customer, Event = e })
                .OrderBy(e => e.Customer)
                .ThenBy(e => GetDistance(e.Customer.City, e.Event.City))
                .Distinct()
                .Take(noOfEvents)
                .ToList()
                .ForEach(e => AddToEmail(e.Customer, e.Event));
            Console.WriteLine("----------------------------END Q#2------------------------");
            Console.WriteLine("\n");



            // #3. If the GetDistance method is an API call which could fail or is too expensive,
            // how will u improve the code written in 2 ? Write the code.
            // # 4.If the GetDistance method can fail, we don't want the process to fail. What can be done?
            // Code it. (Ask clarifying questions to be clear about what is expected business - wise)
            // i Added additional exception Handling method TryGetDistance(Customer.City, Event.City)
            // and the new logic to get Distance can be implemented there.
            Console.WriteLine("Q #3 & #4 - If the GetDistance method is an API call which could fail or is too expensive we don't want the process to fail");
            events
                .Select(e => new { Customer = customer, Event = e })
                .OrderBy(e => e.Customer)
                .ThenBy(e => TryGetDistance(e.Customer.City, e.Event.City))
                .Distinct()
                .Take(noOfEvents)
                .ToList()
                .ForEach(e => AddToEmail(e.Customer, e.Event));
            Console.WriteLine("----------------------------END Q#3 & #4------------------------");
            Console.WriteLine("\n");


            // #5 If we also want to sort the resulting events by other fields like price, etc. to determine which
            // ones to send to the customer, how would you implement it? Code it.
            Console.WriteLine("Q #5 - If we also want to sort the resulting events by other fields like price");
            events
                .Select(e => new { Customer = customer, Event = e })
                .OrderBy(e => e.Customer)
                .ThenBy(e => TryGetDistance(e.Customer.City, e.Event.City))
                .ThenBy(e => GetPrice(e.Event))
                .Distinct()
                .Take(noOfEvents)
                .ToList()
                .ForEach(e => AddToEmail(e.Customer, e.Event, GetPrice(e.Event)));
            Console.WriteLine("----------------------------END Q#5------------------------");
            Console.ReadKey();
        }

        static int TryGetDistance(string fromCity, string toCity)
        {
            try
            {
               return GetDistance(fromCity, toCity);
            }
            catch (Exception e)
            {
                return 0;
            }
           
        }
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                                  + (distance > 0 ? $" ({distance} miles away)" : "")
                                  + (price.HasValue ? $" for ${price}" : ""));
        }
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }
}
