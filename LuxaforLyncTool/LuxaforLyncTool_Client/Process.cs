using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LuxaforLyncTool_Client.Properties;
using LuxaforLyncTool_Client.Resources;
using LuxaforLyncTool_Light;
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

        private ToolStripMenuItem brightnessMenu;

        /// <summary>
        /// Constructor
        /// </summary>
        public Process()
        {
            _notifyIcon = new NotifyIcon();
        }

        /// <summary>
        /// Prepare and display the application's icon in the host's notification tray
        /// </summary>
        public void DisplayIcon()
        {
            _notifyIcon.Text = Strings.ApplicationName;
            _notifyIcon.Icon = Images.TrayIcon;
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            // Exit
            var exitOption = new ToolStripMenuItem { Text = Strings.Exit };
            exitOption.Click += (sender, args) => { Application.Exit(); };
            

            // Brightness
            brightnessMenu = new ToolStripMenuItem { Text = Strings.Brightness};
            ToolStripMenuItem quarterBrightnessItem = new ToolStripMenuItem { Text = "25%", Tag = 0.25, CheckOnClick = true };
            ToolStripMenuItem helfBrightnessItem = new ToolStripMenuItem { Text = "50%", Tag = 0.50, CheckOnClick = true };
            ToolStripMenuItem threeQuarterBrightnessItem = new ToolStripMenuItem { Text = "75%", Tag = 0.75, CheckOnClick = true };
            ToolStripMenuItem maxBrightnessItem = new ToolStripMenuItem { Text = "100%", Tag = 1.0, CheckOnClick = true };
            brightnessMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                maxBrightnessItem, threeQuarterBrightnessItem,helfBrightnessItem, quarterBrightnessItem
            });

            quarterBrightnessItem.Click += (sender, args) =>
            {
                _lightClient.Brightness = 0.25;
                CheckCurrentBrightnessItem();
                ApplyCurrentStatus();
            };
            helfBrightnessItem.Click += (sender, args) =>
            {
                _lightClient.Brightness = 0.5;
                CheckCurrentBrightnessItem();
                ApplyCurrentStatus();
            };
            threeQuarterBrightnessItem.Click += (sender, args) =>
            {
                _lightClient.Brightness = 0.75;
                CheckCurrentBrightnessItem();
                ApplyCurrentStatus();
            };
            maxBrightnessItem.Click += (sender, args) =>
            {
                _lightClient.Brightness = 1.0;
                CheckCurrentBrightnessItem();
                ApplyCurrentStatus();
            };

            ToolStripMenuItem aboutItem = new ToolStripMenuItem() {Text = Strings.About};
            aboutItem.Click += (sender, args) =>
            {
                About about = new About();
                about.Show();
            };

            _notifyIcon.ContextMenuStrip.Items.Add(brightnessMenu);
            _notifyIcon.ContextMenuStrip.Items.Add(aboutItem);
            _notifyIcon.ContextMenuStrip.Items.Add(exitOption);
        }

        /// <summary>
        /// Ensure that the brightness setting in the list of brightnesses is checked-off
        /// </summary>
        public void CheckCurrentBrightnessItem()
        {
            if (_lightClient != null)
            {
                foreach (ToolStripMenuItem subItem in brightnessMenu.DropDownItems)
                {
                    if (subItem.Tag != null && subItem.Tag is double)
                    {
                        subItem.Checked = ((double)subItem.Tag) == _lightClient.Brightness;
                    }
                }
            }
        }

        /// <summary>
        /// Set up listeners for all bound events that will interact with the lights
        /// </summary>
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

            // Initially check off the current brightness
            CheckCurrentBrightnessItem();
        }

        /// <summary>
        /// Light when the computer is locked
        /// </summary>
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

        /// <summary>
        /// Apply the bound handlers to all current conversations
        /// </summary>
        private void ApplyCurrentConversations()
        {
            // Try find any current conversations and bind to new messages for them
            List<Conversation> currentConversations = _chatClient.GetCurrentConversations();
            foreach (Conversation conversation in currentConversations)
            {
                _chatClient.BindHandlerToConversationIMs(conversation);
            }
        }

        /// <summary>
        /// Apply the bound handlers to the current status
        /// </summary>
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

        /// <summary>
        /// Mapping of Lync availabilities to light colors
        /// </summary>
        /// <param name="availability"></param>
        private void SendLightBasedOnAvailability(ContactAvailability availability)
        {
            // And send the appropriate color
            switch (availability)
            {
                case ContactAvailability.Away:
                    _lightClient.SendColor(Color.Orange);
                    break;
                case ContactAvailability.Busy:
                    _lightClient.SendColor(Color.Red);
                    break;
                case ContactAvailability.BusyIdle:
                    _lightClient.SendColor(Color.Red);
                    break;
                case ContactAvailability.DoNotDisturb:
                    _lightClient.SendColor(Color.Red);
                    break;
                case ContactAvailability.Free:
                    _lightClient.SendColor(Color.Green);
                    break;
                case ContactAvailability.FreeIdle:
                    _lightClient.SendColor(Color.Yellow);
                    break;
                case ContactAvailability.Offline:
                    _lightClient.SendColor(Color.Black);
                    break;
                case ContactAvailability.TemporarilyAway:
                    _lightClient.SendColor(Color.Orange);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// What to do when a new chat message occurs (both new conversations and new IMs)
        /// </summary>
        private void NotifyOfChat()
        {
            _lightClient.SendPulseColor(Color.Blue, Speed.Fast, 3);
        }

        /// <summary>
        /// Teardown the application
        /// </summary>
        public void Dispose()
        {
            // Turn off the light before closing
            _lightClient.SendColor(Color.Black);
        }
    }
}