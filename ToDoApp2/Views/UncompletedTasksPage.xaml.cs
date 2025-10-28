using ToDoApp2.ViewModels;
using ToDoApp2.Data;

namespace ToDoApp2.Views;

public partial class UncompletedTasksPage : ContentPage
{
    private readonly TaskViewModel _vm;

    public UncompletedTasksPage()
    {
        InitializeComponent();
        _vm = new TaskViewModel(DatabaseContext.Instance); // sama singleton nagu Completed vaates
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadTasksAsync();
    }

    private async void BackButtonClicked(object sender, EventArgs e)
        => Application.Current.MainPage = new NavigationPage(new AllTasksPage());

    private async void CompletedButtonClicked(object sender, EventArgs e)
        => Application.Current.MainPage = new NavigationPage(new CompletedTasksPage());

    private async void MarkCompletedClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is ToDoApp2.Models.Task task)
        {
            task.IsCompleted = true;
            await _vm.UpdateTaskCompletionAsync(task);
        }
    }
}
