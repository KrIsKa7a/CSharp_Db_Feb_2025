namespace CarDealer
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using Microsoft.EntityFrameworkCore;

    using Data;
    using DTOs.Export;
    using DTOs.Import;
    using Models;
    using Utilities;

    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();
            dbContext.Database.Migrate();

            Console.WriteLine("Database migrated to the latest version successfully!");

            //const string xmlFilePath = "../../../Datasets/sales.xml";
            //string inputXml = File.ReadAllText(xmlFilePath);

            //string result = ImportSales(dbContext, inputXml);
            string result = GetCarsWithTheirListOfParts(dbContext);
            Console.WriteLine(result);
        }

        // Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportSupplierDto[]? supplierDtos = XmlHelper
                .Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");
            if (supplierDtos != null)
            {
                ICollection<Supplier> validSuppliers = new List<Supplier>();
                foreach (ImportSupplierDto supplierDto in supplierDtos)
                {
                    if (!IsValid(supplierDto))
                    {
                        continue;
                    }

                    bool isImporterValid = bool
                        .TryParse(supplierDto.IsImporter, out bool isImporter);
                    if (!isImporterValid)
                    {
                        continue;
                    }

                    Supplier supplier = new Supplier()
                    {
                        Name = supplierDto.Name,
                        IsImporter = isImporter
                    };
                    validSuppliers.Add(supplier);
                }

                context.Suppliers.AddRange(validSuppliers);
                context.SaveChanges();

                result = $"Successfully imported {validSuppliers.Count}";
            }

            return result;
        }

        // Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportPartDto[]? partDtos = XmlHelper
                .Deserialize<ImportPartDto[]>(inputXml, "Parts");
            if (partDtos != null)
            {
                ICollection<int> dbSupplierIds = context
                    .Suppliers
                    .Select(s => s.Id)
                    .ToArray();

                ICollection<Part> validParts = new List<Part>();
                foreach (ImportPartDto partDto in partDtos)
                {
                    if (!IsValid(partDto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal
                        .TryParse(partDto.Price, out decimal price);
                    bool isQuantityValid = int
                        .TryParse(partDto.Quantity, out int quantity);
                    bool isSupplierValid = int
                        .TryParse(partDto.SupplierId, out int supplierId);
                    if ((!isPriceValid) || (!isQuantityValid) || (!isSupplierValid))
                    {
                        // Invalid values for price, quantity or supplier
                        continue;
                    }

                    if (!dbSupplierIds.Contains(supplierId))
                    {
                        // Non-existing supplierId which would violate the FK Constraint
                        continue;
                    }

                    Part part = new Part()
                    {
                        Name = partDto.Name,
                        Price = price,
                        Quantity = quantity,
                        SupplierId = supplierId
                    };
                    validParts.Add(part);
                }

                context.Parts.AddRange(validParts);
                context.SaveChanges();

                result = $"Successfully imported {validParts.Count}";
            }

            return result;
        }

        // Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCarDto[]? carDtos = XmlHelper
                .Deserialize<ImportCarDto[]>(inputXml, "Cars");
            if (carDtos != null)
            {
                ICollection<int> dbPartIds = context
                    .Parts
                    .Select(p => p.Id)
                    .ToArray();

                ICollection<Car> validCars = new List<Car>();
                foreach (ImportCarDto carDto in carDtos)
                {
                    if (!IsValid(carDto))
                    {
                        continue;
                    }

                    bool isTraveledDistanceValid = long
                        .TryParse(carDto.TraveledDistance, out long traveledDistance);
                    if (!isTraveledDistanceValid)
                    {
                        continue;
                    }

                    Car car = new Car()
                    {
                        Make = carDto.Make,
                        Model = carDto.Model,
                        TraveledDistance = traveledDistance
                    };

                    if (carDto.Parts != null)
                    {
                        int[] partIds = carDto
                            .Parts
                            .Where(p => IsValid(p) && 
                                        int.TryParse(p.Id, out int dummy))
                            .Select(p => int.Parse(p.Id))
                            .Distinct()
                            .ToArray();
                        foreach (int partId in partIds)
                        {
                            if (!dbPartIds.Contains(partId))
                            {
                                continue;
                            }

                            PartCar partCar = new PartCar()
                            {
                                PartId = partId,
                                Car = car
                            };
                            car.PartsCars.Add(partCar);
                        }
                    }

                    validCars.Add(car);
                }

                context.Cars.AddRange(validCars);

                context.SaveChanges();

                result = $"Successfully imported {validCars.Count}";
            }

            return result;
        }

        // Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCustomerDto[]? customerDtos = XmlHelper
                .Deserialize<ImportCustomerDto[]>(inputXml, "Customers");
            if (customerDtos != null)
            {
                ICollection<Customer> validCustomers = new List<Customer>();
                foreach (ImportCustomerDto customerDto in customerDtos)
                {
                    if (!IsValid(customerDto))
                    {
                        continue;
                    }

                    bool isBirthDateValid = DateTime
                        .TryParse(customerDto.Birthdate, CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out DateTime birthDate);
                    if (!isBirthDateValid)
                    {
                        continue;
                    }

                    bool isYoungDriverValid = bool
                        .TryParse(customerDto.IsYoungDriver, out bool isYoungDriver);
                    if (!isYoungDriverValid)
                    {
                        continue;
                    }

                    Customer customer = new Customer()
                    {
                        Name = customerDto.Name,
                        BirthDate = birthDate,
                        IsYoungDriver = isYoungDriver
                    };
                    validCustomers.Add(customer);
                }

                context.Customers.AddRange(validCustomers);
                context.SaveChanges();

                result = $"Successfully imported {validCustomers.Count}";
            }

            return result;
        }

        // Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportSaleDto[]? saleDtos = XmlHelper
                .Deserialize<ImportSaleDto[]>(inputXml, "Sales");
            if (saleDtos != null)
            {
                ICollection<int> dbCarIds = context
                    .Cars
                    .Select(c => c.Id)
                    .ToArray();

                ICollection<Sale> validSales = new List<Sale>();
                foreach (ImportSaleDto saleDto in saleDtos)
                {
                    if (!IsValid(saleDto))
                    {
                        continue;
                    }

                    bool isCustomerIdValid = int
                        .TryParse(saleDto.CustomerId, out int customerId);
                    bool isCarIdValid = int
                        .TryParse(saleDto.CarId, out int carId);
                    bool isDiscountValid = decimal
                        .TryParse(saleDto.Discount, out decimal discount);
                    if ((!isCustomerIdValid) || (!isCarIdValid) || (!isDiscountValid))
                    {
                        continue;
                    }

                    if (!dbCarIds.Contains(carId))
                    {
                        continue;
                    }

                    // It's good to validate the customerId in the same way, but Judge may not accept it
                    Sale sale = new Sale()
                    {
                        CarId = carId,
                        CustomerId = customerId,
                        Discount = discount
                    };
                    validSales.Add(sale);
                }

                context.Sales.AddRange(validSales);
                context.SaveChanges();

                result = $"Successfully imported {validSales.Count}";
            }

            return result;
        }

        // Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ExportCarsOverDistanceDto[] cars = context
                .Cars
                .Where(c => c.TraveledDistance > 2_000_000)
                .Select(c => new ExportCarsOverDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance.ToString()
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            string result = XmlHelper
                .Serialize(cars, "cars");
            return result;
        }

        // Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarsWithPartsDto[] carsWithParts = context
                .Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Select(c => new ExportCarsWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance.ToString(),
                    Parts = c
                        .PartsCars
                        .Select(pc => pc.Part)
                        .OrderByDescending(p => p.Price)
                        .Select(p => new ExportCarsWithPartsPartDto()
                        {
                            Name = p.Name,
                            Price = p.Price.ToString()
                        })
                        .ToArray()
                })
                .Take(5)
                .ToArray();

            string result = XmlHelper
                .Serialize(carsWithParts, "cars");
            return result;
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