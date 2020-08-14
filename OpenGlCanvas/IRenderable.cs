using OpenTK;

namespace BasicWindow
{
    interface IRenderable
    {
        void update(double time);
        void render(Matrix4 viewMatrix, Matrix4 projectionMatrix);
        void destroy();
    }

}
