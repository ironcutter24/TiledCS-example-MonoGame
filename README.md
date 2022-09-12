# TiledCS example in MonoGame

Example on how to display [Tiled](https://www.mapeditor.org/) maps with [TiledCS](https://github.com/TheBoneJarmer/TiledCS) in [MonoGame](https://www.monogame.net/) 3.8.1

Made with `MonoGame Cross-Platform Desktop Application (OpenGL)` template.

![Screenshot_1](https://user-images.githubusercontent.com/33135141/189741169-48ac875a-888f-4303-ac6e-a1468378dcd8.png)
 
## Important notes
- .tmx and .tsx files are <b>not</b> supposed to be built with the content pipeline.<br/>
Just add them to the project and set `Copy to output directory` property to `Copy if newer` in the solution explorer.
 
- TiledCS does <b>not</b> support embedded tilesets yet, so make sure `Embed in map` is false when adding new tilesets in Tiled.
 
 
Thanks to [Temeez](https://github.com/Temeez) for getting me started with this [example](https://github.com/Temeez/TiledCS-MonoGame-Example).<br/>
Tileset used is [Cavernas](https://adamatomic.itch.io/cavernas) by [Adam Saltsman](https://adamatomic.itch.io/).<br/>

