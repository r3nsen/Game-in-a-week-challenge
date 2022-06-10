using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

using static jigsaw.Game1;

namespace jigsaw
{
    internal class GraphicsManager
    {
        private GraphicsDevice gd;
        Matrix view, projection;
        VertexPositionColorTexture[] vertex;
        short[] index;
        int vertexCount, indexCount;

        VertexPositionColorTexture[] bvertex;
        short[] bindex;
        int bvertexCount, bindexCount;

        public Effect jigsawFx;
        public Texture2D tex;

        public GraphicsManager(GraphicsDevice _gd)
        {
            gd = _gd;
            
            vertex = new VertexPositionColorTexture[6];
            index = new short[4];

            bvertex = new VertexPositionColorTexture[6];
            bindex = new short[4];

            gd.BlendState = BlendState.NonPremultiplied;
            gd.RasterizerState = RasterizerState.CullNone;
            gd.SamplerStates[0] = SamplerState.PointClamp;
            tex = new Texture2D(gd, 400, 400);
        }

        public void begin(float x, float y, float w, float h)
        {
            Vector3 pos = new Vector3(0, 0, 1);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            Matrix.CreateLookAt(ref pos, ref target, ref up, out view);
            Matrix.CreateOrthographicOffCenter(x, x + w, y + h, y, 0, 100, out projection);
        }
        public void draw(Grid g)
        {
            EnsureSpace(6, 4);
            index[indexCount++] = (short)(vertexCount + 0);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 2);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 3);
            index[indexCount++] = (short)(vertexCount + 2);

            Color col = new Color(g.id / 255f, g.side / 255f, 0);//Color.White;//new Color(75, 60, 90);

            float w = g.rt.X - g.lt.X;
            float h = g.rb.Y - g.rt.Y;

            Vector2 lt = g.lt + new Vector2(-w * .0f, -h * .0f);
            Vector2 lb = g.lb + new Vector2(-w * .0f, +h * .0f);
            Vector2 rt = g.rt + new Vector2(-w * .0f, -h * .0f);
            Vector2 rb = g.rb + new Vector2(-w * .0f, +h * .0f);



            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), col, lt); // new Vector2(0, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), col, lb); // new Vector2(0, 1));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), col, rt); // new Vector2(1, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), col, rb); // new Vector2(1, 1));

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                          * Matrix.CreateScale(new Vector3(1 / .6f, 1 / .6f, 1))
                         * Matrix.CreateScale(new Vector3(g.w, g.h, 1))
                         * Matrix.CreateTranslation(new Vector3(g.x, g.y, 0));
            for (int i = vertexCount - 4; i < vertexCount; i++)
                Vector3.Transform(ref vertex[i].Position, ref world, out vertex[i].Position);
            bdraw(g);
        }

        public void bdraw(Grid g)
        {
            bEnsureSpace(6, 4);
            bindex[bindexCount++] = (short)(bvertexCount + 0);
            bindex[bindexCount++] = (short)(bvertexCount + 1);
            bindex[bindexCount++] = (short)(bvertexCount + 2);
            bindex[bindexCount++] = (short)(bvertexCount + 1);
            bindex[bindexCount++] = (short)(bvertexCount + 3);
            bindex[bindexCount++] = (short)(bvertexCount + 2);

            Color col = new Color(g.id / 255f, g.side / 255f, 0);//Color.White;//new Color(75, 60, 90);

            float w = g.rt.X - g.lt.X;
            float h = g.rb.Y - g.rt.Y;

            Vector2 lt = g.lt + new Vector2(-w * .0f, -h * .0f);
            Vector2 lb = g.lb + new Vector2(-w * .0f, +h * .0f);
            Vector2 rt = g.rt + new Vector2(-w * .0f, -h * .0f);
            Vector2 rb = g.rb + new Vector2(-w * .0f, +h * .0f);



            bvertex[bvertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), col, lt); // new Vector2(0, 0));
            bvertex[bvertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), col, lb); // new Vector2(0, 1));
            bvertex[bvertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), col, rt); // new Vector2(1, 0));
            bvertex[bvertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), col, rb); // new Vector2(1, 1));

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                          * Matrix.CreateScale(new Vector3(1 / .6f, 1 / .6f, 1))
                         * Matrix.CreateScale(new Vector3(g.w, g.h, 1))
                         * Matrix.CreateTranslation(new Vector3(g.ox, g.oy, 0));
            for (int i = bvertexCount - 4; i < bvertexCount; i++)
                Vector3.Transform(ref bvertex[i].Position, ref world, out bvertex[i].Position);
        }

        public void drawBack()
        {
            jigsawFx.Parameters["WorldViewProjection"].SetValue(view * projection);
            jigsawFx.CurrentTechnique.Passes[1].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, bvertex, 0, bvertexCount, bindex, 0, bindexCount / 3);
            bvertexCount = bindexCount = 0;
        }
        public void flush()
        {
            drawBack();

            jigsawFx.Parameters["WorldViewProjection"].SetValue(view * projection);
            jigsawFx.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);
            vertexCount = indexCount = 0;
        }
        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (indexCount + indexSpace >= index.Length)
                Array.Resize(ref index, index.Length * 2);
            if (vertexCount + vertexSpace >= vertex.Length)
                Array.Resize(ref vertex, vertex.Length * 2);
        }
        void bEnsureSpace(int indexSpace, int vertexSpace)
        {
            if (bindexCount + indexSpace >= bindex.Length)
                Array.Resize(ref bindex, bindex.Length * 2);
            if (bvertexCount + vertexSpace >= bvertex.Length)
                Array.Resize(ref bvertex, bvertex.Length * 2);
        }
    }
}