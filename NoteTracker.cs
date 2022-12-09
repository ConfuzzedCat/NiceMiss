using System;
using System.Collections.Generic;
using Zenject;

namespace NiceMiss
{
    public class NoteTracker : IInitializable, IDisposable, ISaberSwingRatingCounterDidChangeReceiver, ISaberSwingRatingCounterDidFinishReceiver
    {
        internal static Dictionary<string, Dictionary<string, Rating>> mapData = new Dictionary<string, Dictionary<string, Rating>>();

        private IDifficultyBeatmap difficultyBeatmap;
        private readonly BeatmapObjectManager beatmapObjectManager;
        private Dictionary<string, Rating> currentMapData;
        private Dictionary<int, NoteCutInfo> swingCounterCutInfo;
        private Dictionary<NoteCutInfo, NoteData> noteCutInfoData;
        private static int noteInt;

        public NoteTracker(IDifficultyBeatmap difficultyBeatmap, BeatmapObjectManager beatmapObjectManager)
        {
            this.difficultyBeatmap = difficultyBeatmap;

            this.beatmapObjectManager = beatmapObjectManager;
        }


        public void Initialize()
        {
            beatmapObjectManager.noteWasMissedEvent += BeatmapObjectManager_noteWasMissedEvent;
            beatmapObjectManager.noteWasCutEvent += BeatmapObjectManager_noteWasCutEvent;
            currentMapData = new Dictionary<string, Rating>();
            swingCounterCutInfo = new Dictionary<int, NoteCutInfo>();
            noteCutInfoData = new Dictionary<NoteCutInfo, NoteData>();
            noteInt = 0;
        }

        private void BeatmapObjectManager_noteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {

            ScoreController_noteWasCutEvent(noteController.noteData, in noteCutInfo, 1);
        }

        private void BeatmapObjectManager_noteWasMissedEvent(NoteController obj)
        {
            ScoreController_noteWasMissedEvent(obj.noteData, 1);
        }

        public void Dispose()
        {
            beatmapObjectManager.noteWasMissedEvent -= BeatmapObjectManager_noteWasMissedEvent;
            beatmapObjectManager.noteWasCutEvent -= BeatmapObjectManager_noteWasCutEvent;
            mapData[difficultyBeatmap.SerializedName()] = currentMapData;
            Plugin.log.Debug(currentMapData.ToString());
        }

        private void ScoreController_noteWasMissedEvent(NoteData noteData, int _)
        {
            if (noteData == null)
            {
                Plugin.log.Critical("NoteData is null");
            }
            NoteDataBypasser bypasser = noteData.NoteDataToBypasser();
            if (bypasser == null)
            {
                Plugin.log.Critical("Bypasser is null");
            }
            currentMapData[bypasser.Serialize()] = new Rating(true);
        }

        internal static void LogBlockInt()
        {
            noteInt += 1;
            Plugin.log.Debug($"noteInt: {noteInt}");
        }

        private void ScoreController_noteWasCutEvent(NoteData noteData, in NoteCutInfo noteCutInfo, int multiplier)
        {
            if (noteData.colorType != ColorType.None && !noteCutInfo.allIsOK)
            {
                if (noteData == null)
                {
                    Plugin.log.Critical("NoteData is null");
                }
                NoteDataBypasser bypasser = noteData.NoteDataToBypasser();
                if (bypasser == null)
                {
                    Plugin.log.Critical("Bypasser is null");
                }
                currentMapData[bypasser.Serialize()] = new Rating(true);
            }
            else
            {
                swingCounterCutInfo.Add(noteInt, noteCutInfo);
                noteCutInfoData.Add(noteCutInfo, noteData);
                CutScoreBuffer cutScoreBuffer = new CutScoreBuffer();
                cutScoreBuffer.Init(noteCutInfo);
                //noteCutInfo.swingRatingCounter.RegisterDidChangeReceiver(this);
                //noteCutInfo.swingRatingCounter.RegisterDidFinishReceiver(this);
                //int beforeCutRawScore, afterCutRawScore, cutDistanceRawScore;
                //ScoreModel.RawScoreWithoutMultiplier(noteCutInfo.swingRatingCounter, noteCutInfo.cutDistanceToCenter, out beforeCutRawScore, out afterCutRawScore, out cutDistanceRawScore);
                int angle = cutScoreBuffer.beforeCutScore + cutScoreBuffer.afterCutScore;
                Plugin.log.Debug($"noteCuteInfo.cutAngle: {noteCutInfo.cutAngle} - angleCutScoreBuffer: {angle}");
                int acc = cutScoreBuffer.centerDistanceCutScore;
                if (noteData == null)
                {
                    Plugin.log.Critical("NoteData is null");
                }
                NoteDataBypasser bypasser = noteData.NoteDataToBypasser();
                if (bypasser == null)
                {
                    Plugin.log.Critical("Bypasser is null");
                }
                currentMapData[bypasser.Serialize()] = new Rating(angle, acc);

            }
        }


        public void HandleSaberSwingRatingCounterDidChange(ISaberSwingRatingCounter saberSwingRatingCounter, float rating)
        {

            NoteCutInfo noteCutInfo;

            if (swingCounterCutInfo.TryGetValue(noteInt, out noteCutInfo))
            {
                CutScoreBuffer cutScoreBuffer = new CutScoreBuffer();
                cutScoreBuffer.Init(noteCutInfo);
                int angle = cutScoreBuffer.beforeCutScore + cutScoreBuffer.afterCutScore;
                NoteData noteData;
                if (noteCutInfoData.TryGetValue(noteCutInfo, out noteData))
                {
                    if (noteData == null)
                    {
                        Plugin.log.Critical("NoteData is null");
                    }
                    NoteDataBypasser bypasser = noteData.NoteDataToBypasser();
                    if (bypasser == null)
                    {
                        Plugin.log.Critical("Bypasser is null");
                    }
                    currentMapData[bypasser.Serialize()] = new Rating(angle, cutScoreBuffer.centerDistanceCutScore);
                }
            }
        }


        public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
        {
            Plugin.log.Debug($"Removing noteInt: {noteInt}");
            swingCounterCutInfo.Remove(noteInt);
            saberSwingRatingCounter.UnregisterDidChangeReceiver(this);
            saberSwingRatingCounter.UnregisterDidFinishReceiver(this);
        }
        public struct Rating
        {
            public Rating(int angle, int accuracy)
            {
                hitScore = angle + accuracy;
                this.angle = angle;
                this.accuracy = accuracy;
                missed = false;
                LogBlockInt();
            }

            public Rating(bool missed)
            {
                hitScore = 0;
                angle = 0;
                accuracy = 0;
                this.missed = missed;
                LogBlockInt();

            }

            public int hitScore;
            public int angle;
            public int accuracy;
            public bool missed;
        }
    }
}
