using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LuxaforLyncTool_Lync.Helpers;
using Microsoft.Lync.Internal.Utilities.Helpers;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace LuxaforLyncTool_Lync
{
    public class ChatClient
    {
        // Our Lync client
        private static LyncClient _lyncClient;

        public bool IsSignedIn => _lyncClient?.Self?.Contact != null;

        // The new message event handler. It is not initially known but must be bound to all initial conversations on process
        //  start, which is why it's defined here early
        private EventHandler<MessageSentEventArgs> newMessageHandler;

        private int reconnectionMilliseconds;

        /// <summary>
        /// Constructor
        /// </summary>
        public ChatClient(int defaultReconnectionMilliseconds)
        {
            this.reconnectionMilliseconds = defaultReconnectionMilliseconds;

            // Get the current Lync client
            ConnectToLync();
        }

        public Action DisconnectedAction { get; set; }
        public Action ConnnectedAction { get; set; }

        private ConnectionStatus LastKnownStatus { get; set; } = ConnectionStatus.Connected;

        private void ClientStateChangedEvent(object sender, ClientStateChangedEventArgs args)
        {
            if (args.OldState == ClientState.SignedIn && args.NewState != ClientState.SignedIn)
            {
                DisconnectedAction?.Invoke();
            }
            if (args.OldState != ClientState.SignedIn && args.NewState == ClientState.SignedIn)
            {
                ConnnectedAction?.Invoke();
            }
        }

        private void ConversationAddedHandler(object sender, ConversationManagerEventArgs args)
        {
            BindHandlerToConversationIMs(args.Conversation);
        }

        /// <summary>
        /// Try connect to the current lync instance
        /// </summary>
        private void ConnectToLync()
        {
            // Connect to Lync
            try
            {
                _lyncClient = LyncClient.GetClient();

                if (this.IsSignedIn)
                {
                    ConnnectedAction?.Invoke();
                    LastKnownStatus = ConnectionStatus.Connected;
                }

                // And when we ever disconnect, try to establish again
                _lyncClient.StateChanged -= ClientStateChangedEvent;
                _lyncClient.StateChanged += ClientStateChangedEvent;

                // Default binds
                // When a new convo starts
                _lyncClient.ConversationManager.ConversationAdded -= ConversationAddedHandler;
                _lyncClient.ConversationManager.ConversationAdded += ConversationAddedHandler;
            }
            catch (ClientNotFoundException ex)
            {
                if (LastKnownStatus == ConnectionStatus.Connected)
                {
                    DisconnectedAction?.Invoke();
                }
                else
                {
                    LastKnownStatus = ConnectionStatus.Disconnected;
                }
            }
        }

        public void WaitUntilReconnectedToClient()
        {
            while (!this.IsSignedIn)
            {
                LastKnownStatus = ConnectionStatus.Disconnected;
                Task.Delay(this.reconnectionMilliseconds).Wait();
                // Try fetch again
                ConnectToLync();
            }
        }


        /// <summary>
        /// Binds the new message handler to any new messages coming from a conversation participant
        /// </summary>
        /// <param name="participant"></param>
        private void BindNewMessageHandlerToOtherParticipant(Participant participant)
        {
            // That's not me
            if (participant.Contact.Uri != _lyncClient.Self.Contact.Uri)
            {
                // When they send a message
                InstantMessageModality instantMessageModality = participant.Modalities[ModalityTypes.InstantMessage] as InstantMessageModality;
                instantMessageModality.InstantMessageReceived -= newMessageHandler;
                instantMessageModality.InstantMessageReceived += newMessageHandler;
            }
        }

        /// <summary>
        /// Binds the new message event handler to all current participants of a conversation now, as well as when new participants join
        /// </summary>
        /// <param name="conversation"></param>
        public void BindHandlerToConversationIMs(Conversation conversation)
        {
            // When a person joins, do it
            conversation.ParticipantAdded += (object sender, ParticipantCollectionChangedEventArgs args) =>
            {
                BindNewMessageHandlerToOtherParticipant(args.Participant);
            };
            
            // And bind to existing participants
            foreach (Participant participant in conversation.Participants)
            {
                BindNewMessageHandlerToOtherParticipant(participant);
            }
        }

        /// <summary>
        /// Bind the status update event handler
        /// </summary>
        /// <param name="handler"></param>
        public void BindStatusUpdate(EventHandler<ContactInformationChangedEventArgs> handler)
        {
            if (_lyncClient != null)
            {
                _lyncClient.Self.Contact.ContactInformationChanged -= handler;
                _lyncClient.Self.Contact.ContactInformationChanged += handler;
            }
        }

        /// <summary>
        /// Bind the new conversation event handler
        /// </summary>
        /// <param name="handler"></param>
        public void BindNewConversation(EventHandler<ConversationManagerEventArgs> handler)
        {
            if (_lyncClient != null)
            {
                _lyncClient.ConversationManager.ConversationAdded -= handler;
                _lyncClient.ConversationManager.ConversationAdded += handler;
            }
        }

        /// <summary>
        /// Binds the new message event handler
        /// </summary>
        /// <param name="handler"></param>
        public void BindNewConversationMessage(EventHandler<MessageSentEventArgs> handler)
        {
            // We just set the existing handler reference here, as we bound it ealier before this potentially had a value
            newMessageHandler = handler;
        }

        /// <summary>
        /// Gets the current availability of the logged-in Lync user
        /// </summary>
        /// <returns></returns>
        public ContactAvailability? GetAvailability()
        {
            return (ContactAvailability)_lyncClient.Self.Contact.GetContactInformation(ContactInformationType.Availability);
        }

        /// <summary>
        /// Gets the current conversations of the logged-in Lync user
        /// </summary>
        /// <returns></returns>
        public List<Conversation> GetCurrentConversations()
        {
            return _lyncClient.ConversationManager.Conversations.ToList();
        }
    }
}