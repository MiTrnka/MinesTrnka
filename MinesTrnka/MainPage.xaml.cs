using MinesTrnka.ViewModels;

namespace MinesTrnka;

public partial class MainPage : ContentPage
{
    BoardViewModel boardViewModel;
    public MainPage()
    {
        InitializeComponent();
        boardViewModel = new BoardViewModel(MineGrid);
        BindingContext = boardViewModel;
        boardViewModel.StartNewGame(); // spuštění nové hry při startu
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new SettingPage(boardViewModel));
    }
}
