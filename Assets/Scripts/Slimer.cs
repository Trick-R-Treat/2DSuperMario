using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slimer : MonoBehaviour
{
    public AudioClip deathSound;  //AUDIO
    private AudioSource audioSource;  //AUDIO

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); //AUDIO
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  //Jos Slimer t�rm�� pelaajan kanssa...
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower | player.magicpower)  //Jos pelaajalla on t�hti/taikavoima...
            {
                Hit();  //...Slimer saa osuman.
            }
            else  //Muuten...
            {
                player.Hit();  //...pelaaja saa osuman.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell"))  //Jos kuori osuu Slimeriin...
        {
            Hit();  //...Slimer saa osuman.
        }

        if (other.CompareTag("Bullet")) //Jos ammus osuu...
        {
            Hit(); //Slimer saa osuman
            GameManager.Instance.AddScore(100);
            Destroy(other.gameObject); //Tuhoa ammus
        }
    }

    private void Hit()
    {
        GetComponent<AnimatedSprite>().enabled = false;  //Poistetaan animaatiot k�yt�st�.
        GetComponent<DeathAnimation>().enabled = true;  //Toteutetaan kuoleman animaatio.

        PlayDeathSound(); //Soita kuolema��ni

        Destroy(gameObject, 3f);  //Poistetaan Slimer n�kyvist� 3 sekuntin kuluttua.
    }

    private void PlayDeathSound()
    {
        if (deathSound != null)
        {
            GameObject soundObject = new GameObject("DeathSound");
            AudioSource tempAudio = soundObject.AddComponent<AudioSource>();
            tempAudio.clip = deathSound;
            tempAudio.Play();
            Destroy(soundObject, deathSound.length); //Tuhoa ��niobjekti, kun ��ni on toistettu
        }
    }
}
