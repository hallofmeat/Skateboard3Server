using System;
using System.Collections.Generic;
using System.Text;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Notifications.UserSession
{
    public class UserAddedNotification
    {
        [TdfField("DATA")]
        public UserData User { get; set; }
    }

    public class UserData
    {
    }
}
