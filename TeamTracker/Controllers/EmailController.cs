using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using TeamTracker.Models;
using TeamTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TeamTracker.ViewModel;  


namespace TeamTracker.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context; // Inject your database context

        public EmailController(IEmailSender emailSender, ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _context = context; // Ensure you have access to your context
        }

        // GET: Display the email form
        [HttpGet]
        public IActionResult SendPersonalEmail(string toEmail)
        {
            ViewBag.ToEmail = toEmail;
            return View();
        }

        // Send Personal Email
        [HttpPost]
        public async Task<IActionResult> SendPersonalEmail(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                return BadRequest("Email, subject, and message cannot be empty.");
            }

            try
            {
                await _emailSender.SendEmailAsync(toEmail, subject, message);

                // Log the sent email
                var sentEmail = new SentEmail
                {
                    RecipientEmail = toEmail,
                    Subject = subject,
                    Body = message,
                    SentDate = DateTime.UtcNow
                };

                _context.SentEmails.Add(sentEmail);
                await _context.SaveChangesAsync(); // Save the sent email

                TempData["SuccessMessage"] = "Personal email sent successfully!";
                return RedirectToAction("SendPersonalEmail"); // Redirect after post
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending email: {ex.Message}");
            }
        }


        // Send Collective Email to all employees
        [HttpPost]
        public async Task<IActionResult> SendCollectiveEmail(string subject, string message)
        {
            try
            {
                var employees = await _context.Employees.ToListAsync(); // Get all employees
                var emailTasks = new List<Task>();

                foreach (var employee in employees)
                {
                    if (!string.IsNullOrEmpty(employee.Email))
                    {
                        emailTasks.Add(_emailSender.SendEmailAsync(employee.Email, subject, message));

                        // Log the sent email
                        var sentEmail = new SentEmail
                        {
                            RecipientEmail = employee.Email,
                            Subject = subject,
                            Body = message,
                            SentDate = DateTime.UtcNow
                        };
                        _context.SentEmails.Add(sentEmail);
                    }
                }

                await Task.WhenAll(emailTasks); // Wait for all emails to be sent
                await _context.SaveChangesAsync(); // Save all sent emails

                TempData["SuccessMessage"] = "Collective email sent to all employees!";
                return RedirectToAction("SendCollectiveEmail"); // Redirect after post
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending emails: {ex.Message}");
            }
        }

        // Send Email to Selected Employees
        [HttpPost]
        public async Task<IActionResult> SendEmailToSelectedEmployees(List<int> employeeIds, string subject, string message)
        {
            try
            {
                var employees = await _context.Employees.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
                var emailTasks = new List<Task>();

                foreach (var employee in employees)
                {
                    if (!string.IsNullOrEmpty(employee.Email))
                    {
                        emailTasks.Add(_emailSender.SendEmailAsync(employee.Email, subject, message));

                        // Log the sent email
                        var sentEmail = new SentEmail
                        {
                            RecipientEmail = employee.Email,
                            Subject = subject,
                            Body = message,
                            SentDate = DateTime.UtcNow
                        };
                        _context.SentEmails.Add(sentEmail);
                    }
                }

                await Task.WhenAll(emailTasks); // Wait for all selected emails to be sent
                await _context.SaveChangesAsync(); // Save all sent emails

                TempData["SuccessMessage"] = "Email sent to selected employees!";
                return RedirectToAction("SendEmailToSelectedEmployeesForm"); // Redirect after post
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending emails: {ex.Message}");
            }
        }


        // Display the form for sending emails to selected employees
        [HttpGet]
        public async Task<IActionResult> SendEmailToSelectedEmployeesForm()
        {
            var employees = await _context.Employees.ToListAsync();
            ViewBag.Employees = employees;
            return View();
        }
        //view to display both sent and received emails
        public IActionResult Inbox()
        {
            var sentEmails = _context.SentEmails.ToList(); // Fetch sent emails

            var inboxViewModel = new InboxView
            {
                SentEmails = sentEmails,
            };

            return View(inboxViewModel);
        }

    }
}
