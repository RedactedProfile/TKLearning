using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TkLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Game : GameWindow
    {
        Primitives.Quad quad = new Primitives.Quad();

        static public void Main()
        {
            using (Game game = new Game(800, 600, "Something"))
            {
                game.Run();
            }
            
        }

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

        protected override void OnLoad()
        { 
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            ///
            quad.Load();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            quad.Draw();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;
            if(input.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        private void Exit()
        {
            Close();
        }
    }

    
}
