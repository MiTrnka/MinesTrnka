using MinesTrnka.ViewModels;

namespace MinesTrnka;

public partial class SettingPage : ContentPage
{
	public SettingPage(BoardViewModel bwm)
	{
		InitializeComponent();
		BindingContext = bwm;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

        Navigation.PopModalAsync();
    }
}