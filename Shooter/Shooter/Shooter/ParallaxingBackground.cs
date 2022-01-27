// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class ParallaxingBackground
    {
        Texture2D texture;
        Vector2[] positions;
        int speed;
        int screenHeight;

        public void Initialize( ContentManager content, String texturePath, int screenHeight, int speed )
        {
            texture = content.Load<Texture2D>( texturePath );
            this.speed = speed;
            this.screenHeight = screenHeight;

            // Tile the background image, add 2 for buffers above and below the screen
            positions = new Vector2[ screenHeight / texture.Height + 2 ];
            
            // Set the initial positions of the parallaxing background
            for ( int i = 0; i < positions.Length; i++ )
                positions[i] = new Vector2( 0, i * texture.Height - 1 );
        }

        public void Update()
        {
            for ( int i = 0; i < positions.Length; i++ )
            {
                positions[i].Y += speed;

                if ( positions[i].Y >= screenHeight )
                    positions[i].Y = -texture.Height;
            }
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            for ( int i = 0; i < positions.Length; i++ )
            {
                spriteBatch.Draw( texture, positions[i], Color.White );
            }
        }
    }
}
