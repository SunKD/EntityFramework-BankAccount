using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace BankAccounts.Models
{
    public class Account : BaseEntity
    {
        public int AccountID { get; set; }
        public float Balance { get; set; }
        public int UserID { get; set; }
        public User User{get; set;}
        public DateTime Created_at { get; set; } = new DateTime(DateTime.UtcNow.Ticks);
        public DateTime Updated_at { get; set; } = new DateTime(DateTime.UtcNow.Ticks);
        
        public List<Transaction> AccountTransactions {get; set;} = new List<Transaction>();
    }
}