using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Business;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Business.Concrete
{
    public class ExpenseManager : IExpenseService
    {
        private IExpenseDal _expenseDal;
        private IHttpContextAccessor _httpContextAccessor;

        public ExpenseManager(IExpenseDal expenseDal)
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            _expenseDal = expenseDal;
        }

        [SecuredOperation("user")]
        public IResult Add(Expense expense)
        {
            _expenseDal.Add(expense);
            return new SuccessResult(Messages.DataAdded);
        }

        [SecuredOperation("user")]
        public IResult Delete(int expenseId)
        {
            Expense expense = _expenseDal.Get(e => e.Id == expenseId);

            IResult result = BusinessRules.Run(CheckUser(expense.UserId));
            if (result != null)
            {
                return result;
            }

            _expenseDal.Delete(expense);
            return new SuccessResult(Messages.DataDeleted);
        }

        [SecuredOperation("user")]
        public IDataResult<List<Expense>> GetAll()
        {
            return new SuccessDataResult<List<Expense>>(_expenseDal.GetAll(e => e.UserId == _httpContextAccessor.HttpContext.User.GetUserId()), Messages.DataFetched);
        }

        [SecuredOperation("user")]
        public IDataResult<Expense> GetById(int expenseId)
        {
            return new SuccessDataResult<Expense>(_expenseDal.Get(e => e.Id == expenseId && e.UserId == _httpContextAccessor.HttpContext.User.GetUserId()), Messages.DataFetched);
        }




        [SecuredOperation("user")]
        public IResult Update(Expense expense)
        {
            IResult result = BusinessRules.Run(CheckUser(expense.UserId));
            if (result != null)
            {
                return result;
            }

            _expenseDal.Update(expense);
            return new SuccessResult(Messages.DataUpdated);
        }

        private IResult CheckUser(int userId)
        {
            if (_httpContextAccessor.HttpContext.User.GetUserId() != userId)
            {
                return new ErrorResult(Messages.UserMismatch);
            }
            return new SuccessResult();
        }

        public IDataResult<byte[]> GetMonthlyReport(int year, int month)
        {
            List<Expense> expenseList = _expenseDal.GetAll(e => e.UserId == _httpContextAccessor.HttpContext.User.GetUserId() && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month);
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text($"Monthly Expenses Report - {month}")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .AddTable(table =>
                        {
                            table.AddColumn("Date", col => col.Alignment = HorizontalAlignment.Left);
                            table.AddColumn("Amount", col => col.Alignment = HorizontalAlignment.Right);
                            table.AddColumn("Description", col => col.Alignment = HorizontalAlignment.Left);

                            foreach (var expense in expenseList)
                            {
                                table.AddRow(
                                    expense.ExpenseDate.ToString("yyyy-MM-dd"),
                                    $"{expense.Amount:C}",
                                    expense.Description
                                );
                            }
                        })
                        .AddParagraph(builder =>
                        {
                            builder.Text($"Total Expenses: {expenseList.Sum(e => e.Amount):C}")
                                   .Alignment(HorizontalAlignment.Right);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return new SuccessDataResult<byte[]>(stream.ToArray(), "Monthly Expenses PDF generated successfully.");
            }
        }
    }
}


/*
 
 
  public static byte[] GenerateMonthlyExpensesPdf(List<Expense> expenseList, string month)
    {
        return new PDFDocument()
            .AddSection()
                .Header(Header)
                .Footer(Footer)
                .AddParagraph($"Monthly Expenses Report - {month}")
                .AddParagraph($"Generated on: {DateTime.Now}")
                .AddTable()
                    .AddColumn("Date", col => col.Alignment = HorizontalAlignment.Left)
                    .AddColumn("Amount", col => col.Alignment = HorizontalAlignment.Right)
                    .AddColumn("Description", col => col.Alignment = HorizontalAlignment.Left)
                    .ForEach(expenseList, (table, expense) =>
                    {
                        table.AddRow(
                            expense.ExpenseDate.ToString("yyyy-MM-dd"),
                            $"{expense.Amount:C}",
                            expense.Description
                        );
                    })
                .Parent()
                .AddParagraph()
                    .Text($"Total Expenses: {expenseList.Sum(e => e.Amount):C}")
                    .Alignment(HorizontalAlignment.Right)
            .GenerateDocument();
    }

    private static void Header(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Stack(stack =>
            {
                stack.Element().Text("Monthly Expenses Report").FontSize(15).Bold();
                stack.Element().Text($"Generated on: {DateTime.Now.ToShortDateString()}");
            }).Padding(10);
        });
    }

    private static void Footer(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Stack(stack =>
            {
                stack.Element().Text("Page {pageNumber} of {totalPages}").Alignment(HorizontalAlignment.Right);
            }).Padding(10);
        });
    }
 */
