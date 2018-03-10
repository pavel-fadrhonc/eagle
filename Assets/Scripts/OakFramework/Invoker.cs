//  <copyright file="Invoker.cs" company="Xinity">
//  Copyright (c) 2013 All Right Reserved, http://xinity.com/
// 
//  </copyright>
//  <author>Pavel Fadrhonc</author>
//  <email>pavel.fadrhonc@xinity.com</email>
//  <date>21.8.2013</date>
//  <summary> Allows to InvokeOnce or InvokeRepeating methods that are not part of MonoBehaviours </summary>

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Invoker : SceneSingleton<Invoker>
{
    private class InvokeTask
    {
        public OakTask Task;
        public float Delay;
        public float Interval;
        public float TimeSinceLastInvoke;
        public float TotalRunTime;
        public bool started;
        public float cancelTime;
        public bool active;

        public InvokeTask()
        {
            Reset();
        }

        public void Reset()
        {
            Task = null;
            Delay = 0;
            Interval = 0;
            TimeSinceLastInvoke = 0;
            started = false;
            cancelTime = 0;
            TotalRunTime = 0;
            active = true;
        }
    }

    private class InvokeTasksCache
    {
        private List<InvokeTask> _invokeTasks = new List<InvokeTask>();

        public InvokeTask GetCleanTask()
        {
            InvokeTask task;
            if ((task = _invokeTasks.FirstOrDefault(t => t.active == false)) == null)
            {
                task = new InvokeTask();
                _invokeTasks.Add(task);
            }
            else
            {
                task.Reset();
            }

            return task;
        }

        public void StopTask(InvokeTask task)
        {
            task.active = false;
        }
    }

    private List<InvokeTask> _tasks = new List<InvokeTask>();
    private InvokeTasksCache _tasksCache = new InvokeTasksCache();
    private List<InvokeTask> _removedTasks = new List<InvokeTask>();

#region UNITY METHODS

    void FixedUpdate()
    {
        _removedTasks.Clear();

        for (int index = 0; index < _tasks.Count; index++)
        {
            var invokeTask = _tasks[index];
            invokeTask.TotalRunTime += Time.deltaTime;

            if (!invokeTask.started)
            {
                if (Mathf.Approximately(invokeTask.TotalRunTime, invokeTask.Delay) ||
                    invokeTask.TotalRunTime > invokeTask.Delay)
                {
                    invokeTask.started = true;
                    invokeTask.TimeSinceLastInvoke += Time.deltaTime;
                    invokeTask.Task();
                }
            }
            else
            {
                invokeTask.TimeSinceLastInvoke += Time.deltaTime;
                if (Mathf.Approximately(invokeTask.TimeSinceLastInvoke, invokeTask.Interval) ||
                    invokeTask.TimeSinceLastInvoke > invokeTask.Interval)
                {
                    invokeTask.Task();
                    invokeTask.TimeSinceLastInvoke = invokeTask.TimeSinceLastInvoke - invokeTask.Interval;
                }
            }

            if ((Mathf.Approximately(invokeTask.TotalRunTime, invokeTask.cancelTime) ||
                 invokeTask.TotalRunTime > invokeTask.cancelTime) &&
                (invokeTask.cancelTime > 0 && invokeTask.Interval != 0 || invokeTask.Interval == 0))
                _removedTasks.Add(invokeTask);
        }

        // removing has to be at the end for Invoke to work properly
        for (int index = 0; index < _removedTasks.Count; index++)
        {
            var removedTask = _removedTasks[index];
            StopTask(removedTask);
        }
    }

#endregion

    #region PUBLIC METHODS

    public void InvokeRepeating(OakTask task_, float delay_, float interval_, float cancelTime = 0)
    {
        var invokeTask = _tasksCache.GetCleanTask();
        invokeTask.Delay = delay_;
        invokeTask.Interval = interval_;
        invokeTask.Task = task_;
        invokeTask.started = false;
        invokeTask.cancelTime = cancelTime;

        _tasks.Add(invokeTask);
    }

    /// <summary>
    /// Invokes just once
    /// </summary>
    public void Invoke(OakTask task_, float delay_)
    {
        // using cancelTime = delay can work because record are remove at the end of Update loop
        InvokeRepeating(task_, delay_, 0, delay_);
    }

    /// <summary>
    /// Wont break if task is not actually Invoking
    /// </summary>
    public void StopInvoke(OakTask task, float delay)
    {
        var invTask = _tasks.Find(t => t.Task == task);
        if (invTask == null) return;
        if (delay == 0)
            StopTask(invTask);
        else
            invTask.cancelTime = invTask.TotalRunTime + delay;
    }

    #endregion

    #region PRIVATE / PROTECTED METHODS

    private void StopTask(InvokeTask task)
    {
        _tasks.Remove(task);
        _tasksCache.StopTask(task);        
    }

    #endregion
}
