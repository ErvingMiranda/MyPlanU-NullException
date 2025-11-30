using System.Collections.Specialized;
using MyPlanU.App.ViewModels;

namespace MyPlanU.App.Pages;

public partial class PromptyPage : ContentPage
{
	public PromptyPage(PromptyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        if (viewModel.Mensajes is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += OnMensajesCollectionChanged;
        }
	}

    private void OnMensajesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // Scroll to the last item
            var count = MessagesCollectionView.ItemsSource.Cast<object>().Count();
            if (count > 0)
            {
                MessagesCollectionView.ScrollTo(count - 1, position: ScrollToPosition.End, animate: true);
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PromptyViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
