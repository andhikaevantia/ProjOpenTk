using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;



namespace Pertemuan1
{
    static class Constants
    {
        public const string path = "../../../Shaders/";
    }
    internal class Window : GameWindow
    {
        Asset3d[] _object3d = new Asset3d[1];
        double _time;
        float degr = 0;
        Camera _camera;
        bool _firstMove = true;
        Vector2 _lastPos;
        Vector3 _objecPost = new Vector3(0.0f, 0.0f, 0.0f);
        float _rotationSpeed = 1f;

        Asset3d LightObject = new Asset3d();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }
        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //ganti background
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            LightObject.createBoxVertices(1.2f, 1.0f, 2.0f, 0.5f);
            LightObject.load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            _camera = new Camera(new Vector3(0, 0, 1), Size.X / Size.Y);
            _object3d[0] = new Asset3d();
            _object3d[0].createBoxVertices2(0.0f, 0.0f, 0, 0.5f);
            //_object3d[0].createEllipsoid2(0.2f, 0.2f, 0.2f, 0.0f, 0.0f, 0.0f, 72, 24);
            //_object3d[0].createEllipsoid(0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f);
            _object3d[0].addChild(0.5f, 0.5f, 0.0f, 0.25f);
            _object3d[0].load_withnormal(Constants.path + "shaderwithNormal.vert", Constants.path + "objectnew.frag",Size.X,Size.Y);
            


            CursorGrabbed = true;

            

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            //
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //_time += 9.0 * args.Time;
            Matrix4 temp = Matrix4.Identity;
            //temp = temp * Matrix4.CreateTranslation(0.5f, 0.5f, 0.0f);
            //degr += MathHelper.DegreesToRadians(20f);
            //temp = temp * Matrix4.CreateRotationX(degr);
            //_object3d[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[1], 0);
            //_object3d[0].Child[0].rotatede(_object3d[0].Child[0]._centerPosition, _object3d[0].Child[0]._euler[0], 1);

            _object3d[0].render(0,_time,temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[0].setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f));
            _object3d[0].setSpecularDiffuseVariable(LightObject._centerPosition, _camera.Position);


            _object3d[0].resetEuler();
            LightObject.render(3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            float cameraSpeed = 0.5f;
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)args.Time;
            }
            var mouse = MouseState;
            var sensitivity = 0.2f;
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }

            if (KeyboardState.IsKeyDown(Keys.N))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objecPost;
                _camera.Yaw += _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objecPost, _rotationSpeed).ExtractRotation());
                _camera.Position += _objecPost;

                _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
            }
            if (KeyboardState.IsKeyDown(Keys.Comma))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objecPost;
                _camera.Yaw -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objecPost, -_rotationSpeed).ExtractRotation());
                _camera.Position += _objecPost;

                _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
            }
            if (KeyboardState.IsKeyDown(Keys.K))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objecPost;
                _camera.Pitch -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objecPost, _rotationSpeed).ExtractRotation());
                _camera.Position += _objecPost;
                _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
            }
            if (KeyboardState.IsKeyDown(Keys.M))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objecPost;
                _camera.Pitch += _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objecPost, -_rotationSpeed).ExtractRotation());
                _camera.Position += _objecPost;
                _camera._front = -Vector3.Normalize(_camera.Position - _objecPost);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Left)
            {
                float _x = (MousePosition.X - Size.X / 2) / (Size.X / 2);
                float _y = -(MousePosition.Y - Size.Y / 2) / (Size.Y / 2);

                Console.WriteLine("x = " + _x + "y = " + _y);
            }
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            Console.WriteLine("Offset Y: " + e.OffsetY);
            Console.WriteLine("Offset X: " + e.OffsetX);
            _camera.Fov = _camera.Fov - e.OffsetY;
        }

    }
}
