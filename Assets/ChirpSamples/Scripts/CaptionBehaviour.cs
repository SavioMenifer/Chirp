using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Playables;

using XRAccess.Chirp;
using SimpleSRT;

public class CaptionBehaviour : PlayableBehaviour
{
    public TextAsset captionFile;

    private List<SubtitleBlock> _captions;
    private int _currentIndex = 0;

    public override void OnGraphStart(Playable playable)
    {
        if (captionFile != null)
        {
            _captions = SRTParser.Load(captionFile);
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_captions == null || _captions.Count == 0 || CaptionSystem.Instance == null)
        {
            return;
        }

        CaptionSource source = playerData as CaptionSource;

        float currentTime = (float)playable.GetTime();

        while (_currentIndex < _captions.Count && currentTime >= (float)_captions[_currentIndex].From)
        {
            var caption = _captions[_currentIndex];
            string removedLineBreaks = Regex.Replace(caption.Text, @"\r\n?|\n", " "); // temp for demo
            source.ShowTimedCaption(removedLineBreaks, (float)caption.Length);
            _currentIndex++;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        CaptionRenderManager.Instance?.ClearCaptions(); //temp for demo
    }
}
