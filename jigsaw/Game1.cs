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

        public struct Grid
        {
            public float x, y, z;
            public float w, h;
            public float ox, oy;
            public Vector2 lt, lb, rt, rb;
            public int id, side;
        }
        Grid[] pieces;

        int W, H;
        int tableW, tableH;
        int pieceW, pieceH;
        Texture2D imagem;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }
        Random rand = new Random(69);
        protected override void LoadContent()
        {
            gm = new GraphicsManager(GraphicsDevice);
            gm.jigsawFx = Content.Load<Effect>("jigsawFx");
            imagem = Content.Load<Texture2D>("img");
            gm.jigsawFx.Parameters["tex"].SetValue(imagem);

            W = _graphics.PreferredBackBufferWidth;
            H = _graphics.PreferredBackBufferHeight;

            tableW = 7;
            tableH = 5;
            pieceW = 80;
            pieceH = 80;

            int offx = (W - tableW * pieceW) / 2;
            int offy = (H - tableH * pieceH) / 2;

            pieces = new Grid[tableW * tableH];

            int imgW = imagem.Width;
            int imgH = imagem.Height;

            gm.jigsawFx.Parameters["size"].SetValue(new Vector2(imgW, imgH));
            // 564 398 - 
            Vector2 imgSize = new Vector2(imgW, imgH);
            int r = 0;
            for (int j = 0; j < tableH; j++)
            {
                for (int i = 0; i < tableW; i++)
                {
                    pieces[i + j * tableW].ox = pieces[i + j * tableW].x = offx + pieceW / 2 + i * pieceW;
                    pieces[i + j * tableW].oy = pieces[i + j * tableW].y = offy + pieceH / 2 + j * pieceH;

                    pieces[i + j * tableW].w = pieceW;
                    pieces[i + j * tableW].h = pieceH;

                    pieces[i + j * tableW].lt = new Vector2((imgW / tableW) * (i + 0), (imgH / tableH) * (j + 0)) / imgSize;
                    pieces[i + j * tableW].lb = new Vector2((imgW / tableW) * (i + 0), (imgH / tableH) * (j + 1)) / imgSize;
                    pieces[i + j * tableW].rt = new Vector2((imgW / tableW) * (i + 1), (imgH / tableH) * (j + 0)) / imgSize;
                    pieces[i + j * tableW].rb = new Vector2((imgW / tableW) * (i + 1), (imgH / tableH) * (j + 1)) / imgSize;

                    pieces[i + j * tableW].id = rand.Next(0, 16);

                    //pieces[i + j * tableW].id &= ~4;

                    if (i - 1 >= 0)
                    {
                        if ((pieces[(i - 1) + j * tableW].id & 4) != 0) pieces[i + j * tableW].id &= ~1;
                        else pieces[i + j * tableW].id |= 1;
                    }
                    if (j - 1 >= 0)
                    {
                        if ((pieces[i + (j - 1) * tableW].id & 8) != 0) pieces[i + j * tableW].id &= ~2;
                        else pieces[i + j * tableW].id |= 2;
                    }

                    pieces[i + j * tableW].side = (j == 0 ? 2 : 0) | (j == (tableH - 1) ? 8 : 0) | (i == 0 ? 1 : 0) | (i == (tableW - 1) ? 4 : 0);
                    //r = (r + 1) % 16;

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
                    for (int i = pieces.Length - 1; i >= 0; --i)
                    {
                        float x = pieces[i].x - pieceW / 2;
                        float y = pieces[i].y - pieceH / 2;
                        int mx = Mouse.GetState().X;
                        int my = Mouse.GetState().Y;

                        if (mx > x && mx < x + pieceW && my > y && my < y + pieceH)
                        {
                            grabId = pieces.Length - 1;

                            Grid temp = pieces[i];
                            for (int j = i + 1; j < pieces.Length; j++)
                                pieces[j - 1] = pieces[j];
                            pieces[^1] = temp;

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
                    gm.draw(pieces[i + j * tableW]);//pieces[i + j * tableW].x, pieces[i + j * tableW].y, pieceW - 1, pieceH - 1);
                    //gm.draw(offx + i * pieceW,offy + j * pieceH, pieceW - 1, pieceH - 1);
                }
            }

            gm.flush();

            base.Draw(gameTime);
        }
    }
}
