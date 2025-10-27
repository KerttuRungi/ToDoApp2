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

    // ✓ Tasks (võib lihtsalt andmed värskendada)
    private async void CompletedTasksClicked(object sender, EventArgs e)
    {
        await _vm.LoadTasksAsync(); // hoiab Completed vaate värskena
    }


    // ← Back nupp


    async void BackButtonClicked(System.Object sender, System.EventArgs e)
   => Application.Current.MainPage = new NavigationPage(new AllTasksPage());

    // Uncompleted nupp
    private async void UncompletedButtonClicked(object sender, EventArgs e)
    {
       // await Shell.Current.GoToAsync(); // viib avalehele
    }

}
