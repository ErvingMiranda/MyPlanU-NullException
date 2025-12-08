using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class RecuperarPasswordPage : ContentPage
{
	public RecuperarPasswordPage(RecuperarPasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
