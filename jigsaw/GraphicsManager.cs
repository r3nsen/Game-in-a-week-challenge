using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace jigsaw
{
    internal class GraphicsManager
    {
        private GraphicsDevice gd;
        Matrix view, projection;
        VertexPositionColorTexture[] vertex;
        short[] index;
        int vertexCount, indexCount;

        public Effect jigsawFx;
        public Texture2D tex;

        public GraphicsManager(GraphicsDevice _gd)
        {
            gd = _gd;
            vertex = new VertexPositionColorTexture[6];
            index = new short[4];
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
        public void draw(float x, float y, float w, float h)
        {
            EnsureSpace(6, 4);
            index[indexCount++] = (short)(vertexCount + 0);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 2);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 3);
            index[indexCount++] = (short)(vertexCount + 2);

            Color col = new Color(75, 60, 90);

            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), col, new Vector2(0, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), col, new Vector2(0, 1));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), col, new Vector2(1, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), col, new Vector2(1, 1));

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0) * 0)
                         * Matrix.CreateScale(new Vector3(w, h, 1))
                         * Matrix.CreateTranslation(new Vector3(x, y, 0));
            for (int i = vertexCount - 4; i < vertexCount; i++)
                Vector3.Transform(ref vertex[i].Position, ref world, out vertex[i].Position);
        }
        public void flush()
        {
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
    }
}