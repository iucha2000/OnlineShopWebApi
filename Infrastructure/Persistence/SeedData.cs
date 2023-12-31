﻿using Domain;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void SeedProducts(ApplicationDbContext dbContext)
        {
            var productDb = dbContext.Set<Product>();

            if (!productDb.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    var product1 = new Product
                    {
                        Category = ProductCategory.Grocery,
                        Name = "Bread",
                        Price = 1.99m,
                        ProductId = ProductIdentifier.Bread
                    };
                    productDb.Add(product1);
                }

                for (int i = 0; i < 10; i++)
                {
                    var product2 = new Product
                    {
                        Category = ProductCategory.Electronics,
                        Name = "Tablet",
                        Price = 259.99m,
                        ProductId = ProductIdentifier.Tablet
                    };
                    productDb.Add(product2);
                }

                for (int i = 0; i < 15; i++)
                {
                    var product3 = new Product
                    {
                        Category = ProductCategory.Fashion,
                        Name = "Sweater",
                        Price = 49.99m,
                        ProductId = ProductIdentifier.Sweater
                    };
                    productDb.Add(product3);
                }

                for (int i = 0; i < 20; i++)
                {
                    var product4 = new Product
                    {
                        Category = ProductCategory.Home,
                        Name = "Lamp",
                        Price = 19.99m,
                        ProductId = ProductIdentifier.Lamp
                    };
                    productDb.Add(product4);
                }

                dbContext.SaveChanges();
            }
        }
    }
}
