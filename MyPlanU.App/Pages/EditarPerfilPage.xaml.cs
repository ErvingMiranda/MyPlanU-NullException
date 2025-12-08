using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class EditarPerfilPage : ContentPage
{
	public EditarPerfilPage(EditarPerfilViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
