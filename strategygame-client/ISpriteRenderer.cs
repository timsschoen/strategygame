using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    /// <summary>
    /// Simple renderer
    /// </summary>
    public interface ISpriteRenderer
    {
        /// <summary>
        /// Render the texture to the given rectangle, respecting the layerdepth.
        /// </summary>
        /// <param name="texture">texture to draw</param>
        /// <param name="drawPosition">position to draw at</param>
        /// <param name="layerDepth">layerdepth, 1f is the topmost layer, 0f the bottom</param>
        void Draw(Texture2D texture, Rectangle drawPosition, float layerDepth);

        /// <summary>
        /// Render a string with the default font at the given position, respecting the layer depth
        /// </summary>
        /// <param name="stringToDraw">texture to draw</param>
        /// <param name="drawPosition">position to draw at</param>
        /// <param name="color">color for the string</param>
        /// <param name="layerDepth">layerdepth, 1f is the topmost layer, 0f the bottom</param>
        void DrawString(string stringToDraw, Vector2 drawPosition, Color color, float layerDepth);

        /// <summary>
        /// Render a rectangle, using the given rectangle for position, width and height, respecting color and layer depth
        /// </summary>
        /// <param name="rectangle">the rectangle to draw the </param>
        /// <param name="color">color of the rectangle</param>
        /// <param name="filled">fills the rectangle if needed</param>
        /// <param name="layerDepth">layerdepth, 1f is the topmost layer, 0f the bottom</param>
        void DrawRectanglePrimitive(Rectangle rectangle, int borderWidth, Color color, bool filled, float layerDepth);

        /// <summary>
        /// Draws longer text, using the drawString function. text larger than fitting in the ractangle will be scaled down,
        /// if too large will be truncated
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        /// <param name="textColor"></param>
        /// <param name="layerDepth"></param>
        void DrawText(string text, Rectangle rectangle, Color textColor, float layerDepth);

        /// <summary>
        /// Begin drawing, to be called before any draw calls
        /// </summary>
        void Begin();


        /// <summary>
        /// end drawing
        /// </summary>
        void End();
    }
}
