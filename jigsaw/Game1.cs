using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace jigsaw
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        GraphicsManager gm;

        struct Grid
        {
            public float x, y;
            public float ox, oy;
        }
        Grid[] pieces;

        int W, H;
        int tableW, tableH;
        int pieceW, pieceH;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            gm = new GraphicsManager(GraphicsDevice);
            gm.jigsawFx = Content.Load<Effect>("jigsawFx");

            W = _graphics.PreferredBackBufferWidth;
            H = _graphics.PreferredBackBufferHeight;
            tableW = 5;
            tableH = 5;
            pieceW = 40;
            pieceH = 40;

            int offx = (W - tableW * pieceW) / 2;
            int offy = (H - tableH * pieceH) / 2;

            pieces = new Grid[tableW * tableH];

            for (int j = 0; j < tableH; j++)
            {
                for (int i = 0; i < tableW; i++)
                {
                    pieces[i + j * tableW].ox = pieces[i + j * tableW].x = offx + i * pieceW;
                    pieces[i + j * tableW].oy = pieces[i + j * tableW].y = offy + j * pieceH;
                    //gm.draw(offx + i * pieceW, offy + j * pieceH, pieceW - 1, pieceH - 1);
                }
            }

            // TODO: use this.Content to load your game content here
        }
        bool buttonPressed = false;
        int grabId = -1;
        Vector2 mouseoffset;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    for (int i = 0; i < pieces.Length; i++)
                    {
                        float x = pieces[i].x;
                        float y = pieces[i].y;
                        int mx = Mouse.GetState().X;
                        int my = Mouse.GetState().Y;

                        if (mx > x && mx < x + pieceW && my > y && my < y + pieceH)
                        {
                            grabId = i;

                            mouseoffset.X = pieces[grabId].x - Mouse.GetState().X;
                            mouseoffset.Y = pieces[grabId].y - Mouse.GetState().Y;

                            break;
                        }
                    }
                }
                if (grabId != -1)
                {
                    pieces[grabId].x = Mouse.GetState().X + mouseoffset.X;
                    pieces[grabId].y = Mouse.GetState().Y + mouseoffset.Y;
                }
            }
            else
            {
                buttonPressed = false;
                float dist = 8;
                if (grabId != -1)
                {
                    if (Math.Abs(pieces[grabId].x - pieces[grabId].ox) < dist)
                    {
                        if (Math.Abs(pieces[grabId].y - pieces[grabId].oy) < dist)
                        {
                            pieces[grabId].x = pieces[grabId].ox;
                            pieces[grabId].y = pieces[grabId].oy;
                        }
                    }
                    grabId = -1;
                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 20, 30));
            gm.begin(0, 0, W, H);

            int offx = (W - tableW * pieceW) / 2;
            int offy = (H - tableH * pieceH) / 2;

            for (int j = 0; j < tableH; j++)
            {
                for (int i = 0; i < tableW; i++)
                {
                    gm.draw(pieces[i + j * tableW].x, pieces[i + j * tableW].y, pieceW - 1, pieceH - 1);
                    //gm.draw(offx + i * pieceW,offy + j * pieceH, pieceW - 1, pieceH - 1);
                }
            }

            gm.flush();

            base.Draw(gameTime);
        }
    }
}
