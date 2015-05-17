using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Loom : MonoBehaviour {

    #region 公共字段

    /// <summary>
    /// 最大线程数，静态公共字段
    /// </summary>
    public static int maxThreads = 8;

    #endregion

    #region 私有字段

    /// <summary>
    /// 当前线程数，静态私有字段
    /// </summary>
    private static int numThreads;

    /// <summary>
    /// 单例模式
    /// </summary> 
    private static Loom _current;

    private int _count;

    /// <summary>
    /// 是否已初始化
    /// </summary> 
    private static bool initialized;

    private List<Action> _actions;
    private List<DelayedQueueItem> _delayed;
    private List<DelayedQueueItem> _currentDelayed;

    /// <summary>
    /// 当前行为列表
    /// </summary>
    private List<Action> _currentActions;

    #endregion

    #region 公有属性
    
    /// <summary>
    /// 得到当前Loom对象的单例属性
    /// </summary>
    public static Loom Current {
        get {
            Initialize();
            return _current;
        }
    }

    #endregion

    #region Unity3d API
    
    void Awake() {
        _current = this;
        initialized = true;
        _actions = new List<Action>();
        _delayed = new List<DelayedQueueItem>();
        _currentDelayed = new List<DelayedQueueItem>();
        _currentActions = new List<Action>();
    }

    void Update ( ) {
        lock ( _actions ) {
            _currentActions.Clear( );
            _currentActions.AddRange(_actions);
            _actions.Clear( );
        }
        foreach ( Action a in _currentActions ) {
            a( );
        }
        lock ( _delayed ) {
            _currentDelayed.Clear( );
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach ( var item in _currentDelayed )
                _delayed.Remove(item);
        }
        foreach ( var delayed in _currentDelayed ) {
            delayed.action( );
        }
    }

    void OnDisable ( ) {
        if ( _current == this ) {
            _current = null;
        }
    }

    #endregion Unity3d API

    /// <summary>
    /// 初始化，造一个新的游戏物体Loom，并加一个Loom脚本
    /// </summary> 
    private static void Initialize() {
        if (!initialized) {
            if (!Application.isPlaying)
                return;
            initialized = true;
            GameObject g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }
    
    /// <summary>
    /// 延时队列项
    /// </summary>
    public struct DelayedQueueItem {
        public float time;
        public Action action;
    }    

    /// <summary>
    /// 在主线程上运行的代码
    /// </summary>    
    public static void QueueOnMainThread(Action action) {
        QueueOnMainThread(action, 0f);
    }
    /// <summary>
    /// 在主线程上运行的代码, 第2个参数为延迟几秒执行, 依赖Time.time执行
    /// </summary>   
    public static void QueueOnMainThread(Action action, float time) {
        if (time != 0) {
            lock (Current._delayed) {
                // 增加Time.time的值给time达到延时的效果
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        } else {
            lock (Current._actions) {
                Current._actions.Add(action);
            }
        }
    }

    /// <summary>
    /// 在新线程上运行的代码
    /// </summary>   
    public static Thread RunAsync(Action a) {
        Initialize();
        while (numThreads >= maxThreads) {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action) {
        try {
            ((Action)action)();
        } catch(Exception e) {
            Debug.Log(e.Message);
            Debug.Log(e.Source);
        } finally  {
            Interlocked.Decrement(ref numThreads);
        }
    }
    
}