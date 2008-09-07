using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Shaghal
{
    /// <summary>
    /// Manage a list of listener to the given interface
    /// </summary>
    public abstract class Notifier<T> : GameComponent
    {
        private List<T> _listeners = new List<T>();

        public Notifier(Game game) : base(game)
        {}

        /// <summary>
        /// This method is called for each listener during the Update
        /// </summary>
        /// <param name="listener">listenr to notify if needed</param>
        public abstract void Notify(T listener);

        public void AddListener(T listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(T listener)
        {
            _listeners.Remove(listener);
        }

        public override void Update(GameTime gameTime)
        {
            sendNotification();
            base.Update(gameTime);
        }

        /// <summary>
        /// Call Notify for each listener
        /// </summary>
        private void sendNotification()
        {
            foreach (T l in _listeners)
                Notify(l);
        }
        
    }
}
