using _NueCore.SaveSystem;

namespace _NueExtras.NTrackerSystem
{
    public class TrackSave_Global : NBaseSave
    {
        #region Setup

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.Global;
        }

        protected override string GetSavePath()
        {
            return  "TrackSave";
        }

        public override void Save()
        {
            NSaver.SaveData<TrackSave_Global>();
        }

        public override void Load()
        {
            NSaver.GetSaveData<TrackSave_Global>();
        }
        
        public override void ResetSave()
        {
            NSaver.ResetSave<TrackSave_Global>();
        }

        #endregion

        public TrackInfo globalTrack;
        public TrackInfo sessionTrack;
    }
}