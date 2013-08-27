using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using CommunityExpressNS;

namespace SteamworksUnityTestDX
{
    /// <summary>
    /// Simple HelloWorld application using SharpDX.Toolkit.
    /// The purpose of this application is to show the minimal setup of a game.
    /// </summary>
    public class HelloWorldGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private float delta = 0.0f;
        private bool shown = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldGame" /> class.
        /// </summary>
        public HelloWorldGame()
        {
            CommunityExpress.Instance.Initialize();

            CommunityExpress.Instance.Friends.GameOverlayActivated += new Friends.GameOverlayActivatedHandler(Friends_GameOverlayActivated);

            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        void Friends_GameOverlayActivated(bool result)
        {
            if (result)
            {
                shown = true;
            }
        }

        protected override void Initialize()
        {
            Window.Title = "HelloWorld!";
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(GraphicsDevice.BackBuffer, Color.CornflowerBlue);

            if (!shown && gameTime.TotalGameTime.TotalSeconds > 10.0f)
            {
                shown = true;
                CommunityExpress.Instance.Friends.ActivateGameOverlay(EGameOverlay.EGameOverlayCommunity);
            }

            base.Draw(gameTime);

            CommunityExpress.Instance.RunCallbacks();
        }

        static void Main(string[] args)
        {
            HelloWorldGame game = new HelloWorldGame();
            game.Run();
        }
    }
}

