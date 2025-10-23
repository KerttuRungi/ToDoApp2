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
    private async void CompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ());
    }
    private async void UncompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new());
    }
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoApp2.Models.Task task)
        {
            task.IsCompleted = e.Value;

            // Fire and forget — UI updates instantly
            _ = _viewModel.UpdateTaskCompletionAsync(task);
        }
    }


}