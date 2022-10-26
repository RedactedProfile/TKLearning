using System;
using ImGuiNET;
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
        ImGuiController imGuiController;
        Primitives.Quad quad = new Primitives.Quad();

        static public void Main()
        {
            using (Game game = new Game(800, 600, "Something"))
            {
                game.Run();
            }
            
        }

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, APIVersion = new Version(4, 6) }) { }

        protected override void OnLoad()
        { 
            base.OnLoad();

            Title += ": OGL: " + GL.GetString(StringName.Version);

            imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            ///
            quad.Load();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            imGuiController.Update(this, (float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            quad.Draw();

            ImGui.ShowDemoWindow();
            imGuiController.Render();
            ImGuiController.CheckGLError("End of frame");

            

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            quad.Update();

            KeyboardState input = KeyboardState;
            if(input.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            imGuiController.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            imGuiController.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            imGuiController.MouseScroll(e.Offset);
        }

        private void Exit()
        {
            Close();
        }
    }

    
}
