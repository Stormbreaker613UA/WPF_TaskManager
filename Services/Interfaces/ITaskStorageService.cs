using System;
using System.Collections.Generic;
using TaskManager.Models;

namespace TaskManager.Services.Interfaces;

public interface ITaskStorageService
{
    List<TaskItem> GetAll();
    TaskItem? GetById(Guid id);
    void Add(TaskItem task);
    void Update(TaskItem task);
    void Delete(Guid id);
}
