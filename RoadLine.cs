using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace NeaPrototype{
    internal class RoadLine : Sprite{

        int scale  = 0;
        public int midpoint = 1280 / 2;
        public override Rectangle Rect{
            get{
                return new Rectangle((int)xPos, (int)pos.Y, scale, scale);
            }
        }

        public RoadLine(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.midpoint = 1280 / 2;
            width = (int)Math.Floor(((pos.Y - 480) * 6.0));
            xPos = (int)Math.Floor(this.midpoint - (width / 2.0));
        }

        public override void moveMidPoint(float xMove){
            this.midpoint = (int)xMove + 640;
            width = (int)Math.Floor((pos.Y - 480) * 1.0);

            float curveFactor = (midpoint - (1280 / 2)) / (1280 / 2.0f);
            float curveStrength = 300;
            float yFactor = Math.Max(0, (pos.Y - 480) / 480.0f);

            xPos = (int)Math.Floor(midpoint - (width / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);

        }

    }


}