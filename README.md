# Task Manager (WPF)

A small desktop task management application built with WPF and the MVVM pattern, focused on clean architecture and separation of concerns rather than UI polish.

## Overview

This project was built as a hands-on exercise to apply MVVM principles, dependency injection, and clean layering in a desktop application — as a complement to my primary backend experience (ASP.NET Core + EF Core).

## Features

- Add, edit, and delete tasks
- Mark tasks as completed
- Set priority (Low / Medium / High)
- Set due dates, with automatic "overdue" detection
- Persistent storage via JSON file (auto-created on first run)
- Auto-save on any change

## Architecture

The project follows classic MVVM with clear separation between layers:

```
Models/          → Plain domain entities (TaskItem), no UI dependencies
ViewModels/      → UI-facing logic, INotifyPropertyChanged, commands
Views/           → XAML UI, data-bound to ViewModels
Commands/        → ICommand implementation (RelayCommand)
Services/        → Data access abstraction (Interfaces + Implementation)
```

Key design decisions:

- **`TaskItem` is a plain POCO** (not a ViewModel) — kept free of UI concerns so it could later be reused as an EF Core entity without changes.
- **`TaskItemViewModel` wraps `TaskItem`** to provide `INotifyPropertyChanged` support and UI-only computed properties (e.g. `IsOverdue`), keeping the domain model clean.
- **`ITaskStorageService` interface** decouples the ViewModel from the storage implementation. The current implementation (`JsonTaskStorageService`) persists data to a local JSON file, but the interface is designed so a future `EfTaskStorageService` (EF Core + SQLite) could be swapped in without touching any ViewModel code — a practical example of the Dependency Inversion Principle.
- **Dependency Injection** via `Microsoft.Extensions.DependencyInjection`, configured in `App.xaml.cs`, following constructor injection throughout.
- **Auto-save via events** — `TaskItemViewModel` raises a `TaskChanged` event on any property change, which `MainViewModel` listens to in order to persist updates immediately, without the ViewModel needing direct access to the storage service.

## Tech Stack

- .NET (WPF)
- C#
- MVVM (hand-rolled, no third-party MVVM framework)
- Microsoft.Extensions.DependencyInjection
- System.Text.Json

## Project Structure

```
TaskManager/
├── App.xaml
├── App.xaml.cs
│
├── Models/
│   └── TaskItem.cs
│
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── TaskItemViewModel.cs
│   └── MainViewModel.cs
│
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
│
├── Commands/
│   └── RelayCommand.cs
│
├── Services/
│   ├── Interfaces/
│   │   └── ITaskStorageService.cs
│   └── Implementation/
│       └── JsonTaskStorageService.cs
│
└── Data/
    └── tasks.json (auto-created on first run)
```

## Possible Next Steps

- Replace JSON storage with EF Core + SQLite (`ITaskStorageService` is already designed for this)
- Add filtering/sorting (by priority, completion status, due date)
- Improve UI styling (currently uses default `DataGrid` styling)
- Add validation (e.g. required title, due date must be in the future)

## Running the Project

1. Clone the repository
2. Open `TaskManager.sln` in Visual Studio
3. Restore NuGet packages (should happen automatically)
4. Run (`F5`) — a `Data/tasks.json` file will be created automatically on first launch