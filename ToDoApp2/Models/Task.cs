using SQLite;
using System;
using System.ComponentModel;

namespace ToDoApp2.Models
{
    public class Task : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime? DueDate { get; set; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        public Task Clone() => new Task
        {
            Id = this.Id,
            Title = this.Title,
            DueDate = this.DueDate,
            IsCompleted = this.IsCompleted
        };

        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                return (false, $"{nameof(Title)} is required");
            }

            return (true, null);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
