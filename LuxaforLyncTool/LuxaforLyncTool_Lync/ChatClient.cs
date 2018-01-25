using System;
using Microsoft.Lync.Model;

namespace LuxaforLyncTool_Lync
{
    public class ChatClient
    {
        // Our Lync client
        private LyncClient lyncClient;

        // Out event handler for lycn status updates
        public EventHandler<ContactInformationChangedEventArgs> StatusChangedHandler{ get; set; }

        public ChatClient()
        {
            // Get the current Lync client
            LyncClient lyncClient = LyncClient.GetClient();
            if (lyncClient != null)
            {
                // Bind up the event handler for status changes
                lyncClient.Self.Contact.ContactInformationChanged += StatusChangedHandler;
            }
            

            Contact self = sender as Contact;

            if (eventArgs.ChangedContactInformation.Contains(ContactInformationType.Availability))
            {
                ContactAvailability availability = (ContactAvailability)self.GetContactInformation(ContactInformationType.Availability);

                char green = 'G';
                char targetColor;

                switch (availability)
                {
                    case ContactAvailability.Free:
                        targetColor
                }

            }
        }
    }
}
