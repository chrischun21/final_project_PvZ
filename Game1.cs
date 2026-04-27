using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;

namespace final_project_PvZ;

public class Game1 : Game
{
    private Random _rand = new Random();

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int _width;
    private int _height;

    private Texture2D _background;

    private Vases _vases;
    private Buttons _buttons;

    private Texture2D _vaseTexture;
    private Texture2D _vaseCrackedTexture;
    private Texture2D _plantVaseTexture;
    private Texture2D _plantVaseCrackedTexture;
    private Texture2D _zombieVaseTexture;
    private Texture2D _zombieVaseCrackedTexture;

    private MouseState _previousMouse;
    private KeyboardState _previousKeyboard;

    // Plant textures
    private Texture2D _bevo;
    private Texture2D _bevoIce;
    private Texture2D _bevoDouble;
    private Texture2D _helmet1;
    private Texture2D _helmet2;
    private Texture2D _helmet3;
    private Texture2D _lightBevo;
    private Texture2D _mine1;
    private Texture2D _mine2;
    private Texture2D _mine3;
    private Texture2D _squashTexture;

    // Zombie textures
    private Texture2D _zombieTexture;
    private Texture2D _bucketZombieTexture;
    private Texture2D _gargantuarTexture;

    private bool _levelSelected = false;

    private bool _gameWon = false;
    private bool _gameLost = false;

    private float _fadeAlpha = 0f;
    private bool _isFading = false;
    private bool _fadeOut = true;
    private Difficulty _pendingDifficulty = Difficulty.None;
    private const float FadeSpeed = 1.5f;
    private Texture2D _fadeTexture;

    private PlantInventory _inventory;
    private SpriteFont _font;

    private List<Zombie> _zombies;
    private List<Plant> _plants;
    private List<Projectile> _projectiles;

    private Texture2D _pixel;

    private PlantType? _selectedPlant = null;
    
    private float _victoryDelayTimer = 0f;
    private const float VictoryDelay = 1.5f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1408;
        _graphics.PreferredBackBufferHeight = 938;

        _width = _graphics.PreferredBackBufferWidth;
        _height = _graphics.PreferredBackBufferHeight;

        _graphics.ApplyChanges();

        _background = Content.Load<Texture2D>("Images/background");

        _vaseTexture = Content.Load<Texture2D>("Images/regular_vase");
        _plantVaseTexture = Content.Load<Texture2D>("Images/plant_vase");
        _zombieVaseTexture = Content.Load<Texture2D>("Images/zombie_vase");

        _vaseCrackedTexture = Content.Load<Texture2D>("Images/regular_vase_cracked");
        _plantVaseCrackedTexture = Content.Load<Texture2D>("Images/plant_vase_cracked");
        _zombieVaseCrackedTexture = Content.Load<Texture2D>("Images/zombie_vase_cracked");

        Texture2D easy = Content.Load<Texture2D>("Images/easy_sign");
        Texture2D medium = Content.Load<Texture2D>("Images/medium_sign");
        Texture2D hard = Content.Load<Texture2D>("Images/hard_sign");

        _zombieTexture = Content.Load<Texture2D>("Images/zombie");
        _bucketZombieTexture = Content.Load<Texture2D>("Images/zombie_bucket");
        _gargantuarTexture = Content.Load<Texture2D>("Images/zombie_giant");

        _font = Content.Load<SpriteFont>("fonts/Arial");

        _buttons = new Buttons(easy, medium, hard);

        _vases = new Vases();
        _zombies = new List<Zombie>();
        _plants = new List<Plant>();
        _projectiles = new List<Projectile>();

        _inventory = new PlantInventory();

        HookVaseEvents();

        _bevo = Content.Load<Texture2D>("Images/bevo");
        _bevoIce = Content.Load<Texture2D>("Images/bevo_ice");
        _bevoDouble = Content.Load<Texture2D>("Images/bevo_double");

        _helmet1 = Content.Load<Texture2D>("Images/helmet");
        _helmet2 = Content.Load<Texture2D>("Images/helmet_2");
        _helmet3 = Content.Load<Texture2D>("Images/helmet_3");

        _lightBevo = Content.Load<Texture2D>("Images/light_bevo");

        _mine1 = Content.Load<Texture2D>("Images/mine_1");
        _mine2 = Content.Load<Texture2D>("Images/mine_2");
        _mine3 = Content.Load<Texture2D>("Images/mine_3");

        _squashTexture = Content.Load<Texture2D>("Images/squash");

        base.Initialize();
    }

    private void HookVaseEvents()
    {
        _vases.OnPlantCollected += (type) =>
        {
            _inventory.AddPlant(type);
        };

        _vases.OnZombieSpawned += (pos, type) =>
        {
            _zombies.Add(new Zombie(pos, type));
        };
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _fadeTexture = new Texture2D(GraphicsDevice, 1, 1);
        _fadeTexture.SetData(new[] { Color.White });

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();

        if (_gameWon || _gameLost)
        {
            if (keyboard.IsKeyDown(Keys.P) && !_previousKeyboard.IsKeyDown(Keys.P))
            {
                RestartLevel();
            }

            if (keyboard.IsKeyDown(Keys.L) && !_previousKeyboard.IsKeyDown(Keys.L))
            {
                ResetToMenu();
            }

            _previousKeyboard = keyboard;
            _previousMouse = mouse;
            return;
        }

        HandlePlantSelection(keyboard);
        HandleMouseClick(mouse);
        HandleFade(dt);

        _buttons.Update(dt);
        _vases.Update(dt);

        foreach (var plant in _plants)
            _projectiles.AddRange(plant.Update(dt, _zombies));

        foreach (var proj in _projectiles)
            proj.Update(dt);

        _projectiles.RemoveAll(p => !p.IsAlive);

        HandleProjectileCollisions();

        _zombies.RemoveAll(z => z.Health <= 0);

        foreach (var z in _zombies)
            z.Update(dt);

        HandleZombieEating(dt);

        _plants.RemoveAll(p => !p.IsAlive);

        _vases.UpdatePlanternReveal(_plants);

        if (_zombies.Any(z => z.Position.X < 150))
            _gameLost = true;

        if (_levelSelected &&
            _vases.AllDestroyed() &&
            _zombies.Count == 0)
        {
            _victoryDelayTimer += dt;

            if (_victoryDelayTimer >= VictoryDelay)
                _gameWon = true;
        }
        else
        {
            _victoryDelayTimer = 0f;
        }

        _previousMouse = mouse;
        _previousKeyboard = keyboard;

        base.Update(gameTime);
    }

    private void RestartLevel()
    {
        _plants.Clear();
        _zombies.Clear();
        _projectiles.Clear();

        _inventory = new PlantInventory();

        _vases.SpawnLevel(_pendingDifficulty);

        _selectedPlant = null;

        _gameWon = false;
        _gameLost = false;
    }

    private void ResetToMenu()
    {
        _levelSelected = false;

        _gameWon = false;
        _gameLost = false;

        _fadeAlpha = 0f;
        _isFading = false;
        _fadeOut = true;
        _pendingDifficulty = Difficulty.None;

        _selectedPlant = null;

        _plants.Clear();
        _zombies.Clear();
        _projectiles.Clear();

        _inventory = new PlantInventory();

        _vases = new Vases();
        HookVaseEvents();
    }

    private void HandlePlantSelection(KeyboardState keyboard)
    {
        HandleSelectKey(keyboard, Keys.D1, PlantType.Peashooter);
        HandleSelectKey(keyboard, Keys.D2, PlantType.SnowPea);
        HandleSelectKey(keyboard, Keys.D3, PlantType.Repeater);
        HandleSelectKey(keyboard, Keys.D4, PlantType.Walnut);
        HandleSelectKey(keyboard, Keys.D5, PlantType.Plantern);
        HandleSelectKey(keyboard, Keys.D6, PlantType.PotatoMine);
        HandleSelectKey(keyboard, Keys.D7, PlantType.Squash);
    }

    private void HandleSelectKey(KeyboardState keyboard, Keys key, PlantType type)
    {
        if (keyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key))
        {
            if (_selectedPlant == type)
                _selectedPlant = null;
            else
                _selectedPlant = type;
        }
    }

    private void HandleMouseClick(MouseState mouse)
    {
        if (mouse.LeftButton != ButtonState.Pressed ||
            _previousMouse.LeftButton != ButtonState.Released)
            return;

        Point mousePoint = new(mouse.X, mouse.Y);

        if (!_levelSelected)
        {
            Difficulty choice = _buttons.HandleClick(mousePoint);

            if (choice != Difficulty.None)
            {
                _pendingDifficulty = choice;
                _isFading = true;
                _fadeOut = true;
            }

            return;
        }

        Vector2 mousePos = new(mouse.X, mouse.Y);

        if (_vases.HandleClick(mousePos, _vaseTexture, 0.65f))
            return;

        if (!_selectedPlant.HasValue)
            return;

        var tile = GetNearestTile(mousePos);

        if (!tile.HasValue)
            return;

        bool occupied = _plants.Any(p => Vector2.Distance(p.Position, tile.Value) < 10f);

        if (!occupied && _inventory.UsePlant(_selectedPlant.Value))
        {
            _plants.Add(new Plant(tile.Value, _selectedPlant.Value));
        }
    }

    private void HandleFade(float dt)
    {
        if (!_isFading) return;

        if (_fadeOut)
        {
            _fadeAlpha += FadeSpeed * dt;

            if (_fadeAlpha >= 1f)
            {
                _fadeAlpha = 1f;

                _vases.SpawnLevel(_pendingDifficulty);

                _levelSelected = true;
                _fadeOut = false;
            }
        }
        else
        {
            _fadeAlpha -= FadeSpeed * dt;

            if (_fadeAlpha <= 0f)
            {
                _fadeAlpha = 0f;
                _isFading = false;
            }
        }
    }

    private void HandleProjectileCollisions()
    {
        foreach (var proj in _projectiles)
        {
            foreach (var z in _zombies)
            {
                Rectangle zombieRect = new((int)z.Position.X, (int)z.Position.Y, 40, 60);

                if (!proj.GetHitbox().Intersects(zombieRect))
                    continue;

                proj.IsAlive = false;
                z.TakeDamage(1);

                if (proj.IsSnow)
                    z.ApplySlow();

                break;
            }
        }
    }

    private void HandleZombieEating(float dt)
    {
        foreach (var z in _zombies)
            z.IsEating = false;

        foreach (var plant in _plants)
        {
            float eaters = 0f;

            foreach (var z in _zombies)
            {
                bool sameLane = Math.Abs(z.Position.Y - plant.Position.Y) < 40f;
                bool touching = Math.Abs(z.Position.X - plant.Position.X) < 50f;

                if (!sameLane || !touching)
                    continue;

                z.IsEating = true;

                if (z.Type == ZombieType.Gargantuar)
                {
                    if (!z.IsSmashing)
                        z.StartSmash(plant);
                }
                else
                {
                    eaters += z.GetEatingMultiplier();
                }
            }

            if (eaters > 0)
                plant.TakeEatingDamage(eaters * dt);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_background, new Rectangle(0, 0, _width, _height), Color.White);

        if (!_levelSelected)
            _buttons.Draw(_spriteBatch);
        else
            _vases.Draw(
                _spriteBatch,
                _vaseTexture,
                _vaseCrackedTexture,
                _plantVaseTexture,
                _plantVaseCrackedTexture,
                _zombieVaseTexture,
                _zombieVaseCrackedTexture,
                _bevo,
                _bevoIce,
                _bevoDouble,
                _helmet1,
                _lightBevo,
                _mine3,
                _squashTexture,
                _zombieTexture,
                _bucketZombieTexture,
                _gargantuarTexture,
                0.65f
            );

        DrawGhostPlant();

        if (_fadeAlpha > 0f)
            _spriteBatch.Draw(_fadeTexture,
                new Rectangle(0, 0, _width, _height),
                Color.Black * _fadeAlpha);

        _inventory.Draw(_spriteBatch, _font);

        foreach (var p in _plants)
            p.Draw(_spriteBatch, _bevo, _bevoIce, _bevoDouble,
                _helmet1, _helmet2, _helmet3,
                _lightBevo, _mine1, _mine2, _mine3, _squashTexture);

        foreach (var z in _zombies)
            z.Draw(_spriteBatch, _zombieTexture, _bucketZombieTexture, _gargantuarTexture, _font);

        foreach (var proj in _projectiles)
            proj.Draw(_spriteBatch, _pixel);

        if (_gameWon || _gameLost)
        {
            string title = _gameWon ? "VICTORY" : "DEFEAT";

            Vector2 titleSize = _font.MeasureString(title);
            Vector2 pSize = _font.MeasureString("Press P to Play Again");
            Vector2 lSize = _font.MeasureString("Press L to Return to Menu");

            _spriteBatch.Draw(
                _fadeTexture,
                new Rectangle(0, 0, _width, _height),
                Color.Black * 0.75f);

            _spriteBatch.DrawString(
                _font,
                title,
                new Vector2(_width / 2f - titleSize.X / 2f, 300),
                Color.White);

            _spriteBatch.DrawString(
                _font,
                "Press P to Play Again",
                new Vector2(_width / 2f - pSize.X / 2f, 400),
                Color.White);

            _spriteBatch.DrawString(
                _font,
                "Press L to Return to Menu",
                new Vector2(_width / 2f - lSize.X / 2f, 460),
                Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawGhostPlant()
    {
        if (!_selectedPlant.HasValue)
            return;

        Texture2D tex = _selectedPlant.Value switch
        {
            PlantType.Peashooter => _bevo,
            PlantType.SnowPea => _bevoIce,
            PlantType.Repeater => _bevoDouble,
            PlantType.Walnut => _helmet1,
            PlantType.Plantern => _lightBevo,
            PlantType.PotatoMine => _mine3,
            PlantType.Squash => _squashTexture,
            _ => _bevo
        };

        SpriteEffects effects =
            _selectedPlant.Value == PlantType.Repeater
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

        var mouse = Mouse.GetState();
        var tile = GetNearestTile(new Vector2(mouse.X, mouse.Y));

        if (!tile.HasValue)
            return;

        bool occupied = _plants.Any(p => Vector2.Distance(p.Position, tile.Value) < 10f);

        Color ghostColor = occupied
            ? Color.Red * 0.5f
            : Color.White * 0.5f;

        Vector2 drawPos = tile.Value + new Vector2(0, 80f);

        _spriteBatch.Draw(
            tex,
            drawPos,
            null,
            ghostColor,
            0f,
            new Vector2(tex.Width / 2f, tex.Height),
            0.8f,
            effects,
            0f
        );
    }

    private Vector2? GetNearestTile(Vector2 mousePos)
    {
        int[] yPositions = { 200, 342, 484, 615, 746 };
        int[] xPositions = { 270, 383, 494, 595, 702, 809, 912, 1024, 1145 };

        float minDist = float.MaxValue;
        Vector2 best = Vector2.Zero;

        foreach (int x in xPositions)
        foreach (int y in yPositions)
        {
            Vector2 tile = new(x, y);
            float dist = Vector2.Distance(mousePos, tile);

            if (dist < minDist)
            {
                minDist = dist;
                best = tile;
            }
        }

        return minDist < 50f ? best : null;
    }
}