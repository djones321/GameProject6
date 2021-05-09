using Microsoft.Xna.Framework;
using GameProject6.StateManagement;

namespace GameProject6.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {
        public float volume=10;
        OptionsMenuScreen options = new OptionsMenuScreen();
        public Game game;

        public MainMenuScreen(Game game) : base("Main Menu")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var instructionsScreen = new MenuEntry("Controls and Instructions");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            this.game = game;

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            instructionsScreen.Selected += instructionsScreenMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(instructionsScreen);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            volume = options.volume;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(volume, game));//, new CutSceneScreen());
        }

        private void instructionsScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            TextBoxScreen tbs = new TextBoxScreen("WASD keys to move, Space to fly\nMouse to Aim\nClick and hold to shoot\nDefeat the penguin to win!");
            ScreenManager.AddScreen(tbs, e.PlayerIndex);
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(options, e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
