using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class CambiarPasswordPage : ContentPage
{
	public CambiarPasswordPage(CambiarPasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
