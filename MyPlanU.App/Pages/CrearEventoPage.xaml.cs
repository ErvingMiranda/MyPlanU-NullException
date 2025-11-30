using MyPlanU.App.ViewModels;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.Pages;

public partial class CrearEventoPage : ContentPage
{
    private readonly CrearEventoViewModel _viewModel;

	public CrearEventoPage(CrearEventoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}
