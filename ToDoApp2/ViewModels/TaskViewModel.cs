
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDoApp2.Data;
using TaskModel = ToDoApp2.Models.Task;


namespace ToDoApp2.ViewModels
{
    public partial class TaskViewModel : ObservableObject
    {

        private readonly DatabaseContext _context;

        public TaskViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<TaskModel> _tasks = new();

        [ObservableProperty]
        private TaskModel _operatingTask = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        public async Task LoadTasksAsync()
        {
            await ExecuteAsync(async () =>
            {
                var tasks = await _context.GetAllAsync<TaskModel>();
                if (tasks is not null && tasks.Any())
                {
                    Tasks ??= new ObservableCollection<TaskModel>();
                    foreach (var task in tasks)
                    {
                        Tasks.Add(task);
                    }
                }
            }, "Fetching tasks...");
        }

        private async Task ExecuteAsync(Func<Task> operating, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
            try
            {
                if (operating != null)
                    await operating.Invoke();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing...";
            }
        }

        [RelayCommand]
        private void SetOperatingTask(TaskModel? task) => OperatingTask = task ?? new TaskModel();

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (OperatingTask is null)
                return;

            var (isValid, errorMessage) = OperatingTask.Validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            var busyText = OperatingTask.Id == 0 ? "Creating task..." : "Updating task...";
            await ExecuteAsync(async () =>
            {
                if (OperatingTask.Id == 0)
                {
                    await _context.AddItemAsync<TaskModel>(OperatingTask);
                    Tasks.Add(OperatingTask);
                }
                else
                {
                    if (await _context.UpdateItemAsync<TaskModel>(OperatingTask))
                    {
                        var taskCopy = OperatingTask.Clone();
                        var index = Tasks.IndexOf(OperatingTask);

                        Tasks.RemoveAt(index);
                        Tasks.Insert(index, taskCopy);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Task updating error", "OK");
                        return;
                    }
                }

                SetOperatingTaskCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteTaskAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<TaskModel>(id))
                {
                    var task = Tasks.FirstOrDefault(x => x.Id == id);
                    if (task != null)
                        Tasks.Remove(task);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Task was not deleted", "OK");
                }
            }, "Deleting task...");
        }
    }
}
