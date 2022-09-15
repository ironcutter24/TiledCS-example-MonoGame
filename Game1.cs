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

        const int scaleFactor = 3;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);  // Set samplerState to null to work with high res assets

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
                        destination = ScaleRect(destination, scaleFactor);

                        // You can use the helper methods to get useful information to generate maps
                        SpriteEffects effects = SpriteEffects.None;
                        if (map.IsTileFlippedHorizontal(layer, x, y))
                        {
                            effects |= SpriteEffects.FlipHorizontally;
                        }
                        if (map.IsTileFlippedVertical(layer, x, y))
                        {
                            effects |= SpriteEffects.FlipVertically;
                        }

                        // Render sprite at position tileX, tileY using the rect
                        _spriteBatch.Draw(tilesetTexture, destination, source, Color.White, 0f, Vector2.Zero, effects, 0);
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);

            Rectangle ScaleRect(Rectangle rect, int scaleFactor)
            {
                return new Rectangle(rect.X * scaleFactor, rect.Y * scaleFactor, rect.Width * scaleFactor, rect.Height * scaleFactor);
            }
        }
    }
}