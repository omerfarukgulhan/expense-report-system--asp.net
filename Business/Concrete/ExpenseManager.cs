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
using System;
using System.Collections.Generic;

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
    }
}
