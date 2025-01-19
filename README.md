# RepositoryPatternWithUOW

A demonstration of the **Repository Pattern** with **Unit of Work (UOW)** in a .NET application using **Entity Framework (EF)**. This project showcases a clean and maintainable way to handle data access in your application.

---

## 🛠 Features

- **Repository Pattern**: Encapsulates data access logic for better separation of concerns.
- **Unit of Work (UOW)**: Manages database transactions and ensures atomic operations.
- **Entity Framework**: Provides an ORM layer for seamless database interaction.
- **Generic Repository**: Simplifies CRUD operations with reusable code.
- **Dependency Injection**: Promotes modularity and testability.
- **Best Practices**: Implements clean architecture principles.

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:
- [.NET 6.0+](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- An IDE (e.g., Visual Studio, Rider, or VS Code)

---

### Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-username/RepositoryPatternWithUOW.git
   cd RepositoryPatternWithUOW
   ```

2. **Set Up the Database**:
   - Update the connection string in the `appsettings.json` file:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=YOUR_SERVER;Database=YourDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
       }
     }
     ```

3. **Apply Migrations**:
   Run the following commands to create the database and apply migrations:
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

---

## 🗂 Project Structure

```plaintext
RepositoryPatternWithUOW/
│
├── Controllers/           # API controllers for handling HTTP requests
├── Data/                  # EF Core DbContext and migrations
├── Models/                # Entity models and DTOs
├── Repositories/          # Implementation of generic and specific repositories
├── Services/              # Business logic layer
├── UnitOfWork/            # Unit of Work implementation
├── appsettings.json       # Application configuration file
└── Program.cs             # Entry point of the application
```

---

## 📚 Implementation Details

### 1. **Repository Pattern**

The **Generic Repository** provides a reusable implementation of CRUD operations:
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

Each specific repository (e.g., `ProductRepository`, `CategoryRepository`) inherits from this base interface for additional domain-specific methods.

---

### 2. **Unit of Work (UOW)**

The **Unit of Work** manages database operations to ensure consistency:
```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<int> CompleteAsync();
}
```

The `CompleteAsync` method saves changes to the database as a single transaction.

---

### 3. **Entity Framework Integration**

The project uses **Entity Framework** for seamless database interaction. The `ApplicationDbContext` class extends `DbContext` to define entity sets:
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
```

---

## 🧪 Testing

The project is designed with testability in mind:
- Use dependency injection to mock repositories or services.
- Unit test business logic without depending on EF or the database.

Example using `xUnit`:
```csharp
[Fact]
public async Task GetAllProducts_ShouldReturnProducts()
{
    // Arrange
    var mockRepo = new Mock<IRepository<Product>>();
    mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(MockData.Products);
    var service = new ProductService(mockRepo.Object);

    // Act
    var result = await service.GetAllProductsAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(MockData.Products.Count, result.Count());
}
```

---

## 🤝 Contributing

1. Fork the project.
2. Create your feature branch: `git checkout -b feature/AmazingFeature`.
3. Commit your changes: `git commit -m 'Add AmazingFeature'`.
4. Push to the branch: `git push origin feature/AmazingFeature`.
5. Open a pull request.

---


## 💡 Acknowledgments

Special thanks to the .NET and Entity Framework communities for their invaluable resources and examples.

---

Happy Coding! 😊
