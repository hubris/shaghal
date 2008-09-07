using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shaghal
{
    public class GlobalSystem
    {
        static private Game _game;
        static private ContentManager _content;

        public static void Init(Game game)
        {
            _game = game;
            //_content = new ContentManager(game.Services);
            _content = new ResourceContentManager(game.Services, ContentResource.ResourceManager);
        }

        static public ContentManager Content
        {
            get { return _content; }
        }

        static public Game Game
        {
            get { return _game; }
        }
    }
}
