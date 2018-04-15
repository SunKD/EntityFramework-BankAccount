using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace BankAccounts.Controllers
{
    public class BankController : Controller
    {

        private AccountContext _context;

        public BankController(AccountContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("bank/account/{userID}")]
        public IActionResult Account(int userID)
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            Account userAccount = _context.Accounts.SingleOrDefault(e => e.UserID == userID);
            User currentUser = _context.Users.SingleOrDefault(e => e.UserID == userID);
            var transactions = _context.Transactions.Where(e=>e.AccountID == userAccount.AccountID).OrderBy(e=>e.Created_at).ToList(); 
            transactions.Reverse();
            ViewBag.History = transactions;
            ViewBag.UserName = currentUser.FirstName;
            ViewBag.Balance = userAccount.Balance;
            ViewBag.UserId = currentUser.UserID;
            return View("Account");
        }

        [HttpPost]
        [Route("bank/account/process")]
        public IActionResult Process(float amount)
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            int userID = (int)HttpContext.Session.GetInt32("CurrentUserID");
            Account userAccount = _context.Accounts.SingleOrDefault(e => e.UserID == userID);

            if (amount < 0 && userAccount.Balance < Math.Abs(amount) )
            {
                TempData["Error"] = "The Balance is insufficient.";
                return RedirectToAction("Account", "Bank", new{userID = userID});
            }
            else
            {
                Transaction newTransaction = new Transaction
                {
                    Amount = amount,
                    AccountID = userAccount.AccountID
                };

                userAccount.Balance += amount;
                _context.Transactions.Add(newTransaction);
                _context.SaveChanges();
            }

            return RedirectToAction("Account", "Bank", new{userID = userID});
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool CheckLoggedIn()
        {
            if (HttpContext.Session.GetInt32("CurrentUserID") == null)
            {
                return false;
            }
            return true;
        }
    }
}
