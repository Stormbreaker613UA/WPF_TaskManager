using System;
using System.Runtime.CompilerServices;
using TaskManager.Models;

namespace TaskManager.ViewModels;

public class TaskItemViewModel : BaseViewModel
{
    private readonly TaskItem _model;

    public event EventHandler? TaskChanged;

    public TaskItemViewModel(TaskItem model)
    {
        _model = model;
    }

    public Guid Id => _model.Id;

    public string Title
    {
        get => _model.Title;
        set 
        { 
            if (SetModelProperty(_model.Title, v => _model.Title = v, value))
               TaskChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string Description
    {
        get => _model.Description;
        set 
        { 
            if (SetModelProperty(_model.Description, v => _model.Description = v, value))
               TaskChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public TaskPriority Priority
    {
        get => _model.Priority;
        set 
        { 
            if (SetModelProperty(_model.Priority, v => _model.Priority = v, value))
               TaskChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsCompleted
    {
        get => _model.IsCompleted;
        set
        {
            if (SetModelProperty(_model.IsCompleted, v => _model.IsCompleted = v, value))
            {
                OnPropertyChanged(nameof(IsOverdue));
                TaskChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public DateTime CreatedAt => _model.CreatedAt;

    public DateTime? DueDate
    {
        get => _model.DueDate;
        set
        {
            if (SetModelProperty(_model.DueDate, v => _model.DueDate = v, value))
            {
                OnPropertyChanged(nameof(IsOverdue));
                TaskChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Computed UI-only property: whether the task is overdue 
    public bool IsOverdue => !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.Now;

    // Returns the raw model — required for saving via ITaskStorageService
    public TaskItem ToModel() => _model;

    private bool SetModelProperty<T>(T currentValue, Action<T> setter, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        setter(newValue);
        OnPropertyChanged(propertyName);
        return true;
    }
}
