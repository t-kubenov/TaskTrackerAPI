# TaskTrackerAPI
A web API for tracking projects and tasks

# Introduction
Task Tracker API allows users to create, view, edit and delete projects and tasks (further referred to and defined as "Assignments" in the code). 

# Features
* The API allows users to create, view, edit and delete projects and tasks (further referred to and defined as "Assignments" in the code). 
* Each assignment can be designated to a single or no projects in a many-to-one relationship. 
* The user can view all assignments that are related to a project, and the projects can be filtered by many parameters.
* The code has been documented using Swagger.

# Model Description
Projects have the following attributes:
- Id
- Project Name
- Start Date
- Completion Date
- Project Status
- Project Priority

Assignments' attributes are:
- Id
- Name
- Status
- Description
- Priority
- Parent Project Id

# Installation & Running
* Clone the repository
* Run `update-database` through NuGet Package Manager Console
* Run the project using `dotnet run` from `../TaskTrackerAPI/TaskTrackerAPI`
* Connect to the API using Swagger through ports 7061 or 5278

# Technologies Used
* Visual Studio 2022
* ASP.NET Core 6.0

NuGet Packages:
* Microsoft.EntityFrameworkCore 7.0.1
* Microsoft.EntityFrameworkCore.SqlServer 7.0.1
* Microsoft.EntityFrameworkCore.Tools 7.0.1
* Microsoft.VisualStudio.Web.CodeGeneration.Design 6.0.11
* Swashbuckle.AspNetCore 6.4.0

# System configuration
    OS: Windows 10 x64
    CPU: Intel Core i5 6600K
    GPU: NVIDIA RTX 3060
    


