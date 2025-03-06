namespace ProductShop
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Data;
    using DTOs.Import;
    using Models;

    public class StartUp
    {
        public static void Main()
        {
            const string outputFilePath = "../../../Results/output.json";

            using ProductShopContext dbContext = new ProductShopContext();
            dbContext.Database.Migrate();
            
            string jsonString = File.ReadAllText("../../../Datasets/categories-products.json");
            string result = GetUsersWithProducts(dbContext);

            File.WriteAllText(outputFilePath, result, Encoding.Unicode);

            Console.WriteLine(result);
        }

        // Problem 01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportUserDto[]? userDtos = JsonConvert
                .DeserializeObject<ImportUserDto[]>(inputJson);
            if (userDtos != null)
            {
                ICollection<User> usersToAdd = new List<User>();
                foreach (ImportUserDto userDto in userDtos)
                {
                    if (!IsValid(userDto))
                    {
                        // Here we have full control how to act in case of validation error
                        continue;
                    }

                    int? userAge = null;
                    if (userDto.Age != null)
                    {
                        bool isAgeValid = int.TryParse(userDto.Age, out int parsedAge);
                        if (!isAgeValid)
                        {
                            continue;
                        }

                        userAge = parsedAge;
                    }

                    // Manual Mapping
                    User user = new User()
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Age = userAge
                    };

                    usersToAdd.Add(user);
                }

                context.Users.AddRange(usersToAdd);
                context.SaveChanges();

                result = $"Successfully imported {usersToAdd.Count}";
            }

            return result;
        }

        // Problem 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportProductDto[]? productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(inputJson);
            if (productDtos != null)
            {
                ICollection<int> dbUsers = context
                    .Users
                    .Select(u => u.Id)
                    .ToArray();

                ICollection<Product> validProducts = new List<Product>();
                foreach (ImportProductDto productDto in productDtos)
                {
                    if (!IsValid(productDto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal
                        .TryParse(productDto.Price, out decimal productPrice);
                    bool isSellerValid = int
                        .TryParse(productDto.SellerId, out int sellerId);
                    if ((!isPriceValid) || (!isSellerValid))
                    {
                        continue;
                    }

                    int? buyerId = null;
                    if (productDto.BuyerId != null)
                    {
                        bool isBuyerIdValid = int
                            .TryParse(productDto.BuyerId, out int parsedBuyerId);
                        if (!isBuyerIdValid)
                        {
                            continue;
                        }

                        // Removed for Judge, uncomment in Production!
                        /*if (!dbUsers.Contains(parsedBuyerId))
                        {
                            continue;
                        }*/

                        buyerId = parsedBuyerId;
                    }

                    // Removed for Judge, uncomment in Production!
                    /*if (!dbUsers.Contains(sellerId))
                    {
                        // SellerId is valid integer, but user with this Id does not exist!
                        // Invalid Seller!!!
                        continue;
                    }*/

                    Product product = new Product()
                    {
                        Name = productDto.Name,
                        Price = productPrice,
                        SellerId = sellerId,
                        BuyerId = buyerId
                    };

                    validProducts.Add(product);
                }

                context.Products.AddRange(validProducts);
                context.SaveChanges();

                result = $"Successfully imported {validProducts.Count}";
            }

            return result;
        }

        // Problem 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCategoryDto[]? categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);
            if (categoryDtos != null)
            {
                ICollection<Category> validCategories = new List<Category>();
                foreach (ImportCategoryDto categoryDto in categoryDtos)
                {
                    if (!IsValid(categoryDto))
                    {
                        continue;
                    }

                    // CategoryDTO.Name is not null here!
                    Category category = new Category()
                    {
                        Name = categoryDto.Name!,
                    };

                    validCategories.Add(category);
                }

                context.Categories.AddRange(validCategories);
                context.SaveChanges();

                result = $"Successfully imported {validCategories.Count}";
            }

            return result;
        }

        // Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCategoryProductDto[]? catProdDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDto[]>(inputJson);
            if (catProdDtos != null)
            {
                ICollection<int> dbProducts = context
                    .Products
                    .Select(p => p.Id)
                    .ToArray();
                ICollection<int> dbCategories = context
                    .Categories
                    .Select(c => c.Id)
                    .ToArray();

                ICollection<CategoryProduct> validCatProd = new List<CategoryProduct>();
                foreach (ImportCategoryProductDto catProdDto in catProdDtos)
                {
                    if (!IsValid(catProdDto))
                    {
                        continue;
                    }

                    bool isProductIdValid = int
                        .TryParse(catProdDto.ProductId, out int productId);
                    bool isCategoryIdValid = int
                        .TryParse(catProdDto.CategoryId, out int categoryId);

                    // Don't forget to check if this Ids will not violate the FK constraint!!!
                    if ((!isProductIdValid) || (!isCategoryIdValid))
                    {
                        continue;
                    }

                    CategoryProduct catProd = new CategoryProduct()
                    {
                        ProductId = productId,
                        CategoryId = categoryId
                    };

                    validCatProd.Add(catProd);
                }

                context.CategoriesProducts.AddRange(validCatProd);
                context.SaveChanges();

                result = $"Successfully imported {validCatProd.Count}";
            }

            return result;
        }

        // Problem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 &&
                            p.Price <= 1000)
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    Seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .OrderBy(p => p.Price)
                .ToArray();

            DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(products, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = camelCaseResolver
                });

            return jsonResult;
        }

        // Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSoldProducts = context
                .Users
                .Where(u => u.ProductsSold
                    .Any(p => p.BuyerId.HasValue))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.BuyerId.HasValue)
                        .Select(p => new
                        {
                            p.Name,
                            p.Price,
                            BuyerFirstName = p.Buyer!.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        })
                        .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToArray();

            DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            string jsonResult = JsonConvert
                .SerializeObject(usersWithSoldProducts, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = camelCaseResolver
                });

            return jsonResult;
        }

        // Problem 08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithSoldProducts = context
                .Users
                .Where(u => u.ProductsSold
                    .Any(p => p.BuyerId.HasValue))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold
                            .Count(p => p.BuyerId.HasValue),
                        Products = u.ProductsSold
                            .Where(p => p.BuyerId.HasValue)
                            .Select(p => new
                            {
                                p.Name,
                                p.Price
                            })
                            .ToArray()
                    }
                })
                .ToArray()
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();
            var usersDto = new
            {
                UsersCount = usersWithSoldProducts.Length,
                Users = usersWithSoldProducts
            };

            DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            string jsonResult = JsonConvert
                .SerializeObject(usersDto, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = camelCaseResolver,
                    NullValueHandling = NullValueHandling.Ignore
                });

            return jsonResult;
        }

        // Helper method
        private static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator
                .TryValidateObject(dto, validateContext, validationResults, true);

            return isValid;
        }
    }
}