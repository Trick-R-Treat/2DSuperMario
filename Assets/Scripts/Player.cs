using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;
    private PlayerSpriteRenderer activeRenderer; //Tätä tarvitaan seuraamaan kumpaa ylläolevista rendereistä milloinkin käytetään.

    private DeathAnimation deathAnimation;
    private CapsuleCollider2D capsuleCollider;  //Pienen hahmon collider on liian pieni isolle hahmolle, joten se pitää määrittää erikseen koodissa.

    public bool big => bigRenderer.enabled;
    public bool small => smallRenderer.enabled;
    public bool dead => deathAnimation.enabled;
    public bool starpower { get; private set; }
    public bool magicpower { get; private set; }

    private DeathBarrier deathBarrier;

    public GameObject bulletPrefab;
    public Transform shootingPoint;  //Paikka, josta ammukset ammutaan

    //AUDIO
    public AudioClip jump;
    public AudioClip death;
    public AudioClip powerUp;
    public AudioClip shrink;
    public AudioClip starAudio;
    public AudioClip magicPowerAudio;
    private AudioSource audioSource;
    private AudioSource starAudioSource;
    public AudioClip shootingSound;

    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        activeRenderer = smallRenderer;

        starAudioSource = gameObject.AddComponent<AudioSource>();  //Erillinen audiokomponentti tähtivoiman äänelle.
    }

    void Start()  //AUDIO
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()  //AUDIO
    {
        if (Input.GetButtonDown("Jump"))
        {
            PlaySound(jump);
        }
        
        if (Input.GetKeyDown(KeyCode.X) && big)  //Pelaaja voi ampua vain jos hahmo on iso
        {
            Shoot();
            PlayShootingSound();  //AUDIO Soita ääni vain jos ampuminen on sallittua
        }

    }

    void PlaySound(AudioClip clip)  //AUDIO
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    void PlayStarSound(AudioClip clip)  //STAR AUDIO
    {
        starAudioSource.clip = clip;
        starAudioSource.Play();
    }

    public void Hit()  //Kun pelaaja osuu viholliseen...
    {
        if (!dead && !starpower && !magicpower)  //Jos pelaaja ei ole kuollut ja sillä ei ole tähti/taikavoimaa...
        {
            if (big)  //Jos pelaaja on iso...
            {
                Shrink();  //...se pienenee.
            }

            else  //Muuten...
            {
                Death();  //...se kuolee.
            }
        }
    }

    private void Death()
    {
        if (!dead) //Varmistaa, että kuolemaa ei kutsuta useasti.
        {
            smallRenderer.enabled = false;  //Poistetaan pienen hahmon renderöinti käytöstä.
            bigRenderer.enabled = false;  //Poistetaan suuren hahmon renderöinti käytöstä.
            deathAnimation.enabled = true;  //Otetaan hahmon kuolema-animaatio käyttöön.
            PlaySound(death);  //AUDIO

            GameManager.Instance.ResetLevel(3f);  //Nollataan taso 3 sekuntin kuluttua. Tähän on tärkeä määrittää aika, jotta kuolema-animaatiolla on aikaa rullata läpi.
        }    
    }

    public void Grow()
    {
        smallRenderer.enabled = false;  //Otetaan pienen hahmon renderer pois käytöstä.
        bigRenderer.enabled = true;  //Otetaan suuren hahmon renderer käyttöön.
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);  //Tässä kasvatetaan collider yhdestä yksiköstä kahteen.
        capsuleCollider.offset = new Vector2(0f, 0.5f);  //Tässä siirretään collideria puoli yksikköä ylöspäin, jotta se kohdistuu oikein.

        StartCoroutine(ScaleAnimation());
        PlaySound(powerUp);  //AUDIO Soitetaan ääni kun pelaaja kasvaa.
    }

    private void Shrink()
    {
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(ScaleAnimation());
        PlaySound(shrink);  //AUDIO Soitetaan ääni kun pelaaja kutistuu.
    }

    private IEnumerator ScaleAnimation()  //Tämä tarvitsee toimiakseen using System.Collections;
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)  //Kun on kulunut vähemmän kuin kesto jatkamme animointia...
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                smallRenderer.enabled = !smallRenderer.enabled;
                bigRenderer.enabled = !smallRenderer.enabled;
            }

            yield return null;
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        activeRenderer.enabled = true;
    }

    public void Starpower(float duration = 10f)  //Tähtivoima kestää 10 sekunttia.
    {
        StartCoroutine(StarpowerCoroutine(duration));  //AUDIO
        StartCoroutine(StarpowerAnimation(duration));  //animaatio
    }

    private IEnumerator StarpowerCoroutine(float duration)  //AUDIO
    {
        PlayStarSound(starAudio);  //Soitetaan tähden ääni
        yield return new WaitForSeconds(duration);  //Odotetaan tähtivoiman keston ajan
        starAudioSource.Stop();  //Pysäytetään ääni, kun tähtivoima loppuu
    }

    private IEnumerator StarpowerAnimation(float duration)
    {
        starpower = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);  //Määritetään hahmo muuttamaan väriä kun tähtivoima on aktivoitu. Suluissa on määritelty värisävyt.
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;  //Palautetaan hahmon väri takaisin alkuperäiseksi.
        starpower = false;
    }

    public void Magicpower(float duration = 300f)  //Taikavoima kestää 300 sekunttia.
    {
        StartCoroutine(MagicpowerCoroutine(duration));  //AUDIO
        StartCoroutine(MagicpowerAnimation(duration));  //animaatio
    }

    private IEnumerator MagicpowerCoroutine(float duration)  //AUDIO
    {
        PlayStarSound(magicPowerAudio);  //Soitetaan taikavoiman ääni
        yield return new WaitForSeconds(duration);  //Odotetaan taikavoiman keston ajan
        starAudioSource.Stop();  //Pysäytetään ääni, kun taikavoima loppuu
    }

    private IEnumerator MagicpowerAnimation(float duration)
    {
        magicpower = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }

    private void Shoot()
    {
        if (big)
        {
            Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        }
        
        //else
        //{
        //    {
        //        Debug.Log("Et voi ampua, koska hahmo on pieni!");
        //    }
        //}
    }

    private void PlayShootingSound()  //AUDIO Tarkista, että pelaaja on iso ja ääni on määritelty
    {
        if (big && shootingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
    }
}
