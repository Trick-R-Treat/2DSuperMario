using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : MonoBehaviour
{
    public Sprite shellSprite;
    public float shellSpeed = 12f;  //Kuoren liikenopeus on t�ss� m��ritetty 12f.

    private bool shelled;
    private bool pushed;

    public AudioClip angrySound;  //AUDIO
    private AudioSource audioSource;  //AUDIO

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); //AUDIO
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shelled && collision.gameObject.CompareTag("Player"))  //Jos etana ei ole kuoressaan ja t�rm�� pelaajan kanssa...
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower | player.magicpower)  //Jos pelaajalla on t�hti/taikavoima...
            {
                Hit();  //...etana saa osuman.
                GameManager.Instance.AddScore(100);
            }
            else if (collision.transform.DotTest(transform, Vector2.down))  //Jos pelaaja hypp�� etanan p��lle...
            {
                EnterShell();  //...etana menee kuoreensa sis�lle.
            }
            else  //Muuten...
            {
                player.Hit();  //...pelaaja saa osuman.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shelled && other.CompareTag("Player"))  //Jos etana on kuoressaan ja pelaaja t�rm�� siihen... 
        {
            if (!pushed)  //Jos kuorta ei ole ty�nnetty eli se ei liiku...
            {
                Vector2 direction = new Vector2(transform.position.x - other.transform.position.x, 0f);
                PushShell(direction);  //...kuori l�htee liikkumaan samaan suuntaan mihin sit� ty�nnettiin.
            }
            else  //Muuten...
            {
                Player player = other.GetComponent<Player>();

                if (player.starpower | player.magicpower)  //Jos pelaajalla on t�hti/taikavoima...
                {
                    Hit();  //...etana saa osuman.
                    GameManager.Instance.AddScore(100);
                }
                else  //Muuten...
                {
                    player.Hit();  //...pelaaja saa osuman.
                }
            }
        }

        else if (!shelled && other.gameObject.layer == LayerMask.NameToLayer("Shell"))  //Jos etana ei ole kuoressaan ja toinen kuori osuu siihen...
        {
            Hit();  //...etana saa osuman.
        }

        //LIS�TTY
        if (!shelled && other.CompareTag("Bullet")) //Jos Snail ei ole kuoressaan ja ammus osuu siihen...
        {
            EnterShell(); //Snail menee kuoreen sis�lle
            PlayAngrySound(); //AUDIO
            Destroy(other.gameObject); //Tuhoa ammus
        }
    }

    private void EnterShell()
    {
        shelled = true;

        GetComponent<EntityMovement>().enabled = false;  //Poistetaan liikkuminen k�yt�st�.
        GetComponent<AnimatedSprite>().enabled = false;  //Poistetaan animaatiot k�yt�st�.
        GetComponent<SpriteRenderer>().sprite = shellSprite;  //Haetaan hahmon kuorimuoto.
    }

    private void PushShell(Vector2 direction)
    {
        pushed = true;  //Kuorta on ty�nnetty.
        GetComponent<Rigidbody2D>().isKinematic = false;  //Otetaan Rigidbody uudelleen k�ytt��n ja poistetaan kinemaattisuus, jotta fysiikan moottori k�sittelee liikkeen.
        EntityMovement movement = GetComponent<EntityMovement>();  //Otetaan liikkuminen uudelleen k�ytt��n.
        movement.direction = direction.normalized;  //Kuori liikkuu samaan suuntaan mihin sit� ty�nnettiin.
        movement.speed = shellSpeed;  //Nopeus on yht� suuri kuin kuoren nopeus.
        movement.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Shell");
    }

    private void Hit()
    {
        GetComponent<AnimatedSprite>().enabled = false;  //Poistetaan animaatiot k�yt�st�.
        GetComponent<DeathAnimation>().enabled = true;  //Toteutetaan kuoleman animaatio.

        PlayAngrySound(); //AUDIO

        Destroy(gameObject, 3f);  //Poistetaan etana n�kyvist� 3 sekuntin kuluttua.
    }

    private void PlayAngrySound()  //AUDIO
    {
        if (angrySound != null)
        {
            GameObject soundObject = new GameObject("AngrySound");
            AudioSource tempAudio = soundObject.AddComponent<AudioSource>();
            tempAudio.clip = angrySound;
            tempAudio.Play();
            Destroy(soundObject, angrySound.length); //Tuhoa ��niobjekti, kun ��ni on toistettu
        }
    }
}
