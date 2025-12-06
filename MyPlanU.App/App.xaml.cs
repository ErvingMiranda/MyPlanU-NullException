
using MyPlanU.Backend.Models;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.System;
#endif

namespace MyPlanU.App;

public partial class App : Application
{
    public static Usuario? CurrentUser { get; set; }

    public App()
    {
        InitializeComponent();

        // Pantalla inicial: Login SIN parámetros
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

#if WINDOWS
        window.HandlerChanged += (sender, args) =>
        {
            if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window winuiWindow)
            {
                if (winuiWindow.Content is Microsoft.UI.Xaml.UIElement content)
                {
                    content.KeyDown += (s, e) =>
                    {
                        if (e.Key == VirtualKey.F11)
                        {
                            ToggleFullScreen(winuiWindow);
                        }
                        else if (e.Key == VirtualKey.Escape)
                        {
                            SetFullScreen(winuiWindow, false);
                        }
                    };
                }
            }
        };
#endif
        return window;
    }

#if WINDOWS
    private void ToggleFullScreen(Microsoft.UI.Xaml.Window window)
    {
        var appWindow = GetAppWindow(window);
        if (appWindow != null)
        {
            if (appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.Default);
            }
            else
            {
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
        }
    }

    private void SetFullScreen(Microsoft.UI.Xaml.Window window, bool isFullScreen)
    {
        var appWindow = GetAppWindow(window);
        if (appWindow != null)
        {
            if (isFullScreen)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
            else if (appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.Default);
            }
        }
    }

    private AppWindow GetAppWindow(Microsoft.UI.Xaml.Window window)
    {
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
        return AppWindow.GetFromWindowId(id);
    }
#endif
}
