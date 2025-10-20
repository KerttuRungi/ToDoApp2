using ToDoApp2.Data;
using ToDoApp2.ViewModels;

namespace ToDoApp2.Views;

public partial class AllTasksPage : ContentPage
{
    private readonly TaskViewModel _viewModel;
    public AllTasksPage()
	{
		InitializeComponent();
        var dbContext = new DatabaseContext();

        // Initialize and set the ViewModel
        _viewModel = new TaskViewModel(dbContext);

        // Bind the ViewModel to this page
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load tasks when page becomes visible
        await _viewModel.LoadTasksAsync();
    }
    private async void OnCompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ());
    }
    private async void UncompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new());
    }
}