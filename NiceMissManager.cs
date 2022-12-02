using System.Collections.Generic;
using Zenject;

namespace NiceMiss
{
    internal class NiceMissManager : IInitializable
    {
        internal static Dictionary<string, NoteTracker.Rating> currentMapData;
        internal static bool modActive;

        private readonly NoteTracker noteTracker;
        private readonly NoteOutliner noteOutliner;
        private readonly IDifficultyBeatmap difficultyBeatmap;

        public NiceMissManager([InjectOptional] NoteTracker noteTracker, [InjectOptional] NoteOutliner noteOutliner, IDifficultyBeatmap difficultyBeatmap)
        {
            this.noteTracker = noteTracker;
            this.noteOutliner = noteOutliner;
            this.difficultyBeatmap = difficultyBeatmap;
        }

        public void Initialize()
        {
            if (noteTracker != null && noteOutliner != null)
            {
                if (NoteTracker.mapData.TryGetValue(difficultyBeatmap.SerializedName(), out Dictionary<string, NoteTracker.Rating> currentMapData))
                {
                    NiceMissManager.currentMapData = currentMapData;
                    modActive = true;
                    return;
                }
            }
            NiceMissManager.currentMapData = null;
            modActive = false;
        }
    }
}
