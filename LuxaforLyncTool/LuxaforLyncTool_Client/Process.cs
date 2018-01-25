﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        NotifyIcon _notifyIcon;

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

            ToolStripMenuItem testItem = new ToolStripMenuItem() { Text = "Test!" };
            testItem.Click += (sender, args) =>
            {
                MessageBox.Show("Test!", "Test caption", MessageBoxButtons.OK);
            };

            _notifyIcon.ContextMenuStrip.Items.Add(testItem);

        }

        public void Listen()
        {
           // Create a new light client and connect
           lightClient = new LightClient();

           // Create a new chat client
           chatClient = new ChatClient();

            BindStatusChanges();
            BindNewConversation();
            BindNewConversationMessage();
        }

        private void BindNewConversationMessage()
        {
            chatClient.BindNewConversationMessage((object sender, MessageSentEventArgs args) =>
            {
                NotofyOfChat();
            });
        }

        private void BindNewConversation()
        {
            chatClient.BindNewConversation((object sender, ConversationManagerEventArgs args) =>
            {
                NotofyOfChat();
            });
        }

        private void BindStatusChanges()
        {
            // Bind what happens when status updates
            chatClient.BindStatusUpdate((object sender, ContactInformationChangedEventArgs args) =>
            {
                Contact me = sender as Contact;
                // If this was an availability update,
                if (args.ChangedContactInformation.Contains(ContactInformationType.Availability))
                {
                    // Get the new availability
                    ContactAvailability availability =
                        (ContactAvailability)me.GetContactInformation(ContactInformationType.Availability);

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
            });
        }

        private void NotofyOfChat()
        {
            lightClient.PulseColor(SimpleColors.Blue);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
