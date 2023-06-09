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
            if (timeline.state == PlayState.Playing)
            {
                timeline.Stop();
            }
            else if (timeline.state == PlayState.Paused)
            {
                timeline.Play();
            }

        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            var currentCaptions = CaptionRenderManager.Instance.currentCaptions;
            CaptionRenderManager.Instance.currentRenderer.RefreshCaptions(currentCaptions);
        }
    }
}
