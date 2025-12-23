using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private SpringtrapAI springtrap;
    [SerializeField] private AudioClip[] sounds;
    private AudioSource audioSource;
    [SerializeField] private bool isVentSFX;

    void Start()
    {
        springtrap.Moved.AddListener(PlaySound);
        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound()
    {
        if (isVentSFX)
        {
            if (springtrap.attackProgress != 2) return;
            if (springtrap.attackDirection != SpringtrapAI.AttackDirection.VentA && springtrap.attackDirection != SpringtrapAI.AttackDirection.VentB) return; 

            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.Play();
        }
        else
        {
            if (springtrap.attackProgress > 0) return;
            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.Play();
        }

            
    }
}
