namespace MinesTrnka
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var navigationPage = new NavigationPage(new MainPage());

            // Nastavení barev pro horní lištu
            navigationPage.BarBackgroundColor = Color.FromArgb("#000000");
            navigationPage.BarTextColor = Colors.White;

            return new Window(navigationPage);
        }
    }
}