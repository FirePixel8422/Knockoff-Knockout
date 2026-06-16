using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip hitAudioClip;
    [SerializeField] AudioClip punchAudioClip;
    [SerializeField] AudioClip kickAudioClip;
    [SerializeField] AudioClip blockAudioClip;

    [Header("Pitch Range")]
    [SerializeField] MinMaxFloat hitPitchRange;
    [SerializeField] MinMaxFloat punchPitchRange;
    [SerializeField] MinMaxFloat kickPitchRange;
    [SerializeField] MinMaxFloat blockPitchRange;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        AudioManager.HitSFX += OnHitSFX;
        AudioManager.PunchSFX += OnPunchSFX;
        AudioManager.KickSFX += OnKickSFX;
        AudioManager.BlockSFX += OnBlockSFX;
    }
    [InspectorButton("Play Hit SFX")]
    private void OnHitSFX()
    {
        audioSource.PlayOneShotClipWithPitch(hitAudioClip, EzRandom.Range(hitPitchRange));
    }
    [InspectorButton("Play Punch SFX")]
    private void OnPunchSFX()
    {
        audioSource.PlayOneShotClipWithPitch(punchAudioClip, EzRandom.Range(punchPitchRange));
    }
    [InspectorButton("Play Kick SFX")]
    private void OnKickSFX()
    {

        audioSource.PlayOneShotClipWithPitch(kickAudioClip, EzRandom.Range(kickPitchRange));
    }
    [InspectorButton("Play Block SFX")]
    private void OnBlockSFX()
    {
        audioSource.PlayOneShotClipWithPitch(blockAudioClip, EzRandom.Range(blockPitchRange));
    }
    private void OnDestroy()
    {
        AudioManager.HitSFX -= OnHitSFX;
        AudioManager.PunchSFX -= OnPunchSFX;
        AudioManager.KickSFX -= OnKickSFX;
        AudioManager.BlockSFX -= OnBlockSFX;
    }
}
