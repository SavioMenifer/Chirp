using UnityEngine;
using XRAccess.Chirp;

public class CaptionOnKeypress : MonoBehaviour
{
    public CaptionSource source;
    public CaptionSource source2;
    private string text1;
    private string text2;

    void Start()
    {
        source = GetComponent<CaptionSource>();
        text1 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed aliquam, mauris ut interdum volutpat, magna enim luctus nisl.";
        text2 = "The design is used across stationery, packaging, and engraved on the product.";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            source.ShowTimedCaption(text1, 120.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            source.ShowTimedCaption(text1, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            source.ShowTimedCaption(text1, 2f);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            source.ShowTimedCaption(text2, 3f);
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            source.ShowTimedCaption(text1, 5f);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            source2.ShowTimedCaption(text2, 5f);
        }
    }
}
