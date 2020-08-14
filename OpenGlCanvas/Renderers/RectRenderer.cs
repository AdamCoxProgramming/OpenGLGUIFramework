using Layout;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Windows;

namespace BasicWindow.Resources
{
    class RectRenderer : IRenderable 
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             1f,  0, 0.0f, 0.0f, 0.0f, // top right
             1f, -1f, 0.0f, 0.0f, 1.0f, // bottom right
            0f, -1f, 0.0f,  1.0f, 1.0f, // bottom left
            0f, 0f, 0.0f, 1.0f, 0.0f  // top left
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
       
        private Rect renderRect = new Rect(new Size(1, 1));
        private Color drawColor = new Color(0, 0, 0, 0);

        // We create a double to hold how long has passed since the program was opened.
        private double _time;

        public RectRenderer()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Resources/shader.vert", "Resources/shader.frag");
            _shader.Use();

            _texture = new Texture("Resources/container.png");
            _texture.Use(TextureUnit.Texture1);

            //_texture2 = new Texture("Resources/awesomeface.png");
            //_texture2.Use(TextureUnit.Texture1);

            //_shader.SetInt("texture0", 0);
            //_shader.SetInt("texture1", 1);
            
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

        public void setRect(Rect rect, Color color)
        {
            this.renderRect = rect;
            this.drawColor = color;
        }

        public void render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            GL.Disable(EnableCap.Blend);

            Matrix4 pos = Matrix4.CreateTranslation((float)renderRect.Left / (float)renderRect.Width, -(float)renderRect.Top / (float)renderRect.Height, 0.0f);
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(0));
            Matrix4 scale = Matrix4.CreateScale((float)renderRect.Width, (float)renderRect.Height, 0.5f);
            Matrix4 model = pos * scale * rotation;

            _shader.SetMatrix4("projection", projectionMatrix);
            _shader.SetMatrix4("view", viewMatrix);
            _shader.SetMatrix4("model", model);

            int colorLocation = GL.GetUniformLocation(_shader.Handle, "color");
            GL.Uniform4(colorLocation, new Vector4(drawColor.r/255f, drawColor.g/255f, drawColor.b/255f, drawColor.a/255f));

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
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
