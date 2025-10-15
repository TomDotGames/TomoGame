using Microsoft.Xna.Framework.Graphics;


namespace TomoGame.Core.SceneGraph
{
    public interface IUpdatable
    {
        void Update(float flDeltaTime);
    }

    public interface IRenderable
    {
        void Render(float flDeltaTime, SpriteBatch spriteBatch);
    }

    public class Component
    {
    }
}
