using ToDoApp2.ViewModels;
using ToDoApp2.Data;

namespace ToDoApp2.Views;

public partial class CompletedTasksPage : ContentPage
{
    private readonly TaskViewModel _vm;

    public CompletedTasksPage()
    {
        InitializeComponent();
        _vm = new TaskViewModel(DatabaseContext.Instance); // singleton DB
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadTasksAsync();
    }
}
