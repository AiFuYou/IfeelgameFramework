using System.Runtime.CompilerServices;

namespace IfeelgameFramework.Core.Awaiter
{
    #region INotifyCompletion
    
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
    
    #endregion

    #region ICriticalNotifyCompletion

    public interface ICriticalAwaitable<out TCriticalAwaiter> where TCriticalAwaiter : ICriticalAwaiter
    {
        TCriticalAwaiter GetAwaiter();
    }

    public interface ICriticalAwaitable<out TCriticalAwaiter, out TResult> where TCriticalAwaiter : ICriticalAwaiter<TResult>
    {
        TCriticalAwaiter GetAwaiter();
    }
    
    public interface ICriticalAwaiter : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface ICriticalAwaiter<out TResult> :  ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }
    
    #endregion
}