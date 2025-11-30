using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class ConfiguracionPage : ContentPage
{
	public ConfiguracionPage(ConfiguracionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
