# expense-report-system--asp.net

## Project Description

This Expense Report System is an api built with ASP.NET, designed to help users manage their expenses. The system includes features such as authentication, CRUD operations for expenses, and generating expense reports as PDFs.

## Features

- **Authentication**

  - Secure registration and login for users.
  - Password encryption and user session management.

- **Expense Management**

  - Add, view, edit, and delete expenses.

- **PDF Report Generation**
  - Generate detailed expense reports in PDF format.

## Technologies Used

- **Backend:**

  - ASP.NET Core
  - Entity Framework Core
  - Autofac

- **Database:**

  - SQL Server Express

- **PDF Generation:**

  - QuestPDF

## Installation

To run this project locally, follow these steps:

1. **Clone the repository:**

   ```bash
   git clone https://github.com/omerfarukgulhan/expense-report-system--asp.net
   ```

   ```
   cd expense-report-system
   ```

   ```
   dotnet restore
   dotnet ef database update
   ```

   ```
   dotnet run
   ```
