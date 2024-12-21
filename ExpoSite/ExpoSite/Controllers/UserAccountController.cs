using Expo.Business.DTOs.UserDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Core.HelperEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ExpoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserAccountController> _logger;
        private readonly IMailService _mailService;

        // private readonly RoleManager<IdentityRole> _roleManager;

        public UserAccountController(/*RoleManager<IdentityRole> roleManager, */IUserService userService, UserManager<AppUser> userManager, ILogger<UserAccountController> logger, IMailService mailService)
        {
            //  _roleManager = roleManager;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _mailService = mailService;
        }

        //[HttpGet("create-roles")]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    var roles = new[] { "Admin", "Customer" };

        //    foreach (var role in roles)
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            var result = await _roleManager.CreateAsync(new IdentityRole(role));
        //            if (!result.Succeeded)
        //            {
        //                return BadRequest(new { message = $"Failed to create role: {role}" });
        //            }
        //        }
        //    }

        //    return Ok(new { message = "Roles created successfully!" });
        //}





        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid input data"
                });
            }
            var user1 = await _userManager.FindByEmailAsync(userDto.Email);
            if (user1 is not null)
            {
                throw new GlobalAppException("Email is already in use.");
            }
            try
            {
                var result = await _userService.RegisterAsync(userDto);

                var user = await _userManager.FindByEmailAsync(userDto.Email);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "User registration failed."
                    });
                }

                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string link = Url.Action("ConfirmEmail", "UserAccount", new { userId = user.Id, token = token }, HttpContext.Request.Scheme);
                if(link == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Link not found"
                    });
                }
                await _mailService.SendEmailAsync(new MailRequest
                {
                    Subject = "Confirm Email",
                    ToEmail = user.Email,
                    //Body = $"<a href = '{link}'>Confirm Email</a>"
                    Body = $"\r\n    <!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional //EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n\r\n<head>\r\n  <!--[if gte mso 9]>\r\n<xml>\r\n  <o:OfficeDocumentSettings>\r\n    <o:AllowPNG/>\r\n    <o:PixelsPerInch>96</o:PixelsPerInch>\r\n  </o:OfficeDocumentSettings>\r\n</xml>\r\n<![endif]-->\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n  <meta name=\"x-apple-disable-message-reformatting\">\r\n  <!--[if !mso]><!-->\r\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n  <!--<![endif]-->\r\n  <title></title>\r\n\r\n  <style type=\"text/css\">\r\n    @media only screen and (min-width: 620px) {{\r\n      .u-row {{\r\n        width: 600px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        vertical-align: top;\r\n      }}\r\n      .u-row .u-col-100 {{\r\n        width: 600px !important;\r\n      }}\r\n    }}\r\n    \r\n    @media (max-width: 620px) {{\r\n      .u-row-container {{\r\n        max-width: 100% !important;\r\n        padding-left: 0px !important;\r\n        padding-right: 0px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        min-width: 320px !important;\r\n        max-width: 100% !important;\r\n        display: block !important;\r\n      }}\r\n      .u-row {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col>div {{\r\n        margin: 0 auto;\r\n      }}\r\n    }}\r\n    \r\n    body {{\r\n      margin: 0;\r\n      padding: 0;\r\n    }}\r\n    \r\n    table,\r\n    tr,\r\n    td {{\r\n      vertical-align: top;\r\n      border-collapse: collapse;\r\n    }}\r\n    \r\n    p {{\r\n      margin: 0;\r\n    }}\r\n    \r\n    .ie-container table,\r\n    .mso-container table {{\r\n      table-layout: fixed;\r\n    }}\r\n    \r\n    * {{\r\n      line-height: inherit;\r\n    }}\r\n    \r\n    a[x-apple-data-detectors='true'] {{\r\n      color: inherit !important;\r\n      text-decoration: none !important;\r\n    }}\r\n    \r\n    table,\r\n    td {{\r\n      color: #000000;\r\n    }}\r\n    \r\n    #u_body a {{\r\n      color: #0000ee;\r\n      text-decoration: underline;\r\n    }}\r\n  </style>\r\n\r\n\r\n\r\n  <!--[if !mso]><!-->\r\n  <link href=\"https://fonts.googleapis.com/css?family=Cabin:400,700\" rel=\"stylesheet\" type=\"text/css\">\r\n  <!--<![endif]-->\r\n\r\n</head>\r\n\r\n<body className=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000\">\r\n  <!--[if IE]><div className=\"ie-container\"><![endif]-->\r\n  <!--[if mso]><div className=\"mso-container\"><![endif]-->\r\n  <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%\" cellpadding=\"0\" cellspacing=\"0\">\r\n    <tbody>\r\n      <tr style=\"vertical-align: top\">\r\n        <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\r\n          <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #f9f9f9;\"><![endif]-->\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; color: #afb0c7; line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 14px; line-height: 23.8px;\">View Email in Browser</span></p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #003399;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #003399;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:40px 10px 10px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\r\n                                <tr>\r\n                                  <td style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">\r\n\r\n                                    <img align=\"center\" border=\"0\" src=\"https://cdn.templates.unlayer.com/assets/1597218650916-xxxxc.png\" alt=\"Image\" title=\"Image\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 26%;max-width: 150.8px;\"\r\n                                      width=\"150.8\" />\r\n\r\n                                  </td>\r\n                                </tr>\r\n                              </table>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; color: #e5eaf5; line-height: 140%; text-align: center; word-wrap: break-word;\">\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:0px 10px 31px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; color: #e5eaf5; line-height: 140%; text-align: center; word-wrap: break-word;\">\r\n                                <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 28px; line-height: 39.2px;\"><strong><span style=\"line-height: 39.2px; font-size: 28px;\">\r\n                                </span></strong>\r\n                                  </span>\r\n                                </p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:33px 55px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; line-height: 160%; text-align: center; word-wrap: break-word;\">\r\n                                <p style=\"font-size: 14px; line-height: 160%;\"><span style=\"font-size: 18px; line-height: 28.8px;\"><a href='{link}'>Confirm Email</a> </span></p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <!--[if mso]><style>.v-button {{background: transparent !important;}}</style><![endif]-->\r\n                              <div align=\"center\">\r\n                                <!--[if mso]><v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"\" style=\"height:46px; v-text-anchor:middle; width:234px;\" arcsize=\"8.5%\"  stroke=\"f\" fillcolor=\"#ff6600\"><w:anchorlock/><center style=\"color:#FFFFFF;\"><![endif]-->\r\n                                <!--[if mso]></center></v:roundrect><![endif]-->\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:33px 55px 60px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; line-height: 160%; text-align: center; word-wrap: break-word;\">\r\n                                <p style=\"line-height: 160%; font-size: 14px;\"><span style=\"font-size: 18px; line-height: 28.8px;\">Thanks,</span></p>\r\n                                <p style=\"line-height: 160%; font-size: 14px;\"><span style=\"font-size: 18px; line-height: 28.8px;\">{user.FullName}</span></p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #e5eaf5;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #e5eaf5;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:41px 55px 18px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; color: #003399; line-height: 160%; text-align: center; word-wrap: break-word;\">\r\n                                <p style=\"font-size: 14px; line-height: 160%;\"><span style=\"font-size: 16px; line-height: 25.6px; color: #000000;\">{user.Email}</span></p>\r\n                              </div>\r\n                              <div style=\"font-size: 14px; color: #003399; line-height: 160%; text-align: center; word-wrap: break-word;\">\r\n         \r\n                            </div>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 10px 33px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div align=\"center\">\r\n                                <div style=\"display: table; max-width:244px;\">\r\n                                  <!--[if (mso)|(IE)]><table width=\"244\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"border-collapse:collapse;\" align=\"center\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:244px;\"><tr><![endif]-->\r\n\r\n\r\n                                  <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 17px;\" valign=\"top\"><![endif]-->\r\n                           \r\n                                  <!--[if (mso)|(IE)]></td><![endif]-->\r\n\r\n                                  <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 17px;\" valign=\"top\"><![endif]-->\r\n                              \r\n                                  <!--[if (mso)|(IE)]></td><![endif]-->\r\n\r\n                                  <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 17px;\" valign=\"top\"><![endif]-->\r\n                            \r\n                                  <!--[if (mso)|(IE)]></td><![endif]-->\r\n\r\n                                  <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 17px;\" valign=\"top\"><![endif]-->\r\n                        \r\n                                  <!--[if (mso)|(IE)]></td><![endif]-->\r\n\r\n                                  <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 0px;\" valign=\"top\"><![endif]-->\r\n                            \r\n                                  <!--[if (mso)|(IE)]></td><![endif]-->\r\n\r\n\r\n                                  <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                                </div>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n\r\n\r\n          <div className=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n            <div className=\"u-row\" style=\"margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;\">\r\n              <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #003399;\"><![endif]-->\r\n\r\n                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                <div className=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                  <div style=\"height: 100%;width: 100% !important;\">\r\n                    <!--[if (!mso)&(!IE)]><!-->\r\n                    <div style=\"box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                      <!--<![endif]-->\r\n\r\n                      <table style=\"font-family:'Cabin',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Cabin',sans-serif;\" align=\"left\">\r\n\r\n                              <div style=\"font-size: 14px; color: #fafafa; line-height: 180%; text-align: center; word-wrap: break-word;\">\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n\r\n                      <!--[if (!mso)&(!IE)]><!-->\r\n                    </div>\r\n                    <!--<![endif]-->\r\n                  </div>\r\n                </div>\r\n                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n              </div>\r\n            </div>\r\n          </div>\r\n\r\n\r\n\r\n          <!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n        </td>\r\n      </tr>\r\n    </tbody>\r\n  </table>\r\n  <!--[if mso]></div><![endif]-->\r\n  <!--[if IE]></div><![endif]-->\r\n</body>\r\n\r\n</html>\r\n"

                });

                return Ok( new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User registered successfully!"
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            _logger.LogInformation($"ConfirmEmail called with token: {token} and userId: {userId}");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Invalid token or user ID.");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid token or user ID."
                });
            }



            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with userId: {userId}");
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid token or user ID."
                    });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Email confirmed successfully for userId: {userId}");
                    return Redirect("https://www.google.com"); // buramı domeinde dəyiş
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Email confirmation failed for userId: {userId}, errors: {errors}");
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = $"Email confirmation failed: {errors}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming the email");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {


                // Login metodunu çağırın
                var loginResponse = await _userService.LoginAsync(loginRequest);

                if (loginResponse == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" }); // Parol və ya email səhvdir
                }

                return Ok( new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = loginResponse
                }); 
            } catch (GlobalAppException ex) 
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
            
        }
        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequestDto loginRequest)
        {
            try
            {


                // Login metodunu çağırın
                var loginResponse = await _userService.AdminLoginAsync(loginRequest);

                if (loginResponse == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" }); // Parol və ya email səhvdir
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = loginResponse
                });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }

        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                await _userService.SendForgotPasswordEmailAsync(forgotPasswordDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Password reset email sent successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new {StatusCode = StatusCodes.Status400BadRequest ,Message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                //var decodedToken = System.Web.HttpUtility.UrlDecode(resetPasswordDto.Token);
                //resetPasswordDto.Token = decodedToken;
                //string decodedEmail = HttpUtility.UrlDecode(resetPasswordDto.Email);
                //resetPasswordDto.Email = decodedEmail;
                var result = await _userService.ResetPasswordAsync(resetPasswordDto);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = errors });
                }
                
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Password reset successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
        }

        [HttpGet("get-user-details")]
        [Authorize] // Yalnız giriş etmiş istifadəçilər üçün
        public async Task<IActionResult> GetUserDetails()
        {
            try
            {
                // İstifadəçinin ID-sini Claims vasitəsilə əldə et
                var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(appUserId))
                {
                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "User not authenticated."
                    });
                }

                // İstifadəçinin məlumatlarını gətir
                var user = await _userManager.FindByIdAsync(appUserId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found."
                    });
                }

                // İstifadəçinin məlumatlarını JSON formatında qaytar
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.FullName,
                        user.CompanyName,
                       address =  user.Adress,
                        user.PhoneNumber,
                        user.EmailConfirmed,
                        user.PhoneNumberConfirmed,
                        user.LockoutEnabled,
                        user.AccessFailedCount
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user details.");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto request)
        {
            try
            {
                await _userService.UpdateUserAsync(
                    request.FullName,
                    request.CompanyName,
                    request.Address,
                    request.PhoneNumber
                );

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User updated successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "Error updating user.");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

    }
}
