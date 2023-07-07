using Models;
Customer customer1 = new Customer() { Name = "max", Surname = "muster" };
Customer customer2 = new() { Name = "sofia", Surname = "muster" };
Manager manager1 = new() { Name = "Heinz", Surname = "muster" };

Console.WriteLine(customer1.Name);
