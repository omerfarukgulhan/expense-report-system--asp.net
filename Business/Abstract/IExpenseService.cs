using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IExpenseService
    {
        IDataResult<List<Expense>> GetAll();
        IDataResult<Expense> GetById(int expenseId);
        IDataResult<List<Expense>> GetMonthlyExpenses(int year, int month);
        IDataResult<byte[]> GetMonthlyReport(int year, int month);
        IResult Add(Expense expense);
        IResult Update(Expense expense);
        IResult Delete(int expenseId);
    }
}
