using System.Reflection;
using static NoteData;

namespace NiceMiss
{
    public class NoteDataBypasser : NoteData
    {
        public NoteData noteData;

        
        public NoteDataBypasser(float time, int lineIndex, NoteLineLayer noteLineLayer, NoteLineLayer beforeJumpNoteLineLayer, GameplayType gameplayType, ScoringType scoringType, ColorType colorType, NoteCutDirection cutDirection, float timeToNextColorNote, float timeToPrevColorNote, int flipLineIndex, float flipYSide, float cutDirectionAngleOffset, float cutSfxVolumeMultiplier) : base(time, lineIndex, noteLineLayer, beforeJumpNoteLineLayer, gameplayType, scoringType, colorType, cutDirection, timeToNextColorNote, timeToPrevColorNote, flipLineIndex, flipYSide, cutDirectionAngleOffset, cutSfxVolumeMultiplier)
        {
            var bypasser = (NoteData)typeof(NoteData)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                    null,
                  new[] { typeof(float), typeof(int), typeof(NoteLineLayer), typeof(NoteLineLayer), typeof(GameplayType), typeof(ScoringType), typeof(ColorType), typeof(NoteCutDirection), typeof(float), typeof(float), typeof(int), typeof(float), typeof(float), typeof(float) },
                  null
                )
                .Invoke(new object[] { time, lineIndex, noteLineLayer, beforeJumpNoteLineLayer, gameplayType, scoringType, colorType, cutDirection, timeToNextColorNote, timeToPrevColorNote, flipLineIndex, flipYSide, cutDirectionAngleOffset, cutSfxVolumeMultiplier });


            noteData = bypasser;
        }
        public string Serialize()
        {
             
            string ret = noteData.time.ToString() + "|" + noteData.lineIndex.ToString()+ "|" + noteData.noteLineLayer.ToString() + "|" + noteData.beforeJumpNoteLineLayer.ToString() + "|" + noteData.gameplayType.ToString() + "|" + noteData.scoringType.ToString() + "|" + noteData.colorType.ToString() + "|" + noteData.cutDirection.ToString() + "|" + noteData.timeToNextColorNote.ToString() + "|" + noteData.timeToPrevColorNote.ToString() + "|" + noteData.flipLineIndex.ToString() + "|" + noteData.flipYSide.ToString() + "|" + noteData.cutDirectionAngleOffset.ToString() + "|" + noteData.cutSfxVolumeMultiplier.ToString();
            return ret;
        }

        public static NoteDataBypasser Deserialize(string data)
        {
            string[] split = data.Split('|');
            return new NoteDataBypasser(float.Parse(split[0]), int.Parse(split[1]), NiceMissExtensionMethods.NoteLineLayerParse(split[2]), NiceMissExtensionMethods.NoteLineLayerParse(split[3]), NiceMissExtensionMethods.GameplayTypeParse(split[4]), NiceMissExtensionMethods.ScoringTypeParse(split[5]), NiceMissExtensionMethods.ColorTypeParse(split[6]), NiceMissExtensionMethods.NoteCutDirectionParse(split[7]), float.Parse(split[8]), float.Parse(split[9]), int.Parse(split[10]), float.Parse(split[11]), float.Parse(split[12]), float.Parse(split[13]));
        }
    }

    public static class NiceMissExtensionMethods
    {
        public static NoteDataBypasser NoteDataToBypasser(this NoteData noteData)
        {
            NoteDataBypasser bypasser = new NoteDataBypasser(noteData.time, noteData.lineIndex, noteData.noteLineLayer, noteData.beforeJumpNoteLineLayer, noteData.gameplayType, noteData.scoringType, noteData.colorType, noteData.cutDirection, noteData.timeToNextColorNote, noteData.timeToPrevColorNote, noteData.flipLineIndex, noteData.flipYSide, noteData.cutDirectionAngleOffset, noteData.cutSfxVolumeMultiplier);
            return bypasser;
        }

        public static NoteLineLayer NoteLineLayerParse(string s)
        {
            s = s.Replace("NoteLineLayer", "");

            switch (s)
            {
                case "Base":
                    return NoteLineLayer.Base;
                case "0":
                    return NoteLineLayer.Base;
                case "Upper":
                    return NoteLineLayer.Upper;
                case "1":
                    return NoteLineLayer.Upper;
                case "Top":
                    return NoteLineLayer.Top;
                case "2":
                    return NoteLineLayer.Top;
            }
            return 0;
        }
        public static GameplayType GameplayTypeParse(string s)
        {
            s = s.Replace("GameplayType", "");
            
            switch (s)
            {
                case "Normal":
                    return GameplayType.Normal;
                case "0":
                    return GameplayType.Normal;
                case "Bomb":
                    return GameplayType.Bomb;
                case "1":
                    return GameplayType.Bomb;
                case "BurstSliderHead":
                    return GameplayType.BurstSliderHead;
                case "2":
                    return GameplayType.BurstSliderHead;
                case "BurstSliderElement":
                    return GameplayType.BurstSliderElement;
                case "3":
                    return GameplayType.BurstSliderElement;
                case "BurstSliderElementFill":
                    return GameplayType.BurstSliderElementFill;
                case "4":
                    return GameplayType.BurstSliderElementFill;

            }
            return 0;
        }
        public static ScoringType ScoringTypeParse(string s)
        {
            s = s.Replace("ScoringType", "");

            switch (s)
            {
                case "Ignore":
                    return ScoringType.Ignore;
                case "-1":
                    return ScoringType.Ignore;
                case "NoScore":
                    return ScoringType.NoScore;
                case "0":
                    return ScoringType.NoScore;
                case "Normal":
                    return ScoringType.Normal;
                case "1":
                    return ScoringType.Normal;
                case "SliderHead":
                    return ScoringType.SliderHead;
                case "2":
                    return ScoringType.SliderHead;
                case "SliderTail":
                    return ScoringType.SliderTail;
                case "3":
                    return ScoringType.SliderTail;
                case "BurstSliderHead":
                    return ScoringType.BurstSliderHead;
                case "4":
                    return ScoringType.BurstSliderHead;
                case "BurstSliderElement":
                    return ScoringType.BurstSliderElement;
                case "5":
                    return ScoringType.BurstSliderElement;
            }

            return 0;
        }

        public static ColorType ColorTypeParse(string s)
        {
            s = s.Replace("ColorType", "");

            switch (s)
            {
                case "None":
                    return ColorType.None;
                case "-1":
                    return ColorType.None;
                case "ColorA":
                    return ColorType.ColorA;
                case "0":
                    return ColorType.ColorA;
                case "ColorB":
                    return ColorType.ColorB;
                case "1":
                    return ColorType.ColorB;

            }

            return 0;
        }

        public static NoteCutDirection NoteCutDirectionParse(string s)
        {
            s = s.Replace("NoteCutDirection", "");

            switch (s)
            {
                case "Up":
                    return NoteCutDirection.Up;
                case "0":
                    return NoteCutDirection.Up;
                case "Down":
                    return NoteCutDirection.Down;
                case "1":
                    return NoteCutDirection.Down;
                case "Left":
                    return NoteCutDirection.Left;
                case "2":
                    return NoteCutDirection.Left;
                case "Right":
                    return NoteCutDirection.Right;
                case "3":
                    return NoteCutDirection.Right;
                case "UpLeft":
                    return NoteCutDirection.UpLeft;
                case "4":
                    return NoteCutDirection.UpLeft;
                case "UpRight":
                    return NoteCutDirection.UpRight;
                case "5":
                    return NoteCutDirection.UpRight;
                case "DownLeft":
                    return NoteCutDirection.DownLeft;
                case "6":
                    return NoteCutDirection.DownLeft;
                case "DownRight":
                    return NoteCutDirection.DownRight;
                case "7":
                    return NoteCutDirection.DownRight;
                case "Any":
                    return NoteCutDirection.Any;
                case "8":
                    return NoteCutDirection.Any;
                case "None":
                    return NoteCutDirection.None;
                case "9":
                    return NoteCutDirection.None;
            }

            return 0;
        }
    }
}
