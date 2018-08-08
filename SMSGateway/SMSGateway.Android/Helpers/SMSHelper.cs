using System;
using System.Collections.Generic;
using System.Data;

using SMSGateway.Droid.Helpers;
using SMSGateway.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSHelper))]
namespace SMSGateway.Droid.Helpers
{
    public class SMSHelper : ISMSSender
    {
        public SMSModel Get(int Id = 0)
        {
            return Services.SMSService.Get(Id);
        }

        public List<SMSModel> PendingList()
        {
            try
            {
                return Services.SMSService.PendingList();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
            return null;
        }

        public ConnectionState Reconnect()
        {
            return Services.SMSService.Reconnect();
        }

        public List<SMSModel> SentList()
        {
            try
            {
                return Services.SMSService.SentList();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
            return null;
        }
    }
}