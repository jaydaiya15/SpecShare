using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecShare.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace SpecShare.Pages
{
    public class RegisterModel : PageModel
    {

        private readonly AppDbContext _db;
        public RegisterModel (AppDbContext db) {
            _db = db;
        }

        [BindProperty] public Studentt AddStudent { get; set; } = new();

        public List<DepartmentModel> DepartmentsList { get; set; } = new();


        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;
        [BindProperty] public string EnteredOTP { get; set; } = string.Empty;

        public string ConfirmPasswordError { get; set; } = string.Empty;
        public string OTPMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            DepartmentsList = _db.DepartmentsData.ToList();
        }


        public IActionResult OnPostSendOtp()
        {
            DepartmentsList = _db.DepartmentsData.ToList();

            if (string.IsNullOrWhiteSpace(AddStudent.Email))
            {
                OTPMessage = "Email is required.";
                return Page();
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("EmailOTP", otp);
            HttpContext.Session.SetString("OTPEmail", AddStudent.Email);

            // Prepare the email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SpecShare", "903579001@smtp-brevo.com")); // your Brevo email
            message.To.Add(new MailboxAddress("", AddStudent.Email));
            message.Subject = "SpecShare Email Verification OTP";

            message.Body = new TextPart("plain")
            {
                Text = $"Hello,\n\nYour OTP for SpecShare is: {otp}\n\nDo not share this code with anyone.\n\nThanks,\nSpecShare Team"
            };

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect("smtp-relay.brevo.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("903579001@smtp-brevo.com", "xsmtpsib-04f87d43a80e6715802fbd69f1c191dbcc3529b870778c690e04845bc65e548a-F2O7cH4MVj9waPWk"); // 🔒 use SMTP key
                smtp.Send(message);
                smtp.Disconnect(true);

                OTPMessage = "OTP sent successfully.";
            }
            catch (Exception ex)
            {
                OTPMessage = $"Failed to send OTP: {ex.Message}";
            }

            return Page();
        }



        public IActionResult OnPostVerifyOtp()
        {
            DepartmentsList = _db.DepartmentsData.ToList();

            var correctOtp = HttpContext.Session.GetString("EmailOTP") ?? "";
            var sentToEmail = HttpContext.Session.GetString("OTPEmail") ?? "";

            if (EnteredOTP == correctOtp && AddStudent.Email == sentToEmail)
            {
                TempData["OTPVerified"] = true;
                OTPMessage = "OTP verified successfully.";
            }
            else
            {
                OTPMessage = "Invalid OTP. Please try again.";
            }

            return Page();
        }


        public IActionResult OnPost () {

            DepartmentsList = _db.DepartmentsData.ToList();

            if (AddStudent.Password != ConfirmPassword)
                {
                    ConfirmPasswordError = "Passwords do not match.";
                    return Page();
                }

            AddStudent.Password = BCrypt.Net.BCrypt.HashPassword(AddStudent.Password);

            _db.StudentsData.Add(AddStudent);
            _db.SaveChanges();

             TempData["RegisterSuccess"] = true;

            return Page();
        }
    }
}
