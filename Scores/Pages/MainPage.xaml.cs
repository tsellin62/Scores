using Scores.PageModels;

namespace Scores.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Set the BindingContext to the ViewModel
        BindingContext = new MainPageModel();
    }
}
