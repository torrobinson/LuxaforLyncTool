using System;
using System.Windows.Forms;
using LuxaforLyncTool_Client.Properties;
using LuxaforLyncTool_Light;
using LuxaforLyncTool_Light.Color;
using LuxaforLyncTool_Lync;
using Microsoft.Lync.Model;

namespace LuxaforLyncTool_Client
{
    public class Process : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        private LightClient lightClient;
        private ChatClient chatClient;

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
            lightClient = new LightClient();

            // Create a new chat client
            chatClient = new ChatClient();

            ApplyCurrentStatus();
            ApplyCurrentConversations();

            // Bind to new changes
            BindStatusChanges();
            BindNewConversation();
            BindNewConversationMessage();
        }

        private void ApplyCurrentConversations()
        {
            // Try find any current conversations and bind to new messages for them
            // TODO: Implement
        }

        private void ApplyCurrentStatus()
        {
            // Try fetch the current user availability and set the light to it
            ContactAvailability? availability = chatClient.GetAvailability();
            if (availability != null)
            {
                SendLightBasedOnAvailability(availability ?? ContactAvailability.Free);
            }
        }

        private void BindNewConversationMessage()
        {
            chatClient.BindNewConversationMessage((sender, args) => { NotifyOfChat(); });
        }

        private void BindNewConversation()
        {
            chatClient.BindNewConversation((sender, args) => { NotifyOfChat(); });
        }

        private void BindStatusChanges()
        {
            // Bind what happens when status updates
            chatClient.BindStatusUpdate((sender, args) =>
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
                    lightClient.SendColor(SimpleColors.Yellow);
                    break;
                case ContactAvailability.Busy:
                    lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.BusyIdle:
                    lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.DoNotDisturb:
                    lightClient.SendColor(SimpleColors.Red);
                    break;
                case ContactAvailability.Free:
                    lightClient.SendColor(SimpleColors.Green);
                    break;
                case ContactAvailability.FreeIdle:
                    lightClient.SendColor(SimpleColors.Green);
                    break;
                case ContactAvailability.Offline:
                    lightClient.SendColor(SimpleColors.Off);
                    break;
                case ContactAvailability.TemporarilyAway:
                    lightClient.SendColor(SimpleColors.Yellow);
                    break;
                default:
                    break;
            }
        }

        private void NotifyOfChat()
        {
            lightClient.PulseColor(SimpleColors.Blue);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}