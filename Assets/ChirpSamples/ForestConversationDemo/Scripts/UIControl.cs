using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XRAccess.Chirp;

public class UIControl : MonoBehaviour
{
    public Toggle captionsToggle;

    public Button fontColorButton;
    public TMP_Dropdown fontStyleDropdown;
    public TMP_InputField fontSizeField;

    public Button outlineColorButton;
    public TMP_InputField outlineWidthField;

    public Button bgColorButton;
    public Slider bgOpacity;

    public TMP_InputField lagField;
    public Toggle arrowsToggle;

    public GameObject colorPickerUI;
    public ColorPicker colorPicker;

    public TMP_FontAsset[] fonts;

    private CaptionOptions _options;
    private HeadLockedOptions _rendererOptions;

    private Button _selectedColorButton;

    public void Start()
    {
        StartCoroutine(LoadInitialState());

        fontColorButton.onClick.AddListener(delegate { ShowColorPicker(_options.fontColor, fontColorButton); });
        outlineColorButton.onClick.AddListener(delegate { ShowColorPicker(_options.outlineColor, outlineColorButton); });
        bgColorButton.onClick.AddListener(delegate { ShowColorPicker(_options.backgroundMaterial.color, bgColorButton); });
    }

    private void ShowColorPicker(Color startColor, Button button)
    {
        _selectedColorButton = button;
        Color noAlpha = new Color(startColor.r, startColor.g, startColor.b, 1f);

        if (!colorPickerUI.activeSelf)
        {
            colorPickerUI.SetActive(true);
        }

        colorPicker.color = startColor;
    }

    public void HideColorPicker()
    {
        colorPickerUI.SetActive(false);
    }

    public void ColorPicked()
    {
        Color currentColor = colorPicker.color;

        if (_selectedColorButton == fontColorButton)
        {
            _options.fontColor = currentColor;
            fontColorButton.GetComponent<Image>().color = currentColor;
        }
        else if (_selectedColorButton == bgColorButton)
        {
            _options.backgroundMaterial.color = new Color(currentColor.r, currentColor.g, currentColor.b, _options.backgroundMaterial.color.a);
            bgColorButton.GetComponent<Image>().color = currentColor;
        }
        else if (_selectedColorButton == outlineColorButton)
        {
            _options.outlineColor = currentColor;
            outlineColorButton.GetComponent<Image>().color = currentColor;
        }

        RefreshCaptions();
        HideColorPicker();
    }

    public void ToggleCaptions()
    {
        if (captionsToggle.isOn)
        {
            CaptionRenderManager.Instance.EnableRenderer(PositioningMode.HeadLocked);
            StartCoroutine(LoadInitialState());
        }
        else
        {
            CaptionRenderManager.Instance.DestroyCurrentRenderer();
        }
    }

    public void ToggleArrows()
    {
        _rendererOptions.showIndicatorArrows = arrowsToggle.isOn;
        RefreshCaptions();
    }

    public void SetFontAsset()
    {
        _options.fontAsset = fonts[fontStyleDropdown.value];
        RefreshCaptions();
    }

    public void AddToFontSize(int value)
    {
        _options.fontSize += value;
        fontSizeField.GetComponent<TMP_InputField>().text = _options.fontSize.ToString("0");
        RefreshCaptions();
    }

    public void AddToOutlineWidth(float value)
    {
        _options.outlineWidth += value;
        outlineWidthField.GetComponent<TMP_InputField>().text = _options.outlineWidth.ToString("F2");
        RefreshCaptions();
    }

    public void AddToLag(float value)
    {
        _rendererOptions.lag += value;
        lagField.GetComponent<TMP_InputField>().text = _rendererOptions.lag.ToString("F2");
        RefreshCaptions();
    }

    public void SetBackgroundOpacity()
    {
        Color bgColor = _options.backgroundMaterial.color;
        _options.backgroundMaterial.color = new Color(bgColor.r, bgColor.g, bgColor.b, bgOpacity.value);
    }

    private IEnumerator LoadInitialState()
    {
        yield return new WaitUntil(() => (CaptionRenderManager.Instance != null && CaptionSystem.Instance != null));

        _options = CaptionSystem.Instance.options;

        var currentrenderer = CaptionRenderManager.Instance.currentRenderer;
        _rendererOptions = (HeadLockedOptions)currentrenderer.options;

        if (CaptionRenderManager.Instance.currentRenderer == null)
        {
            captionsToggle.isOn = false;
        }

        fontColorButton.GetComponent<Image>().color = _options.fontColor;
        fontSizeField.GetComponent<TMP_InputField>().text = _options.fontSize.ToString("0");

        outlineColorButton.GetComponent<Image>().color = _options.outlineColor;
        outlineWidthField.GetComponent<TMP_InputField>().text = _options.outlineWidth.ToString("F2");

        Color bgColor = _options.backgroundMaterial.color;
        bgColorButton.GetComponent<Image>().color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f); ;
        bgOpacity.GetComponent<Slider>().value = bgColor.a;

        lagField.GetComponent<TMP_InputField>().text = _rendererOptions.lag.ToString("F2");
        arrowsToggle.GetComponent<Toggle>().isOn = _rendererOptions.showIndicatorArrows;

        List<string> fontNames = new List<string>();
        foreach (TMP_FontAsset font in fonts)
        {
            fontNames.Add(font.faceInfo.familyName);
        }
        fontStyleDropdown.AddOptions(fontNames);
        SetFontAsset();
    }

    private void RefreshCaptions()
    {
        CaptionRenderManager.Instance.RefreshCaptions();
    }
}
