using Layout;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BasicWindow.Resources
{
    class TextRenderer : IRenderable
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             1f,  0, 0.0f, 1.0f, 0.0f, // top right
             1f, -1f, 0.0f, 1.0f, 1.0f, // bottom right
            0f, -1f, 0.0f,  0, 1.0f, // bottom left
            0f, 0f, 0.0f, 0, 0.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private Texture _texture;

        public static Point letterDimensionRatio =new Point(1, 2);

        private Point location = new Point(0, 0);
        private Color drawColor = new Color(255, 255, 255, 255);
        private float drawSize = 10;
        private string textToRender = "";

        // We create a double to hold how long has passed since the program was opened.
        private double _time;

        public TextRenderer()
        {

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Resources/shader.vert", "Resources/ShaderText.frag");
            _shader.Use();

            _texture = new Texture("Resources/monofont.png");
            _texture.Use(TextureUnit.Texture0);

            //_texture2 = new Texture("Resources/font_bitmap.png");
            //_texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            //_shader.SetInt("texture1", 1);

            int colorLocation = GL.GetUniformLocation(_shader.Handle, "color");
            GL.Uniform4(colorLocation, new Vector4(drawColor.a, drawColor.r, drawColor.g, drawColor.b));

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        }

        public void update(double time)
        {
            _time = time;
        }

        public void setText(String text)
        {
            this.textToRender = text;
        }

        public void setLocation(Point location)
        {
            this.location = location;
        }

        public void setColor(Color color)
        {
            drawColor = color;
        }

        public void setTextSize(double size)
        {
            drawSize = (float)size;
        }

        public void render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            int count = 0;
            if (textToRender == null) return;
            foreach (char letter in textToRender)
            {

                _shader.Use();
                _texture.Use(TextureUnit.Texture0);

                GL.BindVertexArray(_vertexArrayObject);

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.Enable(EnableCap.Blend);

                float width = (float)(drawSize * letterDimensionRatio.X);
                float height = (float)(drawSize * letterDimensionRatio.Y);

                float x = (float)((location.X + ((width) * count)) / width);
                float y = -(float)(location.Y / (float)height);

                Matrix4 pos = Matrix4.CreateTranslation(x, y, 0.0f);
                Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(0));
                Matrix4 scale = Matrix4.CreateScale(width, height, 0.5f);
                Matrix4 model = pos * scale * rotation;

                _shader.SetMatrix4("projection", projectionMatrix);
                _shader.SetMatrix4("view", viewMatrix);
                _shader.SetMatrix4("model", model);

                int colorLocation = GL.GetUniformLocation(_shader.Handle, "color");
                GL.Uniform4(colorLocation, new Vector4( drawColor.r /255.0f, drawColor.g/255.0f, drawColor.b/255.0f, drawColor.a/255.0f));

                int indexLocation = GL.GetUniformLocation(_shader.Handle, "index");

                int letterImageIndex = Encoding.ASCII.GetBytes(new char[] { letter })[0] - 32;

                GL.Uniform1(indexLocation, letterImageIndex);

                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                count++;
            }
        }

        public void destroy()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            GL.DeleteTexture(_texture.Handle);
        }

    }
}
