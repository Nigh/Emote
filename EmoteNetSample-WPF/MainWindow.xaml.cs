/*
 *  Project AZUSA © 2015 ( https://github.com/Project-AZUSA )
 *  E-mote.NET Sample for WPF
 *  LICENSE:CC 3.0 BY-NC-SA (CC 署名-非商业性使用-相同方式共享 3.0 中国大陆)
 *  AUTHOR:	UlyssesWu (wdwxy12345@gmail.com)
 *  NOTICE: 当前E-mote.NET版本为nekopara版本，但本实例中不包含nekopara的立绘文件。
 *          在WPF中使用可能非常耗费CPU和内存资源！
 *          WARNING:May be very CPU and Memory costly! Without DX9Ex support in E-mote SDK, all frames need to be copied when rendering.
 *          
 */

using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using AZUSA.EmoteNet;

/*      WindowStyle="None"
        AllowsTransparency="True"
        两者需要一起使用
*/

namespace EmoteNetSample_WPF
{
    enum D3DResult : uint
    {
        DEVICE_LOST = 0x88760868,
        DEVICE_NOTRESET = 0x88760869,
        INVAILDCALL = 0x8876086C,
        OK = 0x0,
        UNKNOWN_ERROR = 0xFFFFFFFF
    }

    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const float MSTOF60THS = 60.0f/1000.0f; // msを1/60秒カウントへ変換。
        const double F60THSTOMS = 1000.0f/60.0f; // 1/60秒カウントをmsへ変換。

        const float refreshRate = 1000.0f/80.0f; // 1/60秒カウントをmsへ変換。

        private static double lastX, lastY;
        private static bool leftMouseDown;
        private static bool rightMouseDown = false;
        private static double midX, midY;
        private D3DImage _di;
        private Emote _emote;
        private WindowInteropHelper _helper;
        private EmotePlayer _player;
        private IntPtr _scene;

        private PreciseTimer _timer;

        private double deltaX, deltaY;
        double elapsedTime;

        DateTime lastTime = DateTime.Now;
        double wait = 0.0;

        public MainWindow()
        {
            _helper = new WindowInteropHelper(this);

            // create a D3DImage to host the scene and
            // monitor it for changes in front buffer availability
            _di = new D3DImage();
            _di.IsFrontBufferAvailableChanged
                += OnIsFrontBufferAvailableChanged;
            MouseDown += MainWindow_MouseDown;
            MouseUp += MainWindow_MouseUp;
            MouseMove += MainWindow_MouseMove;
            MouseWheel += MainWindow_MouseWheel;
            // make a brush of the scene available as a resource on the window
            Resources["NekoHacksScene"] = new ImageBrush(_di);

            double x = SystemParameters.WorkArea.Width; //得到屏幕工作区域宽度
            double y = SystemParameters.WorkArea.Height; //得到屏幕工作区域高度
            double x1 = SystemParameters.PrimaryScreenWidth; //得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight; //得到屏幕整体高度

            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = x1 - 800;
            Top = y1 - 600;
            // parse the XAML
            InitializeComponent();
            Topmost = true;
            Width = 800;
            Height = 600;
            midX = Width/2;
            midY = Height/2 - 100;

            _emote = new Emote(_helper.EnsureHandle(), (int) Width, (int) Height, true);
            _emote.EmoteInit();
            _player = _emote.CreatePlayer("Vanilla", "dx_e-mote3.0バニラ私服b_鈴あり.psb");
            _player.SetScale(0.4f, 0, 0);
            _player.OffsetCoord(0, 50);
            _player.SetSmoothing(true);
            _player.Show();
            // begin rendering the custom D3D scene into the D3DImage
            BeginRenderingScene();
        }

        void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _player.OffsetScale(1 + ConvertDelta(e.Delta));
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
            {
                var ex = e.GetPosition(this);
                _player.OffsetCoord((int) (ex.X - lastX), (int) (ex.Y - lastY));
                lastX = ex.X;
                lastY = ex.Y;
            }
            else
            {
                var ex = e.GetPosition(this);
                deltaX = (ex.X - midX)/midX*64;
                deltaY = (ex.Y - midY)/midY*64;

                float frameCount = 0f;
                //float frameCount = 50f;
                float easing = 0f;
                _player.SetVariable("head_UD", (float) deltaY, frameCount, easing);
                _player.SetVariable("head_LR", (float) deltaX, frameCount, easing);
                _player.SetVariable("body_UD", (float) deltaY, frameCount, easing);
                _player.SetVariable("body_LR", (float) deltaX, frameCount, easing);
                _player.SetVariable("face_eye_UD", (float) deltaY, frameCount, easing);
                _player.SetVariable("face_eye_LR", (float) deltaX, frameCount, easing);
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
            return delta/120.0f/50.0f;
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

                _timer = new PreciseTimer();
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
            _scene = IntPtr.Zero;

            _emote.OnDeviceLost();
            while (_emote.D3DTestCooperativeLevel() == (uint) D3DResult.DEVICE_LOST)
            {
                Thread.Sleep(5);
            }
            if (_emote.D3DTestCooperativeLevel() == (uint) D3DResult.DEVICE_NOTRESET)
            {
                _emote.D3DReset();
                _emote.OnDeviceReset();
                //_emote.D3DInitRenderState();
                //var hr = _emote.D3DTestCooperativeLevel();
            }
            else
            {
                Console.WriteLine("{0:x8}", _emote.D3DTestCooperativeLevel());
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            elapsedTime += _timer.GetElaspedTime()*1000;

            if (elapsedTime < refreshRate)
            {
                return;
            }

            // when WPF's composition target is about to render, we update our 
            // custom render target so that it can be blended with the WPF target
            UpdateScene(elapsedTime);

            elapsedTime = 0;
        }

        private void UpdateScene(double elasped)
        {
            if (_di.IsFrontBufferAvailable && _scene != IntPtr.Zero)
            {
                _emote.Update((float) elasped);
                // lock the D3DImage
                _di.Lock();
                // update the scene (via a call into our custom library)
                _emote.D3DBeginScene();
                _emote.Draw();
                _emote.D3DEndScene();
                // invalidate the updated region of the D3DImage (in this case, the whole image)
                _di.AddDirtyRect(new Int32Rect(0, 0, _emote.SurfaceWidth, _emote.SurfaceHeight));
                // unlock the D3DImage
                _di.Unlock();
            }
        }
    }
}