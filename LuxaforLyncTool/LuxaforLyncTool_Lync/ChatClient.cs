using System;
using Microsoft.Lync.Model;

namespace LuxaforLyncTool_Lync
{
    public class ChatClient
    {
        // Our Lync client
        private LyncClient _lyncClient;
        public ChatClient()
        {
            // Get the current Lync client
            _lyncClient = LyncClient.GetClient();
        }

        public void BindStatusUpdate(EventHandler<ContactInformationChangedEventArgs> handler)
        {
            if (_lyncClient != null)
            {
                // Bind up the event handler for status changes
                _lyncClient.Self.Contact.ContactInformationChanged += handler;
            }
        }
    }
}
