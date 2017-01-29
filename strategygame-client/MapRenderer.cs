using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Utilities.Png;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using strategygame_common;

namespace strategygame_client
{
    public class MapRenderer
    {
        public Camera Camera;
        private Map Map;
        public Dictionary<MapCellType, Texture2D> BaseTextures;
        public Dictionary<string, Texture2D> MapEntityTextures;

        private ContentManager Content;

        public MapRenderer(ContentManager contentManager, Map Map, int ScreenWidth, int ScreenHeight)
        {
            Camera = new Camera(50, ScreenWidth, ScreenHeight, Map.Width, Map.Height);
            BaseTextures = new Dictionary<MapCellType, Texture2D>();
            this.Map = Map;
            this.Content = contentManager;
            MapEntityTextures = new Dictionary<string, Texture2D>();
            BaseTextures.Add(MapCellType.Flatland, contentManager.Load<Texture2D>("Map/BaseTextures/Flatland"));
            BaseTextures.Add(MapCellType.Mountain, contentManager.Load<Texture2D>("Map/BaseTextures/Mountain"));
            BaseTextures.Add(MapCellType.Hills, contentManager.Load<Texture2D>("Map/BaseTextures/Hills"));
            BaseTextures.Add(MapCellType.Water, contentManager.Load<Texture2D>("Map/BaseTextures/Water"));
        }

        public void Update()
        {
            Camera.Update();
        }        

        private Vector2 Unproject(Vector2 A)
        {
            return Camera.Unproject(A);
        }

        public float UnprojectDistance(Vector2 A, Vector2 B)
        {
            return Vector2.Distance(Camera.Unproject(A), Camera.Unproject(B));
        }

        public Vector2 getClickedWorldPosition(Point clickPos)
        {
            return Camera.Unproject(new Vector2(clickPos.X, clickPos.Y));
        }

        public Point getClickedHex(Point clickPos)
        {
            return Camera.getClickedHex(new Vector2(clickPos.X, clickPos.Y));
        }

        public int? getClickedEntity(ref Dictionary<int, IEntity> Entities, Point MousePoint)
        {
            int? toReturn = null;
            Vector2 MouseVector = new Vector2(MousePoint.X, MousePoint.Y);
            Vector2 MapCoordinates = Camera.Unproject(MouseVector);
            
            //TODO: Sort
            foreach (KeyValuePair<int,IEntity> entity in Entities)
            {
                if (entity.Value is IMapComponent)
                {
                    IMapComponent MapComponent = (IMapComponent)entity.Value;

                    if(Vector2.Distance(MapCoordinates, MapComponent.Position + new Vector2(0.5f,0.5f)) < MapComponent.Hitboxsize)
                    {
                        return entity.Key;
                    }
                }
            }

            return toReturn;
        }

        public void setMap(Map Map)
        {
            this.Map = Map;

            Camera.setMapSize(Map.Width, Map.Height);
        }

        public void Draw(Map Map, Dictionary<int, IEntity> MapEntities, ISpriteRenderer spriteRenderer, float layerDepth)
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

                    //get destination rectangle
                    Rectangle cellRect = Camera.getRectangleToDrawCell(x, y);

                    spriteRenderer.Draw(BaseTexture, cellRect, layerDepth);
                }
            }

            foreach(IEntity entity in MapEntities.Values)
            {
                if(entity is IMapComponent)
                {
                    IMapComponent MapComponent = (IMapComponent)entity;
                    
                    //get destination rectangle
                    Rectangle cellRect = Camera.getRectangleToDrawCell((int)MapComponent.Position.X, (int)MapComponent.Position.Y);

                    Color TintColor = Color.White;

                    if (entity is ISelectableMapComponent && (entity as ISelectableMapComponent)?.isSelected == true)
                        TintColor = Color.Blue;

                    string FullName = MapComponent.GetType().ToString();
                    string TypeName = FullName.Substring(FullName.LastIndexOf(".") + 1);

                    if (!MapEntityTextures.ContainsKey(TypeName))
                        MapEntityTextures.Add(TypeName, Content.Load<Texture2D>("Map/Entities/" + TypeName));

                    spriteRenderer.Draw(MapEntityTextures[TypeName], cellRect, layerDepth + 0.01f);
                }
            }
        }                
    }
}
