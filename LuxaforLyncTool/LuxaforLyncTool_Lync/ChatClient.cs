using System;
using Microsoft.Lync.Internal.Utilities.Helpers;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace LuxaforLyncTool_Lync
{
    public class ChatClient
    {
        // Our Lync client
        private LyncClient _lyncClient;

        private EventHandler<MessageSentEventArgs> newMessageHandler;

        public ChatClient()
        {
            // Get the current Lync client
            _lyncClient = LyncClient.GetClient();

            // Default binds
            // When a new convo starts
            _lyncClient.ConversationManager.ConversationAdded += (object sender, ConversationManagerEventArgs args) =>
            {
                // When a person joins
                args.Conversation.ParticipantAdded += (object sender2, ParticipantCollectionChangedEventArgs args2) =>
                {
                    // That's not me
                    if (args2.Participant.Contact.Uri != _lyncClient.Self.Contact.Uri)
                    {
                        // When they send a message
                        InstantMessageModality instantMessageModality = args2.Participant.Modalities[ModalityTypes.InstantMessage] as InstantMessageModality;
                        instantMessageModality.InstantMessageReceived += newMessageHandler;
                    }
                };
            };
        }

        public void BindStatusUpdate(EventHandler<ContactInformationChangedEventArgs> handler)
        {
            if (_lyncClient != null)
            {
                _lyncClient.Self.Contact.ContactInformationChanged += handler;
            }
        }

        public void BindNewConversation(EventHandler<ConversationManagerEventArgs> handler)
        {
            if (_lyncClient != null)
            {
                _lyncClient.ConversationManager.ConversationAdded += handler;
            }
        }

        public void BindNewConversationMessage(EventHandler<MessageSentEventArgs> handler)
        {
            newMessageHandler = handler;
        }

        public ContactAvailability? GetAvailability()
        {
            return (ContactAvailability)_lyncClient.Self.Contact.GetContactInformation(ContactInformationType.Availability);
        }
    }
}