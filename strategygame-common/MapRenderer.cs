﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Utilities.Png;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class MapRenderer
    {
        public Camera Camera;
        private Map Map;
        public Dictionary<MapCellType, Texture2D> BaseTextures;

        public MapRenderer(ContentManager contentManager, Map Map, int ScreenWidth, int ScreenHeight)
        {
            Camera = new Camera(25, ScreenWidth, ScreenHeight, Map.Width, Map.Height);
            BaseTextures = new Dictionary<MapCellType, Texture2D>();
            this.Map = Map;

            BaseTextures.Add(MapCellType.Flatland, contentManager.Load<Texture2D>("Map/BaseTextures/Flatland"));
            BaseTextures.Add(MapCellType.Mountain, contentManager.Load<Texture2D>("Map/BaseTextures/Mountain"));
            BaseTextures.Add(MapCellType.Hills, contentManager.Load<Texture2D>("Map/BaseTextures/Hills"));
            BaseTextures.Add(MapCellType.Water, contentManager.Load<Texture2D>("Map/BaseTextures/Water"));
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);
        }        

        public Vector2 getClickedHex(Point clickPos)
        {
            return Camera.getClickedHex(clickPos);
        }

        public void setMap(Map Map)
        {
            this.Map = Map;

            Camera.setMapSize(Map.Width, Map.Height);
        }

        public void Draw(Map Map, Dictionary<int, IEntity> MapEntities, SpriteBatch spriteBatch, float layerDepth)
        {
            //Get Rectangle to draw from camera
            Rectangle toDraw = Camera.getRectangleToDraw();
            for(int x = toDraw.Left; x < toDraw.Right; x++)
            {
                for(int y = toDraw.Top; y < toDraw.Bottom; y++)
                {
                    MapCell mapCell = Map.getMapCellAt(x, y);
                    //Get Base texture

                    if(!BaseTextures.ContainsKey(mapCell.CellType))
                    {
                        continue;
                    }

                    Texture2D BaseTexture = BaseTextures[mapCell.CellType];

                    //TODO: Tint according to Fruchtbarkeit

                    //construct destination rectangle
                    Rectangle cellRect = Camera.getRectangleToDrawCell(x, y);

                    spriteBatch.Draw(BaseTexture, cellRect, null, Color.White, 0.0f, new Vector2(), SpriteEffects.None, layerDepth);
                }
            }

        }                
    }
}
