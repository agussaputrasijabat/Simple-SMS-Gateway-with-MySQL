using SMSGateway.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMSGateway
{
    public interface ISMSSender
    {
        System.Data.ConnectionState Reconnect();
        SMSModel Get(int Id = 0);
        List<SMSModel> SentList();
        List<SMSModel> PendingList();
    }
}
