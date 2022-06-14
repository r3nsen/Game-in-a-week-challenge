using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Racing
{
    class GraphicsManager
    {
        GraphicsDevice gd;
        VertexPositionColorTexture[] vertex = new VertexPositionColorTexture[4];
        short[] index = new short[6];
        int vertexCount, indexCount;
        public Effect fx;

        Matrix view, projection;

        Color col = new Color(200, 200, 200);

        public Vector2 sx, sy, sw, sh;

        public GraphicsManager(GraphicsDevice _gd)
        {
            gd = _gd;
            sx = new Vector2(0, 0);
            sy = new Vector2(1, 0);
            sw = new Vector2(0, 1);
            sh = new Vector2(1, 1);
        }
        public void begin(float x, float y, float w, float h)
        {
            Vector3 pos = new Vector3(0, 0, 1);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            Matrix.CreateLookAt(ref pos, ref target, ref up, out view);
            Matrix.CreateOrthographicOffCenter(x, x + w, y + h, y, 0, 100, out projection);
            fx.Parameters["WorldViewProjection"].SetValue(view * projection);
        }
        public void draw(float x, float y, float w, float h, float cx = .5f, float cy = .5f, float rot = 0)
        {
            EnsureSpace(4, 6);

            index[indexCount++] = (short)(vertexCount + 0);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 2);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 3);
            index[indexCount++] = (short)(vertexCount + 2);

            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), col, sx);
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), col, sy);
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), col, sw);
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), col, sh);

            Matrix world = Matrix.CreateScale(new Vector3(w, h, 1))
                         * Matrix.CreateTranslation(new Vector3(-cx * w, -cy * h, 0))
                         * Matrix.CreateRotationZ(rot)
                         * Matrix.CreateTranslation(new Vector3(x, y, 0));

            for (int i = vertexCount - 4; i < vertexCount; i++)
                Vector3.Transform(ref vertex[i].Position, ref world, out vertex[i].Position);
        }
        public void flush()
        {
            gd.BlendState = BlendState.NonPremultiplied;
            fx.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);
            vertexCount = indexCount = 0;
        }
        public void flushPista()
        {
            gd.BlendState = BlendState.NonPremultiplied;
            fx.Techniques["Pista"].Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);
            vertexCount = indexCount = 0;
        }
        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (vertexCount + vertexSpace >= vertex.Length)
                Array.Resize(ref vertex, vertex.Length + vertexSpace);
            if (indexCount + indexSpace >= index.Length)
                Array.Resize(ref index, index.Length + indexSpace);
        }
    }
}
