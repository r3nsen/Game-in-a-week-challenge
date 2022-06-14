using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace Racing
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        GraphicsManager gm;

        struct Player
        {
            public float x, y;
            public float acc;
            public float spd, maxspd;
            public float angle;
        }
        Player p;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gm = new GraphicsManager(GraphicsDevice);
            gm.fx = Content.Load<Effect>("fx");
            p = new Player();
            p.acc = .00001f;
            p.spd = 0;
            p.maxspd = .002f;
            p.angle = 0;
            p.x = .25f;
            p.y = .5f;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            p.spd -= p.acc / 2;
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                p.spd += p.acc;
            }
            float d = (.01f / p.maxspd) * p.spd;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                p.angle = (float)((p.angle + d) % (2 * Math.PI));

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                p.angle = (float)((p.angle - d) % (2 * Math.PI));

            p.spd = Math.Clamp(p.spd, 0, p.maxspd);

            float x = (float)Math.Sin(p.angle) * p.spd;
            float y = (float)Math.Cos(p.angle) * p.spd;
            p.x -= x;
            p.y -= y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            int w = _graphics.PreferredBackBufferWidth;
            int h = _graphics.PreferredBackBufferHeight;
            GraphicsDevice.Clear(new Color(20, 20, 20));
            gm.begin(0, 0, w, h);                       

            gm.draw(w / 2, h / 2, 1000, 1000, rot: p.angle, cx: p.x, cy: p.y);
            gm.flushPista();
            gm.draw(w / 2, h / 2, 10, 10);
            gm.flush();
            base.Draw(gameTime);
        }
    }
}
