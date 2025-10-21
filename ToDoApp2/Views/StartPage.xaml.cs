namespace ToDoApp2.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        //New page to navigate to goes in the brackets
        await Navigation.PushAsync(new ());
    }
}