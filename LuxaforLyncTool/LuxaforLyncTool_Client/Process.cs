using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LuxaforLyncTool_Client.Properties;
using LuxaforLyncTool_Light;
using LuxaforLyncTool_Light.Color;
using LuxaforLyncTool_Lync;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace LuxaforLyncTool_Client
{
    public class Process : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        private LightClient _lightClient;
        private ChatClient _chatClient;

        public Process()
        {
            _notifyIcon = new NotifyIcon();
        }

        public void Display()
        {
            _notifyIcon.Text = "Test text";
            _notifyIcon.Icon = Resources.tray_icon;
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            var testItem = new ToolStripMenuItem {Text = "Test!"};
            testItem.Click += (sender, args) => { MessageBox.Show("Test!", "Test caption", MessageBoxButtons.OK); };

            _notifyIcon.ContextMenuStrip.Items.Add(testItem);
        }

        public void Listen()
        {
            // Create a new light client and connect
            _lightClient = new LightClient();

            // Create a new chat client
            _chatClient = new ChatClient();

            // Bind to new changes
            BindStatusChanges();
            BindNewConversation();
            BindNewConversationMessage();

            // And apply to current states
            ApplyCurrentStatus();
            ApplyCurrentConversations();
        }

        private void ApplyCurrentConversations()
        {
            // Try find any current conversations and bind to new messages for them
            List<Conversation> currentConversations = _chatClient.GetCurrentConversations();
            foreach (Conversation conversation in currentConversations)
            {
                _chatClient.BindHandlerToConversationIMs(conversation);
            }
        }

        private void ApplyCurrentStatus()
        {
            // Try fetch the current user availability and set the light to it
            ContactAvailability? availability = _chatClient.GetAvailability();
            if (availability != null)
            {
                SendLightBasedOnAvailability(availability ?? ContactAvailability.Free);
            }
        }

        private void BindNewConversationMessage()
        {
            _chatClient.BindNewConversationMessage((sender, args) => { NotifyOfChat(); });
        }

        private void BindNewConversation()
        {
            _chatClient.BindNewConversation((sender, args) => { NotifyOfChat(); });
        }

        private void BindStatusChanges()
        {
            // Bind what happens when status updates
            _chatClient.BindStatusUpdate((sender, args) =>
            {
                var me = sender as Contact;
                // If this was an availability update,
                if (args.ChangedContactInformation.Contains(ContactInformationType.Availability))
                {
                    // Get the new availability
                    var availability = (ContactAvailability) me.GetContactInformation(ContactInformationType.Availability);
                    // Send a color change request
                    SendLightBasedOnAvailability(availability);
                }
            });
        }

        private void SendLightBasedOnAvailability(ContactAvailability availability)
        {
            // And send the appropriate color
            switch (availability)
            {
                case ContactAvailability.Away:
                    _lightClient.SendColor(SimpleColors.Yellow);
                    break;
                case ContactAvailability.Busy:
                    _lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.BusyIdle:
                    _lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.DoNotDisturb:
                    _lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.Free:
                    _lightClient.SendColor(SimpleColors.Green);
                    break;
                case ContactAvailability.FreeIdle:
                    _lightClient.SendColor(SimpleColors.Green);
                    break;
                case ContactAvailability.Offline:
                    _lightClient.SendColor(SimpleColors.Off);
                    break;
                case ContactAvailability.TemporarilyAway:
                    _lightClient.SendColor(SimpleColors.Yellow);
                    break;
                default:
                    break;
            }
        }

        private void NotifyOfChat()
        {
            _lightClient.PulseColor(SimpleColors.Blue);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}