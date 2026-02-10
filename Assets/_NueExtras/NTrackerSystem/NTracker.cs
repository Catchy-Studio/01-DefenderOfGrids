using __Project.Systems.GameSystem;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using _NueCore.SettingsSystem;
using _NueExtras.MainMenuSystem;
using UniRx;
using UnityEngine;

namespace _NueExtras.NTrackerSystem
{
    public class NTracker : MonoBehaviour
    {

        #region Setup
        private void Awake()
        {
            RegisterShop();
            RegisterCore();
        }
        
        #endregion

        #region Active
        private void RegisterCore()
        {
            RBuss.OnEvent<MainMenuREvents.PreNewGameButtonClickedREvent>().Subscribe(ev =>
            {
                if (SaveStatic.HasSave)
                {
                    var abandonCount =NTrackStatic.GetGlobalStat<int>(TrackType.AbandonCount);
                    NTrackStatic.SetGlobalStat(TrackType.AbandonCount, abandonCount + 1);
                    var failCount = NTrackStatic.GetGlobalStat<int>(TrackType.LoseCount);
                    NTrackStatic.SetGlobalStat(TrackType.LoseCount, failCount + 1);
                    NTrackStatic.SaveLastSession();
                }
                var currentRunCount = NTrackStatic.GetGlobalStat<int>(TrackType.TotalRunCount);
                NTrackStatic.SetGlobalStat(TrackType.TotalRunCount,currentRunCount+1);
                NTrackStatic.ResetSession();
            }).AddTo(gameObject);
            RBuss.OnEvent<SettingsREvents.AbandonButtonClickedREvent>().Subscribe(ev =>
            {
                var abandonCount =NTrackStatic.GetGlobalStat<int>(TrackType.AbandonCount);
                NTrackStatic.SetGlobalStat(TrackType.AbandonCount, abandonCount + 1);
                var failCount = NTrackStatic.GetGlobalStat<int>(TrackType.LoseCount);
                NTrackStatic.SetGlobalStat(TrackType.LoseCount, failCount + 1);
                NTrackStatic.SaveLastSession();
                NTrackStatic.ResetSession();
            }).AddTo(gameObject);

            RBuss.OnEvent<GameREvents.LoseREvent>().Subscribe(ev =>
            {
                var loseCount =NTrackStatic.GetGlobalStat<int>(TrackType.LoseCount);
                NTrackStatic.SetGlobalStat(TrackType.LoseCount, loseCount + 1);
            }).AddTo(gameObject);

            RBuss.OnEvent<GameREvents.WinREvent>().Subscribe(ev =>
            {
                var winCount =NTrackStatic.GetGlobalStat<int>(TrackType.WinCount);
                NTrackStatic.SetGlobalStat(TrackType.WinCount, winCount + 1);
                var time = NTrackStatic.GetStat<int>(TrackType.TotalRunTime);
                var lastLongest = NTrackStatic.GetGlobalStat<int>(TrackType.LongestRunTime);
                if (time > lastLongest)
                    NTrackStatic.SetStat(TrackType.LongestRunTime, time);
                var lastFastest = NTrackStatic.GetGlobalStat<int>(TrackType.FastestRunTime);
                if (time < lastFastest || lastFastest == 0)
                    NTrackStatic.SetStat(TrackType.FastestRunTime, time);
            }).AddTo(gameObject);
        }
        private void RegisterShop()
        {
            // RBuss.OnEvent<StickerBase.StickerPurchasedREvent>().Subscribe(ev =>
            // {
            //     var stickerCount = NStats.GetStat<int>(StatsEnum.TotalStickerPurchase);
            //     NStats.SetStat(StatsEnum.TotalStickerPurchase, stickerCount + 1);
            //     var stickerID = ev.Sticker.GetID();
            //     var statKey = StatsEnum.StickerPurchased_.GetKey() + stickerID;
            //     var uniqueStickerCount =NStats.GetStat<int>(statKey);
            //     NStats.SetStat(statKey, uniqueStickerCount+1);
            // }).AddTo(gameObject);
            //
            // RBuss.OnEvent<PackBase.PackPurchasedREvent>().Subscribe(ev =>
            // {
            //     var packCount = NStats.GetStat<int>(StatsEnum.TotalPackPurchase);
            //     NStats.SetStat(StatsEnum.TotalPackPurchase, packCount + 1);
            //     var packID = ev.Pack.GetID();
            //     var statKey = StatsEnum.PackPurchased_.GetKey() + packID;
            //     var uniquePackCount = NStats.GetStat<int>(statKey);
            //     NStats.SetStat(statKey, uniquePackCount+1);
            // }).AddTo(gameObject);
            //
            // RBuss.OnEvent<RelicREvents.RelicClaimed>().Subscribe(ev =>
            // {
            //     var relicCount = NStats.GetStat<int>(StatsEnum.TotalRelicClaimed);
            //     NStats.SetStat(StatsEnum.TotalRelicClaimed, relicCount + 1);
            //     var relicID = ev.Data.GetID();
            //     var statKey = StatsEnum.RelicClaimed_.GetKey() + relicID;
            //     var uniqueRelicCount = NStats.GetStat<int>(statKey);
            //     NStats.SetStat(statKey, uniqueRelicCount+1);
            // }).AddTo(gameObject);
        }

        #endregion
    }
}