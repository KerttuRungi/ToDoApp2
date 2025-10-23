
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

        public string TaskButtonText => OperatingTask != null && OperatingTask.Id > 0 ? "Update Task" : "Create Task";

        partial void OnOperatingTaskChanged(TaskModel value)
        {
            OnPropertyChanged(nameof(TaskButtonText));
        }

        [ObservableProperty]
        private bool _isBusy;

        public async Task LoadTasksAsync()
        {
            await ExecuteAsync(async () =>
            {
                var tasks = await _context.GetAllAsync<TaskModel>();
                Tasks = new ObservableCollection<TaskModel>(tasks ?? new List<TaskModel>());
            });
        }

        private async Task ExecuteAsync(Func<Task> action)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await action.Invoke();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void SetOperatingTask(TaskModel? task)
        {
            OperatingTask = task?.Clone() ?? new TaskModel();
            OnPropertyChanged(nameof(TaskButtonText));
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (OperatingTask is null)
                return;

            var (isValid, errorMessage) = OperatingTask.Validate();
            if (!isValid)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                if (OperatingTask.Id == 0)
                {
                    await _context.AddTaskAsync<TaskModel>(OperatingTask);
                    Tasks.Add(OperatingTask);
                }
                else
                {
                    if (await _context.UpdateTaskAsync<TaskModel>(OperatingTask))
                    {
                        var index = Tasks.ToList().FindIndex(t => t.Id == OperatingTask.Id);
                        if (index >= 0)
                            Tasks[index] = OperatingTask;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Task update failed", "OK");
                        return;
                    }
                }

                SetOperatingTask(null);
            });
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
                    await Application.Current.MainPage.DisplayAlert("Error", "Task was not deleted", "OK");
                }
            });
        }

        public async Task UpdateTaskCompletionAsync(TaskModel task)
        {
            // Update UI immediately
            var index = Tasks.ToList().FindIndex(t => t.Id == task.Id);
            if (index >= 0)
                Tasks[index].IsCompleted = task.IsCompleted;

            // Save in background (non-blocking)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _context.UpdateTaskAsync<TaskModel>(task);
                }
                catch (Exception ex)
                {
                    // Only show alert on main thread
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                    });
                }
            });
        }
    }
}
