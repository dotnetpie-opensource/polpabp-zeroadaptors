using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolpAbp.ZeroAdaptors.Emailing.Account
{
    public interface IZeroAdaptorsAccountEmailer
    {
        Task SendEmailActivationLinkAsync(string email);
    }
}
