using UnityEngine;
using UnityEngine.Playables;

public class CaptionClip : PlayableAsset
{
    public Object captionFile;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CaptionBehaviour>.Create(graph);

        CaptionBehaviour captionBehaviour = playable.GetBehaviour();
        captionBehaviour.captionFile = captionFile;

        return playable;
    }
}
