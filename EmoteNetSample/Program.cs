/*
 *  Project AZUSA © 2015 ( https://github.com/Project-AZUSA )
 *  E-mote.NET Sample - "NekoSprite" Preview
 *  LICENSE:CC 3.0 BY-NC-SA (CC 署名-非商业性使用-相同方式共享 3.0 中国大陆)
 *  AUTHOR:	UlyssesWu (wdwxy12345@gmail.com)
 *  NOTICE: 当前E-mote.NET正在开发，因此相关函数可能出现变动。此示例仅供预览。
 *          当前E-mote.NET版本为nekopara版本，但本实例中不包含nekopara的立绘文件。
 *          
 *          Under development, this sample is only for preview currently.
 *          
 */

using System;
using System.Threading;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;
using EmoteNet;
using Font = SharpDX.Direct3D9.Font;

namespace NekoSprite
{
    class Program
    {
        const double F60THSTOMS = 1000.0f / 60.0f;
        private static double elaspedTime = 2*F60THSTOMS;
        static void Main(string[] args)
        {
            double processTime = 0.0;
            double wait = 0.0;
            FontDescription fontDescription;
            SharpDX.Rectangle fontDimension;
            SharpDX.Direct3D9.Font font;
            string displayText;

            DateTime lastTime = DateTime.Now;
            var form = new RenderForm("AZUSA E-mote Sample - NekoSprite");
            
            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;

            form.TopMost = true;
            
            form.Width = 1280;
            form.Height = 720;

            Emote e = new Emote(form.Handle, 1280, 720);
            e.EmoteInit("dx_e-mote3.0バニラ私服b_鈴あり.psb");

            var device = new Device(new IntPtr(e.D3Device));
            
            // Initialize the Font
            fontDescription = new FontDescription()
            {
                Height = 72,
                Italic = false,
                CharacterSet = FontCharacterSet.Default,
                FaceName = "微软雅黑",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.ClearType,
                Weight = FontWeight.Bold
            };
            
            font = new Font(device, fontDescription);

            displayText = "NekoSprite\nProject AZUSA © 2015";

            // Measure the text to display
            fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, form.Height * 2 / 3, form.Width, form.Height /3), FontDrawFlags.Center | FontDrawFlags.VerticalCenter);

            e.EmoteOffsetScale(0.5f);
            RenderLoop.Run(form, () =>
            {
                e.EmoteUpdate((float)processTime);
                device.Clear(ClearFlags.Target, Color.Wheat, 1.0f, 0);

                device.BeginScene();

                e.EmoteDraw();
                
                font.DrawText(null, displayText, fontDimension, FontDrawFlags.Center | FontDrawFlags.VerticalCenter, SharpDX.Color.Black);

                device.EndScene();
                device.Present();

                wait = elaspedTime;
                processTime = 0;
                DateTime current;
                while (wait > 0)
                {
                    current = DateTime.Now;
                    double elapsed = (current - lastTime).TotalMilliseconds;
                    wait = wait - elapsed;
                    processTime = processTime + elapsed;
                    lastTime = current;
                    Thread.Sleep(1);
                }

            });
        }
    }
}
