using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace final_project_PvZ;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int _width;
    private int _height;
    private Texture2D _background;
    
    private Vases _vases;
    
    private Texture2D _vaseTexture;
    private Texture2D _vaseCrackedTexture;
    
    private Texture2D _plantVaseTexture;
    private Texture2D _plantVaseCrackedTexture;
    
    private Texture2D _zombieVaseTexture;
    private Texture2D _zombieVaseCrackedTexture;
    
    private MouseState _previousMouse;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
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
        
        
        _vases = new Vases();

        // TEST: NUMBER OF SPAWN COLUMNS, NUMBER OF PLANT VASES, NUMBER OF ZOMBIE VASES
        _vases.Spawn(7, 8, 10); // 7 columns, 8 plants, 10 zombies
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        
        var mouse = Mouse.GetState();

        // Detect LEFT CLICK (pressed this frame, not last frame)
        if (mouse.LeftButton == ButtonState.Pressed &&
            _previousMouse.LeftButton == ButtonState.Released)
        {
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            _vases.HandleClick(mousePos, _vaseTexture, 0.5f);
        }

        _previousMouse = mouse;

        _vases.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        
        
        _spriteBatch.Begin();
        
        //draw background
        _spriteBatch.Draw(_background, new Rectangle(0, 0, _width, _height), Color.White);
        
        _vases.Draw(
            _spriteBatch,
            _vaseTexture,
            _vaseCrackedTexture,
            _plantVaseTexture,
            _plantVaseCrackedTexture,
            _zombieVaseTexture,
            _zombieVaseCrackedTexture,
            0.65f
        );
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}