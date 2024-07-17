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
        public IDataResult<List<Expense>> GetMonthlyExpenses(int year, int month)
        {
            return new SuccessDataResult<List<Expense>>(_expenseDal.GetAll(e => e.UserId == _httpContextAccessor.HttpContext.User.GetUserId() && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month), Messages.DataFetched);
        }

        [SecuredOperation("user")]
        public IDataResult<byte[]> GetMonthlyReport(int year, int month)
        {
            List<Expense> expenses = _expenseDal.GetAll(e => e.UserId == _httpContextAccessor.HttpContext.User.GetUserId() && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month);

            Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Expense Report").FontSize(20).Bold().AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Id");
                                header.Cell().Element(CellStyle).Text("UserId");
                                header.Cell().Element(CellStyle).Text("Amount");
                                header.Cell().Element(CellStyle).Text("Expense Date");
                                header.Cell().Element(CellStyle).Text("Description");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.Bold()).Padding(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var expense in expenses)
                            {
                                table.Cell().Element(CellStyle).Text(expense.Id.ToString());
                                table.Cell().Element(CellStyle).Text(expense.UserId.ToString());
                                table.Cell().Element(CellStyle).Text(expense.Amount.ToString());
                                table.Cell().Element(CellStyle).Text(expense.ExpenseDate.ToShortDateString());
                                table.Cell().Element(CellStyle).Text(expense.Description);

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                                }
                            }
                            var totalAmount = expenses.Sum(e => e.Amount);
                            table.Cell().ColumnSpan(5).Text($"Total Amount: {totalAmount}").Bold().LineHeight(3f);
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"ExpenseReport_{Guid.NewGuid()}.pdf");
                document.GeneratePdf(filePath);
                document.GeneratePdf(stream);
                return new SuccessDataResult<byte[]>(stream.ToArray(), Messages.ReportCreated);
            }
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
    }
}