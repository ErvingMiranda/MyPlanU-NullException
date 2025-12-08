using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages
{
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage(RegistroViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}


