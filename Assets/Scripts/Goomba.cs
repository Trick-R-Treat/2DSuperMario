using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Sprite flatSprite;
    public AudioClip deathSound;  //AUDIO
    private AudioSource audioSource;  //AUDIO

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); //AUDIO Hae AudioSource-komponentti
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  //Jos Goomba t�rm�� pelaajan kanssa...
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower || player.magicpower)  //Jos pelaajalla on t�hti/taikavoima...
            {
                Hit();  //...Goomba saa osuman.
                GameManager.Instance.AddScore(100);
            }
            else if (collision.transform.DotTest(transform, Vector2.down))  //Jos pelaaja hypp�� Goomban p��lle...
            {
                Flatten();  //...Goomba litistyy.
                GameManager.Instance.AddScore(100);
            }
            else  //Muuten...
            {
                player.Hit();  //...pelaaja saa osuman.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell"))  //Jos Koopan kuori osuu Goombaan...
        {
            Hit();  //...Goomba saa osuman.
        }

        if (other.CompareTag("Bullet")) //Jos ammus osuu...
        {
            Hit(); //Goomba saa osuman
            GameManager.Instance.AddScore(100);
            Destroy(other.gameObject); //Tuhoa ammus
        }
    }

    private void Flatten()
    {
        GetComponent<Collider2D>().enabled = false;  //Poistetaan t�rm�ys k�yt�st�.
        GetComponent<EntityMovement>().enabled = false;  //Poistetaan liikkuminen k�yt�st�.
        GetComponent<AnimatedSprite>().enabled = false;  //Poistetaan animaatiot k�yt�st�.
        GetComponent<SpriteRenderer>().sprite = flatSprite;  //Haetaan hahmon litistetty muoto.

        PlayDeathSound(); //AUDIO Soita kuolema��ni

        Destroy(gameObject, 0.5f);  //Poistetaan Goomba n�kyvist� puolen sekuntin kuluttua.
    }

    private void Hit()
    {
        GetComponent<AnimatedSprite>().enabled = false;  //Poistetaan animaatiot k�yt�st�.
        GetComponent<DeathAnimation>().enabled = true;  //Toteutetaan kuoleman animaatio.

        PlayDeathSound(); //AUDIO Soita kuolema��ni

        Destroy(gameObject, 3f);  //Poistetaan Goomba n�kyvist� 3 sekuntin kuluttua.
    }

    private void PlayDeathSound()  //AUDIO
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
