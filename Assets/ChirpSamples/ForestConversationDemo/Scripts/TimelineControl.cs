using UnityEngine;
using UnityEngine.Playables;
using XRAccess.Chirp;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector timeline;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePlayState();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            CaptionRenderManager.Instance.RefreshCaptions();
        }
    }

    public void TogglePlayState()
    {
        if (timeline.state == PlayState.Playing)
        {
            timeline.Stop();
        }
        else if (timeline.state == PlayState.Paused)
        {
            timeline.Play();
        }
    }
}
