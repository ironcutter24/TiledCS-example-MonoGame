using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledCS;  // dotnet add package tiledcs

namespace TiledCS_example_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TiledMap map;
        private Dictionary<int, TiledTileset> tilesets;
        private Texture2D tilesetTexture;
        private TiledLayer collisionLayer;

        private Rectangle? debugRect;

        const int scaleFactor = 3;
        private Matrix transformMatrix;

        [Flags]
        enum XFRM
        {
            None = 0,
            Horizontal = 1 << 0,
            Vertical = 1 << 1,
            Diagonal = 1 << 2,

            Rotate90 = Diagonal | Horizontal,
            Rotate180 = Horizontal | Vertical,
            Rotate270 = Vertical | Diagonal,

            VerticalAndRotate90 = Horizontal | Vertical | Diagonal,
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            transformMatrix = Matrix.CreateScale(scaleFactor, scaleFactor, 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set the "Copy to Output Directory" property of these two files to `Copy if newer`
            // by clicking them in the solution explorer.
            map = new TiledMap(Content.RootDirectory + "\\cavernasMap.tmx");
            tilesets = map.GetTiledTilesets(Content.RootDirectory + "/"); // DO NOT forget the / at the end

            // Load "exampleTileset.xnb" file, which is the result of building
            // the image file with "Content.mgcb"
            tilesetTexture = Content.Load<Texture2D>("cavesofgallet_tiles");

            // Retrieving objects or layers can be done using Linq or a for loop
            collisionLayer = map.Layers.First(l => l.name == "Ground");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Get mouse position on screen
            var mousePos = Mouse.GetState().Position.ToVector2();

            // Check if mouse is in the bounds of a Tiled object
            debugRect = null;
            foreach (var obj in collisionLayer.objects)
            {
                var objRect = new Rectangle((int)obj.x, (int)obj.y, (int)obj.width, (int)obj.height);
                if (objRect.Contains(mousePos / scaleFactor))
                {
                    debugRect = objRect;
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);  // Set samplerState to null to work with high res assets

            var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            foreach (var layer in tileLayers)
            {
                for (var y = 0; y < layer.height; y++)
                {
                    for (var x = 0; x < layer.width; x++)
                    {
                        var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                        var gid = layer.data[index]; // The tileset tile index
                        var tileX = x * map.TileWidth;
                        var tileY = y * map.TileHeight;

                        // Gid 0 is used to tell there is no tile set
                        if (gid == 0)
                        {
                            continue;
                        }

                        // Helper method to fetch the right TieldMapTileset instance
                        // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                        var mapTileset = map.GetTiledMapTileset(gid);

                        // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                        var tileset = tilesets[mapTileset.firstgid];

                        // Use the connection object as well as the tileset to figure out the source rectangle
                        var rect = map.GetSourceRect(mapTileset, tileset, gid);

                        // Create destination and source rectangles
                        var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                        var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);


                        // You can use the helper methods to get useful information to generate maps
                        XFRM tileTrs = XFRM.None;
                        if (map.IsTileFlippedHorizontal(layer, x, y)) tileTrs |= XFRM.Horizontal;
                        if (map.IsTileFlippedVertical(layer, x, y)) tileTrs |= XFRM.Vertical;
                        if (map.IsTileFlippedDiagonal(layer, x, y)) tileTrs |= XFRM.Diagonal;

                        //tileTrs = XFRM.VerticalAndRotate90;

                        SpriteEffects effects = SpriteEffects.None;
                        double rotation = 0f;
                        switch (tileTrs)
                        {
                            case XFRM.Horizontal:   effects = SpriteEffects.FlipHorizontally;   break;
                            case XFRM.Vertical:     effects = SpriteEffects.FlipVertically;     break;

                            case XFRM.Rotate90:
                                rotation = Math.PI * .5f;
                                destination.X += map.TileWidth;
                                break;

                            case XFRM.Rotate180:
                                rotation = Math.PI;
                                destination.X += map.TileWidth;
                                destination.Y += map.TileHeight;
                                break;

                            case XFRM.Rotate270:
                                rotation = Math.PI * 3 / 2;
                                destination.Y += map.TileHeight;
                                break;

                            case XFRM.VerticalAndRotate90:
                                effects = SpriteEffects.FlipVertically;
                                rotation = Math.PI * .5f;
                                destination.X += map.TileWidth;
                                break;

                            default:
                                break;
                        }


                        // Render sprite at position tileX, tileY using the rect
                        _spriteBatch.Draw(tilesetTexture, destination, source, Color.White, (float)rotation, Vector2.Zero, effects, 0);
                    }
                }
            }

            // If mouse is over a collider, display its bounds
            if (debugRect != null)
            {
                Texture2D _texture = new Texture2D(GraphicsDevice, 1, 1);
                _texture.SetData(new Color[] { Color.Green });

                _spriteBatch.Draw(_texture, (Rectangle)debugRect, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}