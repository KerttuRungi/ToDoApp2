using ToDoApp2.Data;
using ToDoApp2.ViewModels;

namespace ToDoApp2.Views;

public partial class AllTasksPage : ContentPage
{
    private readonly TaskViewModel _viewModel;

    public AllTasksPage()
    {
        InitializeComponent();
        var dbContext = DatabaseContext.Instance; // kasuta singletoni järjekindlalt

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

    // ? Completed Tasks nupp
    private async void CompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CompletedTasksPage());
    }

    // ? Uncompleted Tasks nupp
    private async void UncompletedTasksClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UncompletedTasksPage());
    }

    // Checkbox muutmine (IsCompleted staatus)
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is ToDoApp2.Models.Task task)
        {
            task.IsCompleted = e.Value;
            _ = _viewModel.UpdateTaskCompletionAsync(task); // Fire and forget – UI update kohe
        }
    }
}
