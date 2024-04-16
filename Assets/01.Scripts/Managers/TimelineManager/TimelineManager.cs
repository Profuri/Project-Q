using System;
using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineQueueInfo
{
    public readonly PlayableDirector Director;
    public readonly TimelineType AssetType;
    public readonly bool SkipOnStart;
    public readonly Action Callback;

    public TimelineQueueInfo(PlayableDirector director, TimelineType assetType, bool skipOnStart, Action callback)
    {
        Director = director;
        AssetType = assetType;
        SkipOnStart = skipOnStart;
        Callback = callback;
    }
}

public class TimelineManager : BaseManager<TimelineManager>
{
    [SerializeField] private TimelineClipSetList _timelineClipSetList;
    [SerializeField] private float _skipOffset;

    private Queue<TimelineQueueInfo> _timelineQueue;
    private PlayableDirector _currentDirector;

    public bool IsPlay
    {
        get
        {
            if (_currentDirector is null || !_currentDirector.playableGraph.IsValid())
            {
                return false;
            }

            return _currentDirector.playableGraph.GetRootPlayable(0).GetTime() <= _currentDirector.duration;
        }
    }

    public override void Init()
    {
        base.Init();
        _timelineQueue = new Queue<TimelineQueueInfo>();
    }

    public override void StartManager()
    {
        _currentDirector = null;
    }

    public void ShowTimeline(PlayableDirector director, TimelineType type, bool skipOnStart = false, Action onComplete = null)
    {
        _timelineQueue.Enqueue(new TimelineQueueInfo(director, type, skipOnStart, onComplete));
        while (_timelineQueue.Count > 1 && !_timelineQueue.Peek().SkipOnStart)
        {
            _timelineQueue.Enqueue(_timelineQueue.Dequeue());
        }

        if (!IsPlay)
        {
            PlayNextQueue();
        }
    }

    private void SetDirectorSpeed(float speed)
    {
        _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
    }

    private void PlayNextQueue()
    {
        if (_timelineQueue.Count <= 0)
        {
            return;
        }
        
        var info = _timelineQueue.Dequeue();
        
        _currentDirector = info.Director;
        _currentDirector.playableAsset = _timelineClipSetList.GetAsset(info.AssetType);
        
        // 이거 문제
        CoroutineManager.Instance.StartSafeCoroutine(GetInstanceID(), PlayRoutine(info.Callback));

        if (info.SkipOnStart)
        {
            _currentDirector.playableGraph.GetRootPlayable(0).SetTime(_currentDirector.duration - _skipOffset);
        }
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {
        _currentDirector.Play();
        yield return new WaitUntil(() => !IsPlay);
        _currentDirector.Stop();
        _currentDirector = null;

        onComplete?.Invoke();

        PlayNextQueue();
    }
}