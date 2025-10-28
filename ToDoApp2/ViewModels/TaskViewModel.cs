using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
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

        // Kõik tööd (toorloetelu)
        [ObservableProperty]
        private ObservableCollection<TaskModel> _tasks = new();

        // ✓ tööd
        [ObservableProperty]
        private ObservableCollection<TaskModel> _completedTasks = new();

        // ✗ tööd — UUS
        [ObservableProperty]
        private ObservableCollection<TaskModel> _uncompletedTasks = new();

        // Vormil töötatav töö
        [ObservableProperty]
        private TaskModel _operatingTask = new();

        public string TaskButtonText =>
            OperatingTask != null && OperatingTask.Id > 0 ? "Update Task" : "Create Task";

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

                // Täida mõlemad alamvaated
                RefreshCompletedAndUncompleted();
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

        // Uuenda ✓ ja ✗ loetelusid korraga
        private void RefreshCompletedAndUncompleted()
        {
            CompletedTasks.Clear();
            UncompletedTasks.Clear();

            foreach (var t in Tasks)
            {
                if (t.IsCompleted) CompletedTasks.Add(t);
                else UncompletedTasks.Add(t);
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
            if (OperatingTask is null) return;

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

                RefreshCompletedAndUncompleted();
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

                RefreshCompletedAndUncompleted();
            });
        }

        public async Task UpdateTaskCompletionAsync(TaskModel task)
        {
            // UI kohe
            var index = Tasks.ToList().FindIndex(t => t.Id == task.Id);
            if (index >= 0)
                Tasks[index].IsCompleted = task.IsCompleted;

            RefreshCompletedAndUncompleted();

            // Salvesta
            _ = Task.Run(async () =>
            {
                try
                {
                    await _context.UpdateTaskAsync<TaskModel>(task);
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                    });
                }
            });
        }
    }
}
