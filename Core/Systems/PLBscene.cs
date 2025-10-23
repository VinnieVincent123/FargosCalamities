using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasEternalBoss.Core.Systems
{
    public class PLBscene : ModSystem
    {
        private static float flashAlpha;
        private static int flashDir;

        public static void TriggerFlash(float duration = 60f)
        {
            flashAlpha = 0f;
            flashDir = 1;
            _fadeSpeed = 1f / duration;
        }

        private static float _fadeSpeed;

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (flashAlpha <= 0f) return;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.White * flashAlpha);
            spriteBatch.End();

            flashAlpha += flashDir * _fadeSpeed;
            if (flashAlpha >= 1f)
            {
                flashDir = -1;
            }    
            else if (flashAlpha <= 0f)
            {
                flashAlpha = 0f;
                flashDir = 0;
            }
        }

        public override void ModifyScreenPosition()
        {
            if (flashAlpha > 0f)
            {
                Main.GameViewMatrix.Zoom = new Vector2(1f - flashAlpha * 0.2f);
            }
            else
            {
                Main.GameViewMatrix.Zoom = Vector2.One;
            }
        }
    }
}