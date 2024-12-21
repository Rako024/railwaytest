using Expo.Business.DTOs.ControllerDto;
using Expo.Business.Service.Abstract;
using Expo.Core.HelperEntities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExpoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMailService _mailService;
        private const string RecipientEmail = "residbabayev42@gmail.com"; // Statik e-poçt ünvanı.

        public ContactController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] ContactRequestDto contactRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid input data"
                });
            }

            try
            {
                // E-poçt göndərmə üçün `MailRequest` obyekti yaradılır.
                var mailRequest = new MailRequest
                {
                    Subject = contactRequest.Title,
                    ToEmail = RecipientEmail,
                    Body = $@"<!DOCTYPE html><html lang='az'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><title>Əlaqə Forması Mesajı</title><style>body {{font-family: Arial, sans-serif;margin: 0;padding: 0;background-color: #f4f4f9;}}.email-container {{max-width: 600px;margin: 20px auto;background-color: #ffffff;border: 1px solid #e0e0e0;border-radius: 8px;overflow: hidden;box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);}}.email-header {{background-color: #0DA5B5;color: white;padding: 20px;text-align: center;font-size: 24px;font-weight: bold;}}.email-body {{padding: 20px;}}.email-body p {{margin: 10px 0;font-size: 16px;color: #333;}}.email-footer {{background-color: #f4f4f9;text-align: center;padding: 10px;font-size: 14px;color: #777;}}.email-footer a {{color: #0DA5B5;text-decoration: none;}}.info-box {{background-color: #f9f9f9;padding: 15px;border: 1px solid #e0e0e0;border-radius: 4px;margin: 10px 0;}}.info-box p {{margin: 5px 0;}}</style></head><body><div class='email-container'><div class='email-header'>📩 Əlaqə Forması Mesajı</div><div class='email-body'><p><strong>Salam,</strong></p><p>Əlaqə forması vasitəsilə yeni mesaj alınıb. Detallar aşağıda qeyd olunub:</p><div class='info-box'><p><strong>Ad və Soyad:</strong> {contactRequest.FullName}</p><p><strong>E-poçt:</strong> {contactRequest.Email}</p><p><strong>Mesaj:</strong> {contactRequest.Message}</p></div><p>Zəhmət olmasa, bu mesaja tezliklə cavab verin.</p></div><div class='email-footer'><p>Xidmətlərimizdən istifadə etdiyiniz üçün təşəkkür edirik!</p><p><a href='https://yourwebsite.com'>Vebsaytımıza keçid</a></p></div></div></body></html>"


                };

                await _mailService.SendEmailAsync(mailRequest);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while sending the email",
                    Details = ex.Message
                });
            }
        }
    }

   
}
