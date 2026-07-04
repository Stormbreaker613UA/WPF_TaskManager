using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Implementation;

public class JsonTaskStorageService : ITaskStorageService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonTaskStorageService()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var dataFolder = Path.Combine(baseDirectory, "Data");

        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        _filePath = Path.Combine(dataFolder, "tasks.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public List<TaskItem> GetAll()
    {
        if (!File.Exists(_filePath))
            return new List<TaskItem>();

        try
        {
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<TaskItem>();

            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions);
            return tasks ?? new List<TaskItem>();
        }
        catch (JsonException)
        {
            return new List<TaskItem>();
        }
    }

    public TaskItem? GetById(Guid id)
    {
        var tasks = GetAll();
        return tasks.FirstOrDefault(t => t.Id == id);
    }

    public void Add(TaskItem task)
    {
        var tasks = GetAll();
        tasks.Add(task);
        SaveAll(tasks);
    }

    public void Update(TaskItem task)
    {
        var tasks = GetAll();
        var existing = tasks.FirstOrDefault(t => t.Id == task.Id);

        if (existing == null)
            return;

        var index = tasks.IndexOf(existing);
        tasks[index] = task;

        SaveAll(tasks);
    }

    public void Delete(Guid id)
    {
        var tasks = GetAll();
        var existing = tasks.FirstOrDefault(t => t.Id == id);

        if (existing == null)
            return;

        tasks.Remove(existing);
        SaveAll(tasks);
    }

    private void SaveAll(List<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}
