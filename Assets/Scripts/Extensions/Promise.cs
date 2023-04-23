using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Simply provides a trigger method
public abstract class InputPromise<S>
{
  abstract public void Trigger(S antecedentResult);

  abstract public void Fail(string message);

}

// Actual implementation of Promise
public class OutputInputPromise<T, S> : InputPromise<S>
{
  bool triggered = false;

  public bool Pending { get; private set; } = true;

  T cachedResult = default(T);

  UnityAction<S, UnityAction<T>, UnityAction<string>> promisedTask;
  List<InputPromise<T>> chainedPromises = new List<InputPromise<T>>();
  List<UnityAction<string>> failTasks = new List<UnityAction<string>>();

  public OutputInputPromise(UnityAction<S, UnityAction<T>, UnityAction<string>> promisedTask)
  {
    this.promisedTask = promisedTask;
  }

  public OutputInputPromise(UnityAction<S, UnityAction<T>, UnityAction<string>> promisedTask, S autoTriggerInput) : this(promisedTask)
    => Trigger(autoTriggerInput);


  public override void Trigger(S antecedentResult)
  {
    if (triggered) return;

    triggered = true;

    promisedTask(antecedentResult,
    (result) =>
    {
      Pending = false;
      cachedResult = result;

      foreach (var nextPromise in chainedPromises) nextPromise.Trigger(result);
    },
    Fail);
  }

  override public void Fail(string message)
  {
    Pending = false;

    foreach (var nextPromise in chainedPromises) nextPromise.Fail(message);

    foreach (var failTask in failTasks) failTask(message);
  }

  public T RequireResult()
  {
    if (Pending) throw new InvalidOperationException("Required result of a pending promise");

    return cachedResult;
  }

  public OutputInputPromise<U, T> Then<U>(Func<T, U> chainTask)
  {
    var chainedPromise = new OutputInputPromise<U, T>((antecedentResult, resolve, reject) =>
    {
      try
      {
        resolve(chainTask(antecedentResult));
      }
      catch (Exception e)
      {
        reject(e.Message);
      }
    });

    chainedPromises.Add(chainedPromise);

    return chainedPromise;
  }

  public OutputInputPromise<T, S> OnFail(UnityAction<string> failTask)
  {
    failTasks.Add(failTask);
    return this;
  }
}

// Wrapper that abstracts the input type of the promise implementation
public class Promise<T>
{
  OutputInputPromise<T, Null> internalPromise;

  public bool Pending => internalPromise.Pending;


  public Promise(UnityAction<UnityAction<T>, UnityAction<string>> promisedTask)
  {
    // Auto trigger internal promise
    internalPromise = new OutputInputPromise<T, Null>(
      (_, resolve, reject) => promisedTask(resolve, reject), null);
  }

  public T RequireResult() => internalPromise.RequireResult();

  public OutputInputPromise<U, T> Then<U>(Func<T, U> chainTask) => internalPromise.Then(chainTask);

  public OutputInputPromise<T, Null> OnFail(UnityAction<string> failTask) => internalPromise.OnFail(failTask);

}