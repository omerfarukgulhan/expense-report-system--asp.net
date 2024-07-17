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

### Example Reports

[View the PDF Documentation](./WebAPI/ExpenseReport_82505912-027f-4204-85d9-0ac8fcb38160.pdf)

[View the PDF Documentation](./WebAPI/ExpenseReport_bf5ba47a-b73f-4ede-8068-10e4b88997bf.pdf)

[View the PDF Documentation](./WebAPI/ExpenseReport_ff539719-1d2f-4183-b242-e7faa8344b81.pdf)

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
