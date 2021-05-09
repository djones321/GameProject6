using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameProject6.StateManagement;


namespace GameProject6.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen, IParticleEmitter
    {
        private ContentManager _content;
        private SpriteFont _gameFont;
        private Game game;

        private Vector2 _playerPosition = new Vector2(100, 100);
        private Vector2 _enemyPosition = new Vector2(100, 100);

        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        private SpriteFont spriteFont;
        private SpriteFont spriteFont2;
        private CharacterSprite dragon;
        private PenguinKing pk;
        //private PenguinKing[] randomPK = new PenguinKing[3];
        private Random rand;
        private MouseState mouseState;
        private double timer;
        private double pkTimer;
        private int fbTicker;
        private int feeshTicker = 0;

        private BallProjectileSprite[] fireballs = new BallProjectileSprite[64];
        private BallProjectileSprite[] feesh = new BallProjectileSprite[64];

        private int health = 5;
        private int pkHealth = 50;
        private bool winFlag = false;

        private int hitMotion = 0;
        private double hitTime = 0;
        private int[] hitDirection = { 0, -1, 0, 1 };
        private int hitNext = 0;
        private float prevT;

        private ExplosionParticleSystem[] explosion = new ExplosionParticleSystem[64];
        private FireworkParticleSystem firework;
        private TrailParticleSystem[] trail = new TrailParticleSystem[64];
        //private PixieParticleSystem[] pixie = new PixieParticleSystem[64];
        private double explosionTimer;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        private Texture2D background;
        private SoundEffect dragonSound;
        private SoundEffect dragonHit;
        private SoundEffect pkSound;
        private SoundEffect pkHit;
        private Song backgroundMusic;
        private float volume;


        public GameplayScreen(float volume, Game game)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);

            this.volume = volume;
            this.game = game;
        }

        // Load graphics content for the game
        public override void Activate()
        {
            rand = new Random();
            //Components = new GameComponentCollection();
            //Game thisGame = new Game();
            //explosion = new ExplosionParticleSystem(thisGame, 80);
            //firework = new FireworkParticleSystem(thisGame, 10);
            //pixie = new PixieParticleSystem(thisGame, this);

            //Position = new Vector2(-100, -100);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            firework = new FireworkParticleSystem(game, 80);
            firework.Visible = false;
            game.Components.Add(firework);

            _gameFont = _content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(2500);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            dragon = new CharacterSprite();


            //rand = new System.Random();
            pk = new PenguinKing(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 102, ScreenManager.GraphicsDevice.Viewport.Height - 176));

            //for(int i = 0; i< randomPK.Length; i++) 
                //randomPK[i] = new PenguinKing(new Vector2(RandomHelper.Next(102, ScreenManager.GraphicsDevice.Viewport.Width - 102), ScreenManager.GraphicsDevice.Viewport.Height - 176));

            //base.Initialize();

            dragon.LoadContent(_content);
            spriteFont = _content.Load<SpriteFont>("bangers");
            spriteFont2 = _content.Load<SpriteFont>("bangers2");

            pk.LoadContent(_content);

            for (int i = 0; i < fireballs.Length; i++)
            {
                fireballs[i] = new BallProjectileSprite(_content, "Fireball");
            }
            for (int i = 0; i < feesh.Length; i++)
            {
                feesh[i] = new BallProjectileSprite(_content, "Feesh");
            }



            for (int i = 0; i < fireballs.Length; i++)
            {
                explosion[i] = new ExplosionParticleSystem(game, 10);
                explosion[i].Visible = false;
                game.Components.Add(explosion[i]);
            }

            for (int i = 0; i<feesh.Length; i++)
            {
                trail[i] = new TrailParticleSystem(game, 10);
                trail[i].Visible = false;
                game.Components.Add(trail[i]);
            }

            background = _content.Load<Texture2D>("SnowBackground");
            dragonSound = _content.Load<SoundEffect>("DragonShoot");
            dragonHit = _content.Load<SoundEffect>("hit");
            pkSound = _content.Load<SoundEffect>("PenguinShoot");
            pkHit = _content.Load<SoundEffect>("hit2");
            backgroundMusic = _content.Load<Song>("AIGeneratedBGM");
            MediaPlayer.Volume = 0.3f * volume/10;
            SoundEffect.MasterVolume = volume / 10;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (health < 1 && !winFlag)
            {
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new EndScreen("You Lost! Try again!\nPress Enter To Return to the Menu", game));
                MediaPlayer.Stop();
            }
            else if (pkHealth < 1)
            {
                winFlag = true;
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new EndScreen("You Won!\nPress Enter To Return to the Menu", game));
                MediaPlayer.Stop();
            }

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                dragon.Update(gameTime);

                if (dragon.position.Y > ScreenManager.GraphicsDevice.Viewport.Height - 64)
                {
                    dragon.grounded = true;
                }
                else dragon.grounded = false;


                dragon.Color = Color.White;

                mouseState = Mouse.GetState();
                timer += gameTime.ElapsedGameTime.TotalSeconds;
                if (mouseState.LeftButton == ButtonState.Pressed && timer > .5f)
                {
                    if (fbTicker > fireballs.Length - 1) fbTicker = 0;
                    fireballs[fbTicker].Shoot(dragon.position, new Vector2(mouseState.X, mouseState.Y), gameTime);
                    dragonSound.Play(.2f, 0, 0);
                    fbTicker++;
                    timer = 0;
                }

                //foreach (BallProjectileSprite fb in fireballs)
                //{
                //    fb.Update(gameTime, dragon.position);
                //}


                pkTimer += gameTime.ElapsedGameTime.TotalSeconds;


                pk.Update(dragon.position, ScreenManager.GraphicsDevice.Viewport.Width);

                if (pk.animations.animationFrame == 0 && pkTimer > 1f && pk.animations.animationType == AnimationsType.Spread)
                {
                    if (feeshTicker + 4 > feesh.Length - 1) feeshTicker = 0;
                    feesh[feeshTicker].Shoot(new Vector2(pk.position.X - 96, pk.position.Y - 16), new Vector2(dragon.position.X, dragon.position.Y - 80), gameTime, 2, new Vector2(32, 18));
                    feesh[feeshTicker + 1].Shoot(new Vector2(pk.position.X - 96, pk.position.Y - 16), new Vector2(dragon.position.X, dragon.position.Y - 20), gameTime, 2, new Vector2(32, 18));
                    feesh[feeshTicker + 2].Shoot(new Vector2(pk.position.X - 96, pk.position.Y - 16), new Vector2(dragon.position.X, dragon.position.Y + 20), gameTime, 2, new Vector2(32, 18));
                    feesh[feeshTicker + 3].Shoot(new Vector2(pk.position.X - 96, pk.position.Y - 16), new Vector2(dragon.position.X, dragon.position.Y + 80), gameTime, 2, new Vector2(32, 18));
                    pkSound.Play(.2f, 0, 0);
                    feeshTicker += 4;
                    pkTimer = 0;
                }
                else if(pk.animations.animationFrame == 0 && pkTimer > .25f && pk.animations.animationType == AnimationsType.Rapid)
                {
                    if (feeshTicker + 1 > feesh.Length - 1) feeshTicker = 0;
                    feesh[feeshTicker].Shoot(new Vector2(pk.position.X - 96, pk.position.Y - 16), new Vector2(dragon.position.X, dragon.position.Y - 80), gameTime, 2, new Vector2(32, 18));
                    pkSound.Play(.2f, 0, 0);
                    feeshTicker += 1;
                    pkTimer = 0;
                }

                //foreach (BallProjectileSprite f in feesh) f.Update(gameTime, pk.position);

                hitTime += gameTime.ElapsedGameTime.TotalSeconds;
                for(int i = 0; i<feesh.Length;i++)
                {
                    feesh[i].Update(gameTime, pk.position);
                    trail[i].Visible = true;
                    trail[i].PlaceTrail(new Vector2(feesh[i].position.X+24, feesh[i].position.Y+8));
                    Velocity = feesh[i].position - Position;
                    Position = feesh[i].position;

                    //pixie[i].Update(gameTime);
                    
                    if (feesh[i].Bounds.CollidesWith(dragon.Bounds) && hitTime > .75 && feesh[i].isVisible)
                    {
                        dragon.Color = Color.Red;
                        feesh[i].isVisible = false;
                        trail[i].Visible = false;
                        feesh[i].position = new Vector2(-1000, -1000);
                        health--;
                        hitMotion+=8;
                        hitTime = 0;
                        dragonHit.Play(.4f, 0, 0);
                    }
                }

                explosionTimer += gameTime.ElapsedGameTime.TotalSeconds;
                for (int i = 0; i<fireballs.Length; i++)
                {
                    fireballs[i].Update(gameTime, dragon.position);
                    if (explosionTimer > .1 && fireballs[i].isVisible)
                    {
                        explosion[i].Visible = true;
                        explosion[i].PlaceExplosion(fireballs[i].position);
                    }

                    if (fireballs[i].Bounds.CollidesWith(pk.HeadBounds) && fireballs[i].isVisible)
                    {
                        firework.Visible = true;
                        firework.PlaceFirework(fireballs[i].position);
                        pk.color = Color.Red;
                        fireballs[i].isVisible = false;
                        explosion[i].Visible = false;
                        fireballs[i].position = new Vector2(-1000, -1000);
                        pkHealth--;
                        pkHit.Play(.1f, 0, 0);
                    }
                }

                if (explosionTimer > .1) explosionTimer = 0;




            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(game), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                var movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                var thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                _playerPosition += movement * 8f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            Matrix transform;

            if (hitMotion != 0)
            {
                if (hitNext > 3) hitNext = 0;

                if (hitDirection[hitNext] == 0)
                {
                    transform = Matrix.CreateTranslation(-prevT, 0, 0);
                    hitNext++;
                }
                else
                {
                    prevT = RandomHelper.NextFloat(20, 50) * hitDirection[hitNext];
                    transform = Matrix.CreateTranslation(prevT, 0, 0);
                    hitNext++;
                }
                hitMotion--;
            }
            else
            {
                transform = new Matrix();
                transform = Matrix.CreateTranslation(0, 0, 0);
            }

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(transformMatrix: transform);

            spriteBatch.Draw(background, new Vector2(-100, 0), null, Color.White, 0, Vector2.Zero, 1.4f, SpriteEffects.None, 0);

            foreach (BallProjectileSprite fb in fireballs) fb.Draw(gameTime, spriteBatch);

            foreach (BallProjectileSprite f in feesh) f.Draw(gameTime, spriteBatch);
            //foreach (PixieParticleSystem p in pixie) p.Draw(gameTime);

            spriteBatch.DrawString(spriteFont, $"Your Health: {health}       Boss Health: {pkHealth}", new Vector2(2, 2), Color.Black);

            //spriteBatch.DrawString(spriteFont, $"PK Head angle: {pk.rotation}", new Vector2(2, 15), Color.Black);

            dragon.Draw(gameTime, spriteBatch);
            pk.Draw(gameTime, spriteBatch);


            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
