﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NeaPrototype;

// other than the c# code (.cs) and the image (of sprite), the rest are built in (or part of the monogame framework porject template)
// so for now The code that I've done is Game1.cs, Player.cs, Road.cs, Sprite.cs and Traffic.cs

// -- git update command--
// git add .
// git commit -m "your message"
// git push origin main


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private float Xaccel = 300;
    private float Xspeed = 0;

    List<Sprite> sprites;
    List<Sprite> roads;


    public float playerSpeed;
    Player player;


    public float spawnCounter = 3;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {   
        IsFixedTimeStep = true;
        //TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.PreferredBackBufferHeight = 960;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D texture = Content.Load<Texture2D>("MRS");
        Vector2 startPos;
        startPos.X = 490;
        startPos.Y = 600;
        sprites = new List<Sprite>();
        roads = new List<Sprite>();

        player = new Player(texture, startPos);
        
        for (int i = 0; i < 200; i++){
            roads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 390 + (i * 3))));
        }

        for (int i = 0; i < 200; i++){
            roads.Add(new Road(Content.Load<Texture2D>("whiteLine"), new Vector2(0, 390 + (i * 3))));
        }

        // for (int i = 0; i < 320; i++){
        //     sprites.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 320 - (i * 2))));
        // }
        sprites.Add(player);

        
    }

    protected override void Update(GameTime gameTime)
    {
        Xaccel = player.speed / 10;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
            Exit();
        }
        
        spawnCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * (1 + (playerSpeed / 500f));
        if (spawnCounter > 3- (playerSpeed / 500f)){
            sprites.Add(new Traffic(Content.Load<Texture2D>("FITRS"), new Vector2(640, 390)));
            spawnCounter = 0;
        }

        sprites.RemoveAll(sprite => sprite.Rect.Y > 1000 || sprite.Rect.Y < 370);
        foreach(Sprite sprite in sprites){
            if (sprite is Player playersprite){
                
                float fraction = 10f;

                if (Keyboard.GetState().IsKeyDown(Keys.D) && playersprite.xPos < 500){
                    Xspeed += Xaccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }else if (Keyboard.GetState().IsKeyDown(Keys.A) && playersprite.xPos > -500){
                    Xspeed -=  Xaccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }else if (Xspeed > 0){
                    Xspeed -= (float)gameTime.ElapsedGameTime.TotalSeconds * fraction;
                }else if (Xspeed < 0){
                    Xspeed += (float)gameTime.ElapsedGameTime.TotalSeconds * fraction;
                }

                if (playersprite.xPos > 550 || player.xPos < -550){
                    Xspeed = -Math.Abs(Xspeed) * (playersprite.xPos / Math.Abs(playersprite.xPos));
                }

                playersprite.moveX(Xspeed);

                playerSpeed = playersprite.speed;

                if (playersprite.speed > 0){
                    playersprite.accelerate(-1 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W)){
                    playersprite.accelerate(10 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }else if (Keyboard.GetState().IsKeyDown(Keys.S) && playersprite.speed > 0){
                    playersprite.accelerate(-20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }

            }

            if (sprite.Rect.Intersects(player.Rect) && sprite != player){
                if (Math.Abs(sprite.yPos - player.Rect.Y) < 50 && Math.Abs(sprite.Rect.X - player.Rect.X) < 20){
                    player.accelerate(((player.Rect.Y - sprite.Rect.Y) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    Console.WriteLine("Crash");
                }
            }

            

            sprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, player.xPos);
        
        }

        foreach (Sprite road in roads){
            road.moveMidPoint(-player.xPos);
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState : SamplerState.PointClamp);

        sprites.Sort((a, b) => a.Rect.Y.CompareTo(b.Rect.Y));

        // _spriteBatch.Draw(road, new Rectangle(400, 400, 100, 200), Color.White);
        // _spriteBatch.Draw(mrs, new Rectangle(400, 400, 282, 190), Color.White);
        foreach(Sprite road in roads){
            _spriteBatch.Draw(road.texture, road.Rect, road.colour);
        }
        foreach(Sprite sprite in sprites){
            _spriteBatch.Draw(sprite.texture, sprite.Rect, sprite.colour);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

}
