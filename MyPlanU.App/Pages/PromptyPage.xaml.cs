using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class PromptyPage : ContentPage
{
	public PromptyPage(PromptyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
