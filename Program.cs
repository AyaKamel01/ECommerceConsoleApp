
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceConsoleApp
{
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void Display()
        {
            Console.WriteLine(
                $"{Id}. {Name} | Price: {Price:C} | Stock: {Stock}");
        }
    }

    class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal Total => Product.Price * Quantity;

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    class ShoppingCart
    {
        private List<CartItem> items = new List<CartItem>();

        public void AddProduct(Product product, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be greater than zero.");
                return;
            }

            if (quantity > product.Stock)
            {
                Console.WriteLine("Not enough stock available.");
                return;
            }

            CartItem existingItem =
                items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > product.Stock)
                {
                    Console.WriteLine("Not enough stock available.");
                    return;
                }

                existingItem.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem(product, quantity));
            }

            Console.WriteLine("Product added to cart successfully.");
        }

        public void ViewCart()
        {
            if (items.Count == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            Console.WriteLine("\n===== Shopping Cart =====");

            foreach (CartItem item in items)
            {
                Console.WriteLine(
                    $"{item.Product.Name} x {item.Quantity} = {item.Total:C}");
            }

            Console.WriteLine($"Total: {GetTotal():C}");
        }

        public decimal GetTotal()
        {
            return items.Sum(item => item.Total);
        }

        public bool IsEmpty()
        {
            return items.Count == 0;
        }

        public bool Checkout()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Your cart is empty.");
                return false;
            }

            foreach (CartItem item in items)
            {
                if (item.Quantity > item.Product.Stock)
                {
                    Console.WriteLine(
                        $"Insufficient stock for {item.Product.Name}.");
                    return false;
                }
            }

            Console.WriteLine("\n===== INVOICE =====");

            foreach (CartItem item in items)
            {
                Console.WriteLine(
                    $"{item.Product.Name} x {item.Quantity} = {item.Total:C}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine($"Grand Total: {GetTotal():C}");

            foreach (CartItem item in items)
            {
                item.Product.Stock -= item.Quantity;
            }

            items.Clear();

            Console.WriteLine("Order completed successfully!");
            return true;
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>
        {
            new Product(1, "Laptop", 25000m, 5),
            new Product(2, "Headphones", 1200m, 10),
            new Product(3, "Keyboard", 800m, 8),
            new Product(4, "Mouse", 400m, 15),
            new Product(5, "Monitor", 6500m, 4)
        };

        static ShoppingCart cart = new ShoppingCart();

        static void Main()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== E-Commerce Store =====");
                Console.WriteLine("1. View Products");
                Console.WriteLine("2. Search Product");
                Console.WriteLine("3. Add Product to Cart");
                Console.WriteLine("4. View Shopping Cart");
                Console.WriteLine("5. Checkout");
                Console.WriteLine("6. Admin: Add Product");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewProducts();
                        break;

                    case "2":
                        SearchProduct();
                        break;

                    case "3":
                        AddToCart();
                        break;

                    case "4":
                        cart.ViewCart();
                        break;

                    case "5":
                        cart.Checkout();
                        break;

                    case "6":
                        AddNewProduct();
                        break;

                    case "7":
                        running = false;
                        Console.WriteLine("Thank you for visiting!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void ViewProducts()
        {
            Console.WriteLine("\n===== Available Products =====");

            foreach (Product product in products)
            {
                product.Display();
            }
        }

        static void SearchProduct()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine() ?? "";

            var results = products
                .Where(p => p.Name.Contains(
                    name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (Product product in results)
            {
                product.Display();
            }
        }

        static void AddToCart()
        {
            ViewProducts();

            Console.Write("Enter product ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }

            Product product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.Write("Enter quantity: ");

            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Invalid quantity.");
                return;
            }

            cart.AddProduct(product, quantity);
        }

        static void AddNewProduct()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Product name cannot be empty.");
                return;
            }

            Console.Write("Enter product price: ");

            if (!decimal.TryParse(
                Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Enter stock quantity: ");

            if (!int.TryParse(
                Console.ReadLine(), out int stock) || stock < 0)
            {
                Console.WriteLine("Invalid stock quantity.");
                return;
            }

            int newId = products.Count == 0
                ? 1
                : products.Max(p => p.Id) + 1;

            products.Add(new Product(newId, name, price, stock));

            Console.WriteLine("Product added successfully!");
        }
    }
}
