using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LuxaforLyncTool_Client.Properties;
using LuxaforLyncTool_Client.Resources;
using LuxaforLyncTool_Light;
using LuxaforLyncTool_Light.Color;
using LuxaforLyncTool_Light.Device;
using LuxaforLyncTool_Lync;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Win32;

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

        public void DisplayIcon()
        {
            _notifyIcon.Text = Strings.ApplicationName;
            _notifyIcon.Icon = Images.TrayIcon;
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            var exitOption = new ToolStripMenuItem { Text = Strings.Exit };
            exitOption.Click += (sender, args) => { Application.Exit(); };
            _notifyIcon.ContextMenuStrip.Items.Add(exitOption);

            var strobeTest = new ToolStripMenuItem { Text = "Strobe" };
            strobeTest.Click += (sender, args) => { _lightClient.SendPulseColor(Color.Red, Speed.Fast, 5, LEDs.Back); };
            _notifyIcon.ContextMenuStrip.Items.Add(strobeTest);
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
            BindComputerLocked();

            // And apply to current states
            ApplyCurrentStatus();
            ApplyCurrentConversations();
        }

        private void BindComputerLocked()
        {
            // On computer session change
            Microsoft.Win32.SystemEvents.SessionSwitch += (sender, args) =>
            {
                if (args.Reason == SessionSwitchReason.SessionLock)
                {
                    // Computer was locked
                    SendLightBasedOnAvailability(ContactAvailability.Busy);
                }
                else if (args.Reason == SessionSwitchReason.SessionUnlock)
                {
                    // Computer was unlocked. Change light to natural state
                    ApplyCurrentStatus();
                }
                else if (args.Reason == SessionSwitchReason.SessionLogoff)
                {
                    SendLightBasedOnAvailability(ContactAvailability.Offline);
                }
            };
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
                    _lightClient.SendComplexColor(Color.Orange);
                    break;
                case ContactAvailability.Busy:
                    _lightClient.SendComplexColor(Color.Red);
                    break;
                case ContactAvailability.BusyIdle:
                    _lightClient.SendComplexColor(Color.Red);
                    break;
                case ContactAvailability.DoNotDisturb:
                    _lightClient.SendComplexColor(Color.Red);
                    break;
                case ContactAvailability.Free:
                    _lightClient.SendComplexColor(Color.Green);
                    break;
                case ContactAvailability.FreeIdle:
                    _lightClient.SendComplexColor(Color.Yellow);
                    break;
                case ContactAvailability.Offline:
                    _lightClient.SendSimpleColor(SimpleColors.Off);
                    break;
                case ContactAvailability.TemporarilyAway:
                    _lightClient.SendComplexColor(Color.Orange);
                    break;
                default:
                    break;
            }
        }

        private void NotifyOfChat()
        {
            _lightClient.SendPulseColor(Color.Blue, Speed.Fast, 3);
        }

        public void Dispose()
        {
            _lightClient.SendSimpleColor(SimpleColors.Off);
        }
    }
}