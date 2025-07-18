# Employee Management System API

This is a secure Employee Management System built using **ASP.NET Core Web API**. It includes features like **JWT-based authentication**, **role-based authorization**, and **menu-level permission mapping**. The project follows the **Repository Pattern** for clean architecture and maintainability.

---

## 🔐 Features

- User Registration & Login
- JWT Token-based Authentication
- Role-Based Access Control (Admin, User, HR, Manager, Intern)
- Menu-Permission Mapping per Role
- View Profile & Self-Update
- Admin CRUD Operations on Employees & Roles
- Uses Entity Framework Core (EF Core) & Code First Migrations

---

## 🏗️ Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Repository Pattern
- AutoMapper
- C#

---

## 📦 Project Structure

/Controllers
  AuthController.cs
  UserController.cs  
  AdminController.cs
  PermissionController.cs

/DTOs
  LoginDTO.cs
  RegisterDTO.cs
  LoginResponseDTO.cs
  UserAccessDTO.cs
  MenuPermissionDTO.cs
  AddPermissionDTO.cs
  AssignRole.cs
  ....
  
/Models
  Employee.cs
  Role.cs
  EmpRole.cs
  RoleMenuPermission.cs

/Interfaces
  IUnitOfWork.cs
  IAuthRepository.cs
  IEmployeRepository.cs
  
/Repositories
  UnitOfWork.cs
  AuthRepository.cs
  EmployeRepository.cs

/Helper
  MappingProfile

/Extensions
  ServiceRegister.cs

## 🚀 How to Run

1. Clone the repo  
   ```bash
   git clone https://github.com/Shruti120809/EmpManage.git
2. Update appsettings.json with your SQL Server connection string.
3. Run the following in Package Manager Console: Update-Database
4. Run the project: dotnet run
