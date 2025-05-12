using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Data
{
    public class Seed 
    {
        private readonly DataContext _dataContext;

        public Seed(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public void SeedDataContext()
        {
            // Delete existing data in correct order due to foreign key constraints
            _dataContext.ProductCategories.RemoveRange(_dataContext.ProductCategories);
            _dataContext.Products.RemoveRange(_dataContext.Products);
            _dataContext.Categories.RemoveRange(_dataContext.Categories);
            _dataContext.SaveChanges();

            var products = new List<Product>()
                {
                    new Product(){Name ="Ipad Air", Description="Apple's Ipad Air, Very Light", Price = 400},
                    new Product(){Name ="Ipad Pro", Description="Apple's Ipad Pro, Advanced Tech", Price = 600},
                    new Product(){Name ="Iphone 16 Pro Max", Description="Apple's Latest Iphone, Just the best", Price = 750},
                    new Product(){Name ="Macbook pro", Description="Apple's IMac system, Very Light Fast and runs on the latest M4 chip", Price = 1400},
                    new Product(){Name ="Airpods pro 2", Description="Best for listening to stero music", Price = 70},
                    new Product(){Name ="S24 Ultra", Description="Samsung's Latest phone, Very Fast", Price = 730},
                    new Product(){Name ="Note 9", Description="Samsung's middle range phone", Price = 230},
                    new Product(){Name ="NoteBook Laptop", Description="Samsung's Laptop, Very Sleek", Price = 800},
                    new Product(){Name ="Spark 5", Description="Tecno's Phone, Very Old", Price = 50},
                    new Product(){Name ="Spark 5 pro", Description="Tecno's Phone, Advanced of the base Spark 5", Price = 60},
                    new Product(){Name ="S pen", Description="Samsung's Wireless Pen, Very Light, Sleek and fast", Price = 40}
                };
                _dataContext.AddRange(products);
                _dataContext.SaveChanges();

                var categories = new List<Category>()
                {
                    new Category(){Title = "Apple"},
                    new Category(){Title = "Samsung"},
                    new Category(){Title = "Tecno"},
                    new Category(){Title = "Big Tech"},
                    new Category(){Title = "Medium Tech"},
                    new Category(){Title = "Small Tech"},
                };
                _dataContext.AddRange(categories);
                _dataContext.SaveChanges();

                var productCategories = new List<ProductCategory>()
                {
                    new ProductCategory { Product = products[0], Category = categories[0] },
                    new ProductCategory { Product = products[1], Category = categories[0] },
                    new ProductCategory { Product = products[2], Category = categories[0] },
                    new ProductCategory { Product = products[3], Category = categories[0] },
                    new ProductCategory { Product = products[4], Category = categories[0] },
                    new ProductCategory { Product = products[5], Category = categories[1] },
                    new ProductCategory { Product = products[6], Category = categories[1] },
                    new ProductCategory { Product = products[7], Category = categories[1] },
                    new ProductCategory { Product = products[8], Category = categories[2] },
                    new ProductCategory { Product = products[9], Category = categories[2] },
                    new ProductCategory { Product = products[10], Category = categories[1] },
                    new ProductCategory { Product = products[0], Category = categories[4] },
                    new ProductCategory { Product = products[1], Category = categories[4] },
                    new ProductCategory { Product = products[2], Category = categories[4] },
                    new ProductCategory { Product = products[3], Category = categories[3] },
                    new ProductCategory { Product = products[4], Category = categories[5] },
                    new ProductCategory { Product = products[5], Category = categories[4] },
                    new ProductCategory { Product = products[6], Category = categories[4] },
                    new ProductCategory { Product = products[7], Category = categories[3] },
                    new ProductCategory { Product = products[8], Category = categories[4] },
                    new ProductCategory { Product = products[9], Category = categories[4] },
                    new ProductCategory { Product = products[10], Category = categories[5] }
                 };
                _dataContext.AddRange(productCategories);
                _dataContext.SaveChanges();

            
        }
    }
}
