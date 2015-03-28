/*
 *  Project AZUSA © 2015 ( https://github.com/Project-AZUSA )
 *  E-mote.NET Sample for WPF
 *  LICENSE:CC 3.0 BY-NC-SA (CC 署名-非商业性使用-相同方式共享 3.0 中国大陆)
 *  AUTHOR:	UlyssesWu (wdwxy12345@gmail.com)
 *  NOTICE: 当前E-mote.NET版本为nekopara版本，但本实例中不包含nekopara的立绘文件。
 *          在WPF中使用非常非常耗费CPU和内存资源！这是因为E-mote SDK不支持DX9Ex，所有绘制经过了一次复制。
 *          WARNING:Very CPU and Memory costly! Without DX9Ex support in E-mote SDK, all frames need to be copied when rendering.
 *          
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AZUSA.EmoteNet;

namespace EmoteNetSample_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3DImage _di;
        private IntPtr _scene;

        private WindowInteropHelper _helper;
        private Emote _emote;

        const float MSTOF60THS = 60.0f / 1000.0f;  // msを1/60秒カウントへ変換。
        const double F60THSTOMS = 1000.0f / 60.0f;  // 1/60秒カウントをmsへ変換。

        const float refreshRate = 2000.0f / 60.0f - 30.0f;  // 1/60秒カウントをmsへ変換。

        DateTime lastTime = DateTime.Now;
        double processTime = 0.0;
        double wait = 0.0;

        static private double lastX = 0, lastY = 0;
        static private bool leftMouseDown = false;
        private static bool rightMouseDown = false;
        private static double midX, midY;

        public MainWindow()
        {
            _helper = new WindowInteropHelper(this);

            // create a D3DImage to host the scene and
            // monitor it for changes in front buffer availability
            _di = new D3DImage();
            _di.IsFrontBufferAvailableChanged
                += new DependencyPropertyChangedEventHandler(OnIsFrontBufferAvailableChanged);
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.MouseMove += MainWindow_MouseMove;
            this.MouseWheel += MainWindow_MouseWheel;
            // make a brush of the scene available as a resource on the window
            Resources["NekoHacksScene"] = new ImageBrush(_di);


            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // parse the XAML
            InitializeComponent();

            this.Width = 1280;
            this.Height = 720;
            midX = Width/2;
            midY = Height/2;

            _emote = new Emote(_helper.EnsureHandle(), (int)this.Width, (int)this.Height, true);

            _emote.EmoteInit("dx_e-mote3.0バニラ私服b_鈴あり.psb");
            _emote.EmoteOffsetScale(0.5f);

            // begin rendering the custom D3D scene into the D3DImage
            BeginRenderingScene();
        }

        void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _emote.EmoteOffsetScale(1 + ConvertDelta(e.Delta));
        }

        private double deltaX = 0, deltaY = 0;

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
            {
                var ex = e.GetPosition(this);
                _emote.EmoteOffsetCoord((int)(ex.X - lastX),(int)( ex.Y - lastY));
                lastX = ex.X;
                lastY = ex.Y;
            }
            else
            {
                var ex = e.GetPosition(this);
                if (ex.X>midX)
                {
                    Console.WriteLine();
                }
                deltaX = (ex.X - midX)/midX * 64;
                deltaY = (ex.Y - midY)/midY * 64;

                _emote.EmotePlayer.SetVariable("head_UD", (float) deltaY, 0, 0);
                _emote.EmotePlayer.SetVariable("head_LR", (float) deltaX, 0, 0);
                _emote.EmotePlayer.SetVariable("body_UD", (float) deltaY, 0, 0);
                _emote.EmotePlayer.SetVariable("body_LR", (float) deltaX, 0, 0);
                _emote.EmotePlayer.SetVariable("face_eye_UD", (float)deltaY, 0, 0);
                _emote.EmotePlayer.SetVariable("face_eye_LR", (float)deltaX, 0, 0);

            }
        }

        void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                leftMouseDown = false;
            }
        }



        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                leftMouseDown = true;
                lastX = e.GetPosition(this).X;
                lastY = e.GetPosition(this).Y;
            }
        }
        private static float ConvertDelta(int delta)
        {
            return delta / 120.0f / 50.0f;
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // if the front buffer is available, then WPF has just created a new
            // D3D device, so we need to start rendering our custom scene
            if (_di.IsFrontBufferAvailable)
            {
                BeginRenderingScene();
            }
            else
            {
                // If the front buffer is no longer available, then WPF has lost its
                // D3D device so there is no reason to waste cycles rendering our
                // custom scene until a new device is created.
                StopRenderingScene();
            }
        }

        private void BeginRenderingScene()
        {
            if (_di.IsFrontBufferAvailable)
            {
                // create a custom D3D scene and get a pointer to its surface
                _scene = new IntPtr(_emote.D3DSurface);

                // set the back buffer using the new scene pointer
                _di.Lock();
                _di.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _scene);
                _di.Unlock();

                // leverage the Rendering event of WPF's composition target to
                // update the custom D3D scene
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void StopRenderingScene()
        {
            // This method is called when WPF loses its D3D device.
            // In such a circumstance, it is very likely that we have lost 
            // our custom D3D device also, so we should just release the scene.
            // We will create a new scene when a D3D device becomes 
            // available again.
            CompositionTarget.Rendering -= OnRendering;
            _emote.D3DRelease();
            _scene = IntPtr.Zero;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            DateTime current = DateTime.Now;
            double elapsed = (current - lastTime).TotalMilliseconds;

            if (elapsed < refreshRate)
            {
                return;
            }
            lastTime = current;
            // when WPF's composition target is about to render, we update our 
            // custom render target so that it can be blended with the WPF target
            UpdateScene();

        }

        private void UpdateScene()
        {
            if (_di.IsFrontBufferAvailable && _scene != IntPtr.Zero)
            {
                _emote.EmoteUpdate(17);
                // lock the D3DImage
                _di.Lock();
                // update the scene (via a call into our custom library)
                _emote.D3DBeginScene();
                _emote.EmoteDraw();
                _emote.D3DEndScene();
                // invalidate the updated region of the D3DImage (in this case, the whole image)
                _di.AddDirtyRect(new Int32Rect(0, 0, _emote.SurfaceWidth, _emote.SurfaceHeight));
                // unlock the D3DImage
                _di.Unlock();

            }
        }
    }
}
