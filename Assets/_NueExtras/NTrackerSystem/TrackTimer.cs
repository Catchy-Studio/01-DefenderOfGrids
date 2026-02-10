using System;
using UniRx;
using UnityEngine;

namespace _NueExtras.NTrackerSystem
{
    public class TrackTimer : IDisposable
    {
        public GameObject GO { get; private set; }
        private IDisposable _disposable;
        public TrackTimer(GameObject go)
        {
            GO = go;
           
            _disposable =Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(ev =>
            {
                NTrackStatic.SetStat(TrackType.TotalRunTime,NTrackStatic.GetStat<int>(TrackType.TotalRunTime)+1);
            }).AddTo(GO);
        }
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}