# Repository Pattern with Unit of Work (UOW) in ASP.NET Core

This document serves as a comprehensive reference for implementing the **Repository Pattern** combined with **Unit of Work (UOW)** in an ASP.NET Core application. The pattern is designed to enhance code reusability, maintainability, and testability by separating the business logic from data access logic.

---

## Why Use the Repository Pattern with UOW?

### **Repository Pattern**

- **Abstracts data access:** Reduces direct dependency on Entity Framework or other ORMs.
- **Encapsulates query logic:** Keeps data retrieval logic in one place.
- **Improves testability:** Makes it easier to mock repositories for unit testing.

### **Unit of Work (UOW)**

- **Manages transactions:** Groups multiple operations into a single transaction.
- **Reduces database calls:** Optimizes performance by minimizing round trips to the database.
- **Centralizes data management:** Ensures that changes to multiple repositories are coordinated.

---

## Folder Structure

```
ProjectRoot
|-- Data
|   |-- Entities
|   |   |-- BaseEntity.cs
|   |   |-- Product.cs
|   |   |-- Order.cs
|   |-- Context
|   |   |-- ApplicationDbContext.cs
|-- Repositories
|   |-- Interfaces
|   |   |-- IRepository.cs
|   |   |-- IProductRepository.cs
|   |-- Implementations
|       |-- Repository.cs
|       |-- ProductRepository.cs
|-- UnitOfWork
|   |-- IUnitOfWork.cs
|   |-- UnitOfWork.cs
|-- Services
|-- Controllers
|-- Startup.cs
```

---

## Implementation Steps

### 1. Define the Base Entity

```csharp
namespace ProjectNamespace.Data.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
```

### 2. Create Entity Classes

```csharp
namespace ProjectNamespace.Data.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
    }
}
```

### 3. Configure the ApplicationDbContext

```csharp
using Microsoft.EntityFrameworkCore;
using ProjectNamespace.Data.Entities;

namespace ProjectNamespace.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations
        }
    }
}
```

### 4. Define the Generic Repository Interface

```csharp
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectNamespace.Repositories.Interfaces
{
    public interface sIRepository<T> where T : clas
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
```

### 5. Implement the Generic Repository

```csharp
using Microsoft.EntityFrameworkCore;
using ProjectNamespace.Data.Context;
using ProjectNamespace.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectNamespace.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);
    }
}
```

### 6. Create Specific Repository Interfaces

```csharp
using ProjectNamespace.Data.Entities;

namespace ProjectNamespace.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetProductByNameAsync(string name);
    }
}
```

### 7. Implement Specific Repositories

```csharp
using Microsoft.EntityFrameworkCore;
using ProjectNamespace.Data.Context;
using ProjectNamespace.Data.Entities;
using ProjectNamespace.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ProjectNamespace.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
```

### 8. Define the Unit of Work Interface

```csharp
using System;
using System.Threading.Tasks;

namespace ProjectNamespace.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        Task<int> CompleteAsync();
    }
}
```

### 9. Implement the Unit of Work

```csharp
using ProjectNamespace.Data.Context;
using ProjectNamespace.Repositories.Interfaces;
using ProjectNamespace.Repositories.Implementations;
using System.Threading.Tasks;

namespace ProjectNamespace.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
        }

        public IProductRepository Products { get; private set; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
```

### 10. Register Services in `Startup.cs`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    services.AddControllersWithViews();
}
```

---

## Usage Example

### Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using ProjectNamespace.UnitOfWork;
using System.Threading.Tasks;

namespace ProjectNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction("GetProducts", new { id = product.Id }, product);
        }
    }
}
```

---

## Benefits

- Clean separation of concerns.
- Centralized transaction management.
- Easily extensible and testable design.

---

## Future Improvements

- Add caching mechanisms.
- Implement pagination for large datasets.
- Use specifications for complex queries.

---

## Resources

- [Microsoft Documentation on EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

