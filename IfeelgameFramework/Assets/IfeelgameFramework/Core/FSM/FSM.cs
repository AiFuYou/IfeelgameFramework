using System.Collections.Generic;
using UnityEngine;

namespace IfeelgameFramework.Core.FSM
{
    public class FSM
    {
        #region 参数
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="param"></param>
        public delegate void FSMCallbackFunc(params object[] param);
        /// <summary>
        /// 状态名称
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// 存储状态回调参数的字典
        /// </summary>
        private readonly Dictionary<string, FSMSimpleStateModel> _simpleStateDict = new Dictionary<string, FSMSimpleStateModel>();

        /// <summary>
        /// 存储状态机转换StateModel用字典
        /// </summary>
        private readonly Dictionary<string, FSMTranslationStateModel> _translateStateDict = new Dictionary<string, FSMTranslationStateModel>();

        #endregion


        #region Model
        /// <summary>
        /// 记录状态名称,和转换FSMTranslation对象
        /// </summary>
        class FSMTranslationStateModel
        {
            private string _name;

            public FSMTranslationStateModel(string name)
            {
                _name = name;
            }

            /// <summary>
            /// 记录状态名和转换状态对象的字典
            /// </summary>
            public readonly Dictionary<string, FSMTranslateModel> TranslationDict = new Dictionary<string, FSMTranslateModel>();
        }

        /// <summary>
        /// 简易回调数据模型
        /// </summary>
        class FSMSimpleStateModel
        {
            public string State;
            public FSMCallbackFunc CallbackFunc;
            public FSMSimpleStateModel(string state, FSMCallbackFunc callBackFunc)
            {
                State = state;
                CallbackFunc = callBackFunc;
            }
        }

        /// <summary>
        /// 状态转换数据模型
        /// </summary>
        class FSMTranslateModel
        {
            public string FromState;
            public string Name;
            public string ToState;
            public FSMCallbackFunc OnTranslationCallback;// 回调函数

            public FSMTranslateModel(string fromState,string name,string toState,FSMCallbackFunc onTranslationCallback)
            {
                FromState = fromState;
                Name = name;
                ToState = toState;
                OnTranslationCallback = onTranslationCallback;
            }
        }


        #endregion


        #region 状态控制方法
        /// <summary>
        /// 添加一个新状态
        /// </summary>
        /// <param name="name">状态名称</param>
        public void AddState(string name)
        {
            _translateStateDict[name] = new FSMTranslationStateModel(name);
        }

        /// <summary>
        /// 设定最开始的状态
        /// </summary>
        /// <param name="name"></param>
        public void StartState(string name)
        {
            State = name;
        }

        /// <summary>
        /// 添加一种状态,和回调方法
        /// </summary>
        /// <param name="state">状态名称</param>
        /// <param name="callbackFunc">回调方法</param>
        public void AddSimpleState(string state, FSMCallbackFunc callbackFunc)
        {
            _simpleStateDict[state] = new FSMSimpleStateModel(state, callbackFunc);
        }

        /// <summary>
        /// 添加一种转换状态监听
        /// </summary>
        /// <param name="translateName">转换行为名称</param>
        /// <param name="fromState">原来是那种状态</param>
        /// <param name="toState">转换为那种状态</param>
        /// <param name="callbackFunc">回调方法</param>
        public void AddTranslateState(string translateName, string fromState, string toState, FSMCallbackFunc callbackFunc)
        {
            _translateStateDict[fromState].TranslationDict[translateName] = new FSMTranslateModel(fromState, translateName, toState, callbackFunc);
        }


        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="state">状态名</param>
        /// <param name="param">参数</param>
        public void ChangeState(string state,params object[] param)
        {
            if (_simpleStateDict.ContainsKey(state) == false)
            {
                Debug.LogError("未找到这个状态");
                return;
            }
            FSMSimpleStateModel simple = _simpleStateDict[state];
            simple.CallbackFunc(param);
            State = state;
        }

        /// <summary>
        /// 通过传入事件名,自动转换状态
        /// </summary>
        /// <param name="name">转换事件名称</param>
        /// <param name="param">参数</param>
        public void TranslateState(string name,params object[] param)
        {
            if (State == null)
            {
                Debug.LogError("未赋值初始状态");
                return;
            }
            if (!_translateStateDict[State].TranslationDict.ContainsKey(name))
            {
                Debug.LogError("未找到这个转换状态");
                return;
            }
            FSMTranslateModel tempTranslation = _translateStateDict[State].TranslationDict[name];
            tempTranslation.OnTranslationCallback(param);
            State = tempTranslation.ToState;
       
        }
        #endregion

        /// <summary>
        /// 清理这个实例的存储
        /// </summary>
        public void Clear()
        {
            _translateStateDict.Clear();
        }

    }
}
