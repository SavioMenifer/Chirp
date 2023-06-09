using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;
using XRAccess.Chirp;

public class CaptionBehaviour : PlayableBehaviour
{
    public UnityEngine.Object captionFile;

    private SubParser _parser;
    private List<SubtitleItem> _captions;
    private int _currentIndex = 0;

    public override void OnGraphStart(Playable playable)
    {
        if (captionFile != null)
        {
            _parser = new SubParser();

            string filePath = AssetDatabase.GetAssetPath(captionFile);
            string fileName = Path.GetFileName(filePath);

            using (var fileStream = File.OpenRead(filePath))
            {
                try
                {
                    var mostLikelyFormat = _parser.GetMostLikelyFormat(fileName);
                    _captions = _parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
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

        while (_currentIndex < _captions.Count && currentTime >= _captions[_currentIndex].StartTime / 1000f)
        {
            var caption = _captions[_currentIndex];
            float duration = (caption.EndTime - caption.StartTime) / 1000f;
            source.ShowTimedCaption(string.Join("\n", caption.Lines), duration);
            _currentIndex++;
        }
    }
}
