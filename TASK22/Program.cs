using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "path/to/json/files"; // шлях до файлів JSON
        Predicate<Product> filter = p => true; // фільтр за замовчуванням

        // Запитати користувача про критерії фільтрації
        Console.WriteLine("Enter filter criteria (or press Enter to skip):");
        Console.Write("Name contains: ");
        string nameContains = Console.ReadLine();
        Console.Write("Minimum price: ");
        double minPrice = double.TryParse(Console.ReadLine(), out double result) ? result : 0;
        Console.Write("Maximum price: ");
        double maxPrice = double.TryParse(Console.ReadLine(), out result) ? result : double.MaxValue;

        // Змінити фільтр на основі критеріїв користувача
        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            filter = filter.And(p => p.Name.Contains(nameContains));
        }
        filter = filter.And(p => p.Price >= minPrice && p.Price <= maxPrice);

        // Прочитати файли JSON та відфільтрувати продукти
        List<Product> products = new List<Product>();
        for (int i = 1; i <= 10; i++)
        {
            string fileName = Path.Combine(filePath, $"{i}.json");
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                List<Product> productList = JsonConvert.DeserializeObject<List<Product>>(json);
                products.AddRange(productList.Where(filter));
            }
        }

        // Відобразити відфільтровані продукти
        Console.WriteLine("Filtered products:");
        foreach (var product in products)
        {
            Console.WriteLine($"Name: {product.Name}, Price: {product.Price}");
        }
    }
}

class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
}

static class PredicateExtensions
{
    public static Predicate<T> And<T>(this Predicate<T> left, Predicate<T> right)
    {
        return x => left(x) && right(x);
    }
}

