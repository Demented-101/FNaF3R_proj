using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private SpringtrapAI springtrap;
    [SerializeField] private AudioClip[] sounds;
    private AudioSource audioSource;

    void Start()
    {
        springtrap.Moved.AddListener(PlaySound);
        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound()
    {
        audioSource.clip = sounds[Random.Range(0, sounds.Length)];
        audioSource.Play();
    }
}
