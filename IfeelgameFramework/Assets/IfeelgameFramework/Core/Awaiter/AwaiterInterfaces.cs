﻿using System.Runtime.CompilerServices;

namespace IfeelgameFramework.Core.Awaiter
{
    public interface IAwaitable<out TAwaiter> where TAwaiter : IAwaiter
    {
        TAwaiter GetAwaiter();
    }

    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult>
    {
        TAwaiter GetAwaiter();
    }

    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface IAwaiter<out TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }

    public interface ICriticalAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
    
    }

    public interface ICriticalAwaiter<out TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
    {
    
    }
}