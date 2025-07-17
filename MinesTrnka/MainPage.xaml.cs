using MinesTrnka.ViewModels;

namespace MinesTrnka;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        var boardViewModel = new BoardViewModel(MineGrid);
        BindingContext = boardViewModel;
        boardViewModel.StartNewGame(); // spuštění nové hry při startu
    }
}
