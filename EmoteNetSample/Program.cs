/*
 *  Project AZUSA © 2015 ( https://github.com/Project-AZUSA )
 *  E-mote.NET Sample - "NekoSprite" Preview
 *  LICENSE:CC 3.0 BY-NC-SA (CC 署名-非商业性使用-相同方式共享 3.0 中国大陆)
 *  AUTHOR:	UlyssesWu (wdwxy12345@gmail.com)
 *  NOTICE: 当前E-mote.NET正在开发，因此相关函数可能出现变动。此示例仅供预览。
 *          当前E-mote.NET版本为nekopara版本，但本实例中不包含nekopara的立绘文件。
 *          
 *          Under development, this demo is only for preview.
 *          
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;
using AZUSA.EmoteNet;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace NekoHacks
{
    class Program
    {
        const double REFRESH = 1.0 / 50.0;
        const int WIDTH = 900;
        const int HEIGHT = 600;

        private const bool EnableTransparentWindow = false;
        private static RenderForm form;
        private static Emote e;
        private static EmotePlayer _player;
        private static EmotePlayer _player2;
        private static PreciseTimer _timer = new PreciseTimer();
        private static double elaspedTime = 0;
        private static Device _device;
        private static Rectangle _screenRect;
        private static Texture _wallpaper;
        private static Sprite _sprite;
        private static SharpDX.Color _color = new SharpDX.ColorBGRA(0xffffffff);

        static unsafe void Main(string[] args)
        {
            Application.EnableVisualStyles();
            double processTime = 0.0;
            double wait = 0.0;
            //FontDescription fontDescription;
            //SharpDX.Rectangle fontDimension;
            //SharpDX.Direct3D9.Font font;
            //string displayText;

            _screenRect = Screen.PrimaryScreen.Bounds;
            form = new RenderForm("AZUSA E-mote Sample - NekoSprite");
            form.AutoScaleMode = AutoScaleMode.None;
            form.FormBorderStyle = FormBorderStyle.None;
            form.ClientSize = new Size(WIDTH, HEIGHT);
            form.Resize += FormOnResize;
            FormOnResize(form, null);
            form.TopMost = true;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(_screenRect.Width - form.Width, _screenRect.Height - form.Height);
            //form.AllowTransparency = true;
            e = new Emote(form.Handle, WIDTH, HEIGHT, false);
            e.EmoteInit();
            e.Device.TextureFilter = GrayTextureFilter;

            if (EnableTransparentWindow)
            {
                e = new Emote(form.Handle, WIDTH, HEIGHT, true, true);
            }

            _player = e.CreatePlayer("Vanilla", "dx_e-mote3.0バニラ私服b_鈴あり.psb");
            e.Device.UseTextureFilter = true; //turn on texture filter!
            //e.Device.SetMaskRegionClipping(true); //Set this to true will increase CPU usage but decrease GPU usage.
            _player2 = e.CreatePlayer("Chocola", "dx_e-mote3.0ショコラ制服b_鈴あり.psb");

            //I don't know how to use this method. I guess this method may combine two models (only if they are complementary).
            //virtual void  CreatePlayer (emote_uint32_t emoteObjectNum, const emote_uint8_t **emoteObjectImage, const emote_uint32_t *emoteObjectSize, class IEmotePlayer **player)=0 
            //_player2 = e.CreatePlayer("Chocola", new[] {"dx_e-mote3.0ショコラ制服b_鈴あり.psb", "dx_e-mote3.0バニラ私服b_鈴あり.psb"});

            _player.Show();
            _player2.Show();

            var faceTable = new Dictionary<string, float>()
            {
                    { "face_eye_UD", -30f },
                    { "face_eye_LR", 0.0f },
                    { "face_eye_open", 5 },
                    { "face_eyebrow", 20f },
                    { "face_mouth", 20 },
                    { "face_talk", 0.5f },
                    { "face_tears", 0.0f },
                    { "face_cheek", 0.0f },
            };
            
            _device = new Device(new IntPtr(e.D3Device));

            //// Initialize the Font
            //fontDescription = new FontDescription()
            //{
            //    Height = 72,
            //    Italic = false,
            //    CharacterSet = FontCharacterSet.Default,
            //    FaceName = "微软雅黑",
            //    MipLevels = 0,
            //    OutputPrecision = FontPrecision.TrueType,
            //    PitchAndFamily = FontPitchAndFamily.Default,
            //    Quality = FontQuality.ClearType,
            //    Weight = FontWeight.Bold
            //};

            //font = new Font(_device, fontDescription);

            //displayText = "NekoSprite\nProject AZUSA © 2015";
            //// Measure the text to display
            //fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, form.Height * 2 / 3, form.Width, form.Height / 3), FontDrawFlags.Center | FontDrawFlags.VerticalCenter);

            _player.SetScale(0.4f, 0, 0);
            _player.SetCoord(300, 50, 0, 0);
            _player.StartWind(0f, 1f, 0.8f, 0.5f, 0.8f);
            _player.SetSmoothing(true);

            _player2.SetScale(0.4f, 0, 0);
            _player2.SetCoord(-100, 50, 0, 0);
            _player2.SetSmoothing(true);

            _player2.SetVariables(faceTable, 2000f, 1f); //-Why are you so angry? -Why am I monochrome?

            //ctrlform.TopMost = true;
            Thread th = new Thread(() =>
            {
                var ctrlform2 = new FormConsole(_player2, "巧克力");
                ctrlform2.Closed += (sender, eventArgs) => {
                    e.DeletePlayer(_player2); //Try to close this form, the memory will be released
                };
                ctrlform2.Show();
                var ctrlform = new FormConsole(_player, "香草");
                //ctrlform.Show();
                Application.Run(ctrlform);
            });
            th.Start();
            //Control
            form.MouseDown += form_MouseDown;
            form.MouseMove += form_MouseMove;
            form.MouseUp += form_MouseUp;
            form.MouseWheel += (sender, eventArgs) => _player.OffsetScale(1 + ConvertDelta(eventArgs.Delta));

            RenderLoop.Run(form, Render);
            //RenderLoop.Run(form, Render2); //不会执行
        }

        //For efficiency, you have to use unsafe method for filter
        private static unsafe void GrayTextureFilter(byte* image, uint imageSize)
        {
            while (imageSize > 0)
            {
                //BGRA
                byte gray = (byte)(0.298912f * image[2] + 0.586611f * image[1] + 0.114478f * image[0]);
                image[0] = image[1] = image[2] = gray;
                //image[3] = (byte)(image[3] * 0.5f); //Alpha
                image += 4;
                imageSize -= 4;
            }
            //int p = 0;
            //while (p + 4 < imageSize)
            //{
            //    byte gray = (byte) (0.298912f*image[p + 2] + 0.586611f*image[p + 1] + 0.114478f*image[p]);
            //    image[p] = image[p + 1] = image[p + 2] = gray;
            //    p += 4;
            //}
        }


        private static void Render()
        {
            elaspedTime += _timer.GetElaspedTime();
            if (elaspedTime < REFRESH)
            {
                Thread.Sleep(1);
                return;
            }
            e.Update((float)REFRESH * 1000);
            _device.Clear(ClearFlags.Target, Color.Transparent, 1.0f, 0);

            _device.BeginScene();
            //device.UpdateSurface(device.GetBackBuffer(0,0),new Surface(new IntPtr(e.D3DSurface)));
            e.Draw();
            _device.EndScene();
            _device.Present();
            if (EnableTransparentWindow)
            {
                e.D3DAfterPresentSetTransparentWindow();
            }

            elaspedTime = 0;
        }


        private static void FormOnResize(object sender, EventArgs eventArgs)
        {
            var form = (Form)sender;
            int[] margins = new int[] { 0, 0, form.Width, form.Height };

            // Extend aero glass style to whole form
            Util.DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }

        static private int lastX = 0, lastY = 0;
        static private bool leftMouseDown = false;
        private static bool rightMouseDown = false;

        static void form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = true;
            }
        }

        static void form_MouseMove(object sender, MouseEventArgs ex)
        {
            if (leftMouseDown)
            {
                _player.OffsetCoord(ex.X - lastX, ex.Y - lastY);
                lastX = ex.X;
                lastY = ex.Y;
                //form.Location.Offset(ex.X - lastX, ex.Y - lastY);
            }
        }

        static void form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = true;
                lastX = e.X;
                lastY = e.Y;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = true;
            }
        }

        private static float ConvertDelta(int delta)
        {
            return delta / 120.0f / 50.0f;
        }

    }
}
