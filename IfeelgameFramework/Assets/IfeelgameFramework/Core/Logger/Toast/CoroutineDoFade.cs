using System;
using System.Collections;
using UnityEngine;

namespace IfeelgameFramework.Core.Logger.Toast
{
    /// <summary>
    /// 自定义协程：数字变化
    /// </summary>
    public class CoroutineNumChange : IEnumerator
    {
        private readonly float _numStart;
        private readonly float _numEnd;
        private float _numCurrent;
        private readonly float _numOffset;
        private readonly Action<float> _moveNextCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="numStart">开始数字</param>
        /// <param name="numEnd">结束数字</param>
        /// <param name="frameCount">所需帧数</param>
        /// <param name="moveNextCallback">每帧变化完成后的回调</param>
        public CoroutineNumChange(float numStart, float numEnd, int frameCount, Action<float> moveNextCallback)
        {
            _numStart = numStart;
            _numEnd = numEnd;
            _moveNextCallback = moveNextCallback;

            _numCurrent = _numStart;
            _numOffset = (_numEnd - _numStart) / frameCount;
        }
    
        public bool MoveNext()
        {
            _numCurrent += _numOffset;
            if (Math.Abs(_numCurrent - _numStart) >= Math.Abs(_numEnd - _numStart))
            {
                _numCurrent = _numEnd;
            }

            _moveNextCallback?.Invoke(_numCurrent);

            return Math.Abs(_numCurrent - _numStart) < Math.Abs(_numEnd - _numStart);
        }

        public void Reset()
        {
            _numCurrent = _numStart;
        }

        public object Current => _numCurrent;
    }
}
