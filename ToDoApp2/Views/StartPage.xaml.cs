namespace ToDoApp2.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    
    async void OnGetStartedClicked(System.Object sender, System.EventArgs e)
       => Application.Current.MainPage = new NavigationPage(new AllTasksPage());
}