﻿using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _expenseService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{expenseId}")]
        public IActionResult Get(int expenseId)
        {
            var result = _expenseService.GetById(expenseId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{year}/{month}")]
        public IActionResult Get(int year, int month)
        {
            var result = _expenseService.GetMonthlyExpenses(year, month);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("report/{year}/{month}")]
        public IActionResult GetMonthlyReport(int year, int month)
        {
            var result = _expenseService.GetMonthlyReport(year, month);
            if (result.Success)
            {
                return File(result.Data, "application/pdf", $"Monthly Expense Report-{month}-{year}.pdf");
            }
            return BadRequest(result);
        }

        [HttpPost]
        public IActionResult Add(Expense expense)
        {
            var result = _expenseService.Add(expense);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{epxenseId}")]
        public IActionResult Delete(int epxenseId)
        {
            var result = _expenseService.Delete(epxenseId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut]
        public IActionResult Update(Expense expense)
        {
            var result = _expenseService.Update(expense);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
