using Expo.Core.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
