using Core.Entities;
using System;

namespace Entities.Concrete
{
    public class Expense : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; }
    }
}
