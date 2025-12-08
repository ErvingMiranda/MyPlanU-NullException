using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class AmigosPage : ContentPage
{
	public AmigosPage(AmigosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AmigosViewModel vm)
        {
            await vm.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
