using MyPlanU.App.ViewModels;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.Pages;

[QueryProperty(nameof(Usuario), "Usuario")]
public partial class CrearEventoPage : ContentPage
{
    private readonly CrearEventoViewModel _viewModel;

    public Usuario Usuario
    {
        set => _viewModel.SetUsuario(value);
    }

	public CrearEventoPage(CrearEventoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}
