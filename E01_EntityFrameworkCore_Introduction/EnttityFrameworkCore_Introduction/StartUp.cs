namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;

    using System.Text;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using SoftUniContext dbContext = new SoftUniContext();
            dbContext.Database.EnsureCreated();

            string result = DeleteProjectById(dbContext);
            Console.WriteLine(result);
        }

        // Problem 03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray(); // Materialize the query -> The query is detached from DB
                            // The data is now in-memory -> In the memory of our C# Program

            // Hint: It is possible to attach to the DB at later point
            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesRnD = context
                .Employees
                .Where(e => e.Department.Name.Equals("Research and Development"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employeesRnD)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} from Research and Development - ${e.Salary.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            const string newAddressText = "Vitoshka 15"; // Just a good practice to avoid magic strings and numbers
            const int newAddressTownId = 4;

            Address newAddress = new Address()
            {
                AddressText = newAddressText,
                TownId = newAddressTownId
            };

            // 2 ways for adding the new address
            // I. Explictly
            //  1. First add the new address to addresses
            //  2. Attach the new address to employee
            //  3. SaveChanges
            // II. Implicitly (using nested add)
            //  1. Attach the new address to employee
            //  2. SaveChanges

            //context.Addresses.Add(newAddress); // 1 from Explicit

            Employee nakovEmployee = context
                .Employees
                .First(e => e.LastName.Equals("Nakov"));
            nakovEmployee.Address = newAddress; // 2 from Explicit/1 from Implicit

            context.SaveChanges(); // 3 from Explicit/2 from Implicit

            string?[] addresses = context
                .Employees
                .Where(e => e.AddressId.HasValue)
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address!.AddressText)
                .Take(10)
                .ToArray();

            return String.Join(Environment.NewLine, addresses);
        }

        // Problem 07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesWithProjects = context
                .Employees
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    ManagerFirstName = e.Manager == null ? 
                        null : e.Manager.FirstName,
                    ManagerLastName = e.Manager == null ? 
                        null : e.Manager.LastName,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project)
                        .Where(p => p.StartDate.Year >= 2001 &&
                                    p.StartDate.Year <= 2003)
                        .Select(p => new
                        {
                            ProjectName = p.Name,
                            p.StartDate,
                            p.EndDate
                        })
                        .ToArray()
                })
                .Take(10)
                .ToArray();

            foreach (var e in employeesWithProjects)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {
                    string startDateFormatted = p.StartDate
                        .ToString("M/d/yyyy h:mm:ss tt");
                    string endDateFormatted = p.EndDate.HasValue ?
                        p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";
                    sb
                        .AppendLine($"--{p.ProjectName} - {startDateFormatted} - {endDateFormatted}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            const int deleteProjectId = 2; // Good practice

            IEnumerable<EmployeeProject> employeeProjectsDelete = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == deleteProjectId)
                .ToArray();
            context.EmployeesProjects.RemoveRange(employeeProjectsDelete);

            Project? deleteProject = context
                .Projects
                .Find(deleteProjectId);
            if (deleteProject != null)
            {
                context.Projects.Remove(deleteProject);
            }

            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Select(p => p.Name)
                .Take(10)
                .ToArray();

            return String.Join(Environment.NewLine, projectNames);
        }

        private static void RestoreDatabase(SoftUniContext context)
        {
            // This will NOT insert the data in the DB
            // This would work if we were working Code-First approach
            // Thi is not optimal for DB-First
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
