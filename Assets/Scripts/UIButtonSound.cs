using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField] 
    private AudioClip clip;
    [SerializeField, Range(0f, 1f)] 
    private float volume = 0.4f;
    [SerializeField, Range(0.1f, 3f)] 
    private float pitch = 1f;

    public void Play()
    {
        if (clip == null) return;
        var go = new GameObject("TempUIAudio_" + clip.name);
        go.transform.SetParent(null);
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.spatialBlend = 0f;
        src.playOnAwake = false;
        src.pitch = pitch;
        src.Play();
        Destroy(go, clip.length / Mathf.Abs(src.pitch) + 0.1f);
    }

}