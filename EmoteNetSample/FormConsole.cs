using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AZUSA.EmoteNet;

namespace NekoHacks
{
    public partial class FormConsole : Form
    {
        private EmotePlayer emote;
        private Dictionary<string, Control> argTracks = new Dictionary<string, Control>();
        private bool haveWind = false;
        private float windStart = 0.0f,windEnd = 1.0f,windSpeed = 0.0f,powerMin=0.0f,powerMax=1.0f;

        public FormConsole(EmotePlayer emotePlayer,string info = "")
        {
            emote = emotePlayer;
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(info))
            {
                this.Text = string.Format("猫娘控制台 - 正在控制:{0}",info);
            }
            haveWind = chkWind.Checked;
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            ArgTrackBar temp = new ArgTrackBar("Bust摇动",ConvertToInt(emote.GetBustScale()));
            temp.OnValueChanged += SetBustValue;
            argTracks.TryAdd("Bust", temp);

            temp = new ArgTrackBar("头发摇动",ConvertToInt(emote.GetHairScale()));
            temp.OnValueChanged += SetHairValue;
            argTracks.TryAdd("Hair", temp);

            temp = new ArgTrackBar("风向");
            temp.OnValueChanged += SetWindDirect;
            argTracks.TryAdd("Wind", temp);

            temp = new ArgTrackBar("风力");
            temp.OnValueChanged += SetWindSpeed;
            argTracks.TryAdd("WindSpeed", temp);

            UpdatePanel();
            //foreach (Control control in flpArg.Controls)
            //{
            //    control.Dock = DockStyle.Top;
            //}
        }

        private void UpdatePanel()
        {
            foreach (var argTrackBar in argTracks)
            {
                if (!flpArg.Controls.Contains(argTrackBar.Value))
                {
                    flpArg.Controls.Add(argTrackBar.Value);
                    argTrackBar.Value.Dock = DockStyle.Top;
                }
            }
        }

        private void UpdateWind()
        {
            if (haveWind)
            {
                emote.StartWind(windStart, windEnd, windSpeed, powerMin, powerMax);
            }
        }

        private void SetWindSpeed(object sender, ArgTrackBar.TrackEventArgs eventArgs)
        {
            windSpeed = ConvertPercent(eventArgs.Value);
            UpdateWind();
        }

        private void SetWindDirect(object sender, ArgTrackBar.TrackEventArgs eventArgs)
        {
            if (eventArgs.Value < 50)
            {
                windStart = ConvertPercent(eventArgs.Value);
                windEnd = 1.0f;
            }
            else
            {
                windStart = ConvertPercent(eventArgs.Value);
                windEnd = 0.0f;
            }
            UpdateWind();
        }

        private void SetHairValue(object sender, ArgTrackBar.TrackEventArgs eventArgs)
        {
            emote.SetHairScale(ConvertPercent(eventArgs.Value));
        }

        private void SetBustValue(object sender, ArgTrackBar.TrackEventArgs eventArgs)
        {
            emote.SetBustScale(ConvertPercent(eventArgs.Value));
        }

        private static float ConvertPercent(int value)
        {
            return ((float) value)/100.0f;
        }

        private static int ConvertToInt(float value)
        {
            return (int) (value*100);
        }

        private void chkWind_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWind.Checked)
            {
                haveWind = true;
                emote.StartWind(windStart,windEnd,windSpeed,powerMin,powerMax);
            }
            else
            {
                haveWind = false;
                emote.StopWind();
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            var count = emote.CountVariables();
            for (uint i = 0; i < count; i++)
            {
                Console.WriteLine(emote.GetVariableLabelAt(i));
                ArgTrackBar temp = new ArgTrackBar(emote.GetVariableLabelAt(i),
                    ConvertToInt(emote.GetVariable(emote.GetVariableLabelAt(i))),-4097, 4097);
                temp.OnValueChanged += SetVariable;
                argTracks.TryAdd(emote.GetVariableLabelAt(i), temp);
            }
            UpdatePanel();
        }

        private void SetVariable(object sender, ArgTrackBar.TrackEventArgs eventArgs)
        {
            emote.SetVariable(eventArgs.Name,ConvertPercent(eventArgs.Value),0,0);
        }

        private void btnTime_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Playing");
            var count = emote.CountPlayingTimelines();
            for (uint i = 0; i < count; i++)
            {
                Console.WriteLine(emote.GetPlayingTimelineLabelAt(i));
                Button btn = new Button()
                {
                    Name = emote.GetPlayingTimelineLabelAt(i),
                    Text = emote.GetPlayingTimelineLabelAt(i),
                    Width = 200
                };
                btn.Click += btn_Click;
                argTracks.TryAdd(emote.GetPlayingTimelineLabelAt(i), btn);
            }
            Console.WriteLine("Diff");
            count = emote.CountDiffTimelines();
            for (uint i = 0; i < count; i++)
            {
                Console.WriteLine(emote.GetDiffTimelineLabelAt(i));
                Button btn = new Button()
                {
                    Name = emote.GetDiffTimelineLabelAt(i),
                    Text = emote.GetDiffTimelineLabelAt(i),
                    Width = 200
                };
                btn.Click += btn_Click;
                argTracks.TryAdd(emote.GetDiffTimelineLabelAt(i), btn);
            }
            Console.WriteLine("Main");
            count = emote.CountMainTimelines();
            for (uint i = 0; i < count; i++)
            {
                Console.WriteLine(emote.GetMainTimelineLabelAt(i));
                Button btn = new Button()
                {
                    Name = emote.GetMainTimelineLabelAt(i),
                    Text = emote.GetMainTimelineLabelAt(i),
                    Width = 200
                };
                btn.Click += btn_Click;
                argTracks.TryAdd(emote.GetMainTimelineLabelAt(i), btn);
            }
            UpdatePanel();
        }

        void btn_Click(object sender, EventArgs e)
        {
            emote.PlayTimeline(((Button)sender).Text,TimelinePlayFlags.NONE);
        }

        private void FormConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
