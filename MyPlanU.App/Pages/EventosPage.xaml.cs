using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class EventosPage : ContentPage
{
	public EventosPage(EventosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
