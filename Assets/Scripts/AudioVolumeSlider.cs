using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
    public const string VolumePreferenceKey = "AudioVolume";

    [SerializeField] private Slider slider;
    [SerializeField] private string volumePreferenceKey = VolumePreferenceKey;
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 1f;

    private void Awake()
    {
        EnsureSliderReference();
        SyncSliderToSavedVolume();
    }

    private void OnEnable()
    {
        EnsureSliderReference();
        SyncSliderToSavedVolume();

        if (slider != null)
            slider.onValueChanged.AddListener(SetVolume);
    }

    private void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        ApplyVolume(volume);
        PlayerPrefs.SetFloat(volumePreferenceKey, AudioListener.volume);
        PlayerPrefs.Save();
    }

    public static void ApplySavedVolume()
    {
        ApplyVolume(PlayerPrefs.GetFloat(VolumePreferenceKey, 1f));
    }

    private void SyncSliderToSavedVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(volumePreferenceKey, defaultVolume);
        ApplyVolume(savedVolume);

        if (slider != null)
            slider.SetValueWithoutNotify(AudioListener.volume);
    }

    private static void ApplyVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    private void EnsureSliderReference()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }
}
