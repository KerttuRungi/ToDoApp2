# ToDoApp2

**ToDoApp2** is a **.NET MAUI** To-Do application, developed as a **group project**.  
It allows users to manage tasks with full CRUD operations, filtering, and completion tracking.

---

## 📝 Features

- **Task Management**
  - Create, read, update, delete tasks  
  - Mark tasks as completed or uncompleted  
  - View different task pages (All, Completed, Uncompleted)  

- **AllTasksPage**
  - Displays all tasks in a **CollectionView**  
  - Tasks have checkboxes to toggle completion  
  - Edit and delete buttons for each task  
  - Dynamic coloring of tasks: completed tasks are green, uncompleted are default color  
  - “No tasks found” placeholder when empty
  
- **CompletedTasksPage & UncompletedTasksPage**
  - Filtered views of tasks based on completion status  
  - CollectionView displays tasks only with `IsCompleted = true` or `false` 
  - Back/Navigation buttons to switch between views  
  - "No tasks found" placeholder when filtered list is empty  

- **Navigation**
  - Buttons to navigate to CompletedTasksPage and UncompletedTasksPage  
  - Shell or stack navigation for smooth page transitions  

- **Local SQLite Persistence**
  - Tasks stored locally using a database context  
  - Retrieves filtered items using LINQ expressions  
  - Creates tables if they do not exist automatically   

---

## 🧩 Group Collaboration

- Multiple branches for feature development and UI improvements

- **Team workflow:**

- Each member works on a separate branch

- Merge into main after peer review

- Coordinated fixes, helping out on different branches if needed

## 🎯 Purpose

- Learn .NET MAUI development

- Practice C# 

- Practice MVVM architecture and data-binding

- Implement CRUD operations with SQLite persistence

- Gain experience with team-based development and branch management

## 🔮 Future Improvements

Add task priorities and categories
