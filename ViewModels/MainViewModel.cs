using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskManager.Commands;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ITaskStorageService _storageService;

    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    private TaskItemViewModel? _selectedTask;
    public TaskItemViewModel? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    private string _newTaskTitle = string.Empty;
    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set => SetProperty(ref _newTaskTitle, value);
    }

    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }

    public MainViewModel(ITaskStorageService storageService)
    {
        _storageService = storageService;

        var loadedTasks = _storageService.GetAll();
        Tasks = new ObservableCollection<TaskItemViewModel>(
            loadedTasks.Select(CreateTaskViewModel));

        AddTaskCommand = new RelayCommand(_ => AddTask(), _ => CanAddTask());
        DeleteTaskCommand = new RelayCommand(_ => DeleteTask(), _ => CanDeleteTask());
    }

    private TaskItemViewModel CreateTaskViewModel(TaskItem model)
    {
        var viewModel = new TaskItemViewModel(model);
        viewModel.TaskChanged += OnTaskChanged; 
        return viewModel;
    }

    private void OnTaskChanged(object? sender, EventArgs e)
    {
        if (sender is TaskItemViewModel changedTask)
            _storageService.Update(changedTask.ToModel());
    }

    private bool CanAddTask()
    {
        return !string.IsNullOrWhiteSpace(NewTaskTitle);
    }

    private void AddTask()
    {
        var newModel = new TaskItem
        {
            Title = NewTaskTitle
        };

        _storageService.Add(newModel);

        var newViewModel = CreateTaskViewModel(newModel);
        Tasks.Add(newViewModel);

        NewTaskTitle = string.Empty;
    }

    private bool CanDeleteTask()
    {
        return SelectedTask != null;
    }

    private void DeleteTask()
    {
        if (SelectedTask == null)
            return;

        SelectedTask.TaskChanged -= OnTaskChanged;

        _storageService.Delete(SelectedTask.Id);
        Tasks.Remove(SelectedTask);

        SelectedTask = null;
    }
}
