using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IExpenseService
    {
        IDataResult<List<Expense>> GetAll();
        IDataResult<Expense> GetById(int expenseId);
        IResult Add(Expense expense);
        IResult Update(Expense expense);
        IResult Delete(int expenseId);
        IDataResult<byte[]> GetMonthlyReport(int year, int month);
    }
}
