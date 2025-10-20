using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp2.Models
{
    public class Task
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

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
    }
}
