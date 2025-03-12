using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;
    private PlayerSpriteRenderer activeRenderer; //T�t� tarvitaan seuraamaan kumpaa yll�olevista rendereist� milloinkin k�ytet��n.

    private DeathAnimation deathAnimation;
    private CapsuleCollider2D capsuleCollider;  //Pienen hahmon collider on liian pieni isolle hahmolle, joten se pit�� m��ritt�� erikseen koodissa.

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

        starAudioSource = gameObject.AddComponent<AudioSource>();  //Erillinen audiokomponentti t�htivoiman ��nelle.
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
            PlayShootingSound();  //AUDIO Soita ��ni vain jos ampuminen on sallittua
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
        if (!dead && !starpower && !magicpower)  //Jos pelaaja ei ole kuollut ja sill� ei ole t�hti/taikavoimaa...
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
        if (!dead) //Varmistaa, ett� kuolemaa ei kutsuta useasti.
        {
            smallRenderer.enabled = false;  //Poistetaan pienen hahmon render�inti k�yt�st�.
            bigRenderer.enabled = false;  //Poistetaan suuren hahmon render�inti k�yt�st�.
            deathAnimation.enabled = true;  //Otetaan hahmon kuolema-animaatio k�ytt��n.
            PlaySound(death);  //AUDIO

            GameManager.Instance.ResetLevel(3f);  //Nollataan taso 3 sekuntin kuluttua. T�h�n on t�rke� m��ritt�� aika, jotta kuolema-animaatiolla on aikaa rullata l�pi.
        }    
    }

    public void Grow()
    {
        smallRenderer.enabled = false;  //Otetaan pienen hahmon renderer pois k�yt�st�.
        bigRenderer.enabled = true;  //Otetaan suuren hahmon renderer k�ytt��n.
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);  //T�ss� kasvatetaan collider yhdest� yksik�st� kahteen.
        capsuleCollider.offset = new Vector2(0f, 0.5f);  //T�ss� siirret��n collideria puoli yksikk�� yl�sp�in, jotta se kohdistuu oikein.

        StartCoroutine(ScaleAnimation());
        PlaySound(powerUp);  //AUDIO Soitetaan ��ni kun pelaaja kasvaa.
    }

    private void Shrink()
    {
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(ScaleAnimation());
        PlaySound(shrink);  //AUDIO Soitetaan ��ni kun pelaaja kutistuu.
    }

    private IEnumerator ScaleAnimation()  //T�m� tarvitsee toimiakseen using System.Collections;
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)  //Kun on kulunut v�hemm�n kuin kesto jatkamme animointia...
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

    public void Starpower(float duration = 10f)  //T�htivoima kest�� 10 sekunttia.
    {
        StartCoroutine(StarpowerCoroutine(duration));  //AUDIO
        StartCoroutine(StarpowerAnimation(duration));  //animaatio
    }

    private IEnumerator StarpowerCoroutine(float duration)  //AUDIO
    {
        PlayStarSound(starAudio);  //Soitetaan t�hden ��ni
        yield return new WaitForSeconds(duration);  //Odotetaan t�htivoiman keston ajan
        starAudioSource.Stop();  //Pys�ytet��n ��ni, kun t�htivoima loppuu
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
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);  //M��ritet��n hahmo muuttamaan v�ri� kun t�htivoima on aktivoitu. Suluissa on m��ritelty v�ris�vyt.
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;  //Palautetaan hahmon v�ri takaisin alkuper�iseksi.
        starpower = false;
    }

    public void Magicpower(float duration = 300f)  //Taikavoima kest�� 300 sekunttia.
    {
        StartCoroutine(MagicpowerCoroutine(duration));  //AUDIO
        StartCoroutine(MagicpowerAnimation(duration));  //animaatio
    }

    private IEnumerator MagicpowerCoroutine(float duration)  //AUDIO
    {
        PlayStarSound(magicPowerAudio);  //Soitetaan taikavoiman ��ni
        yield return new WaitForSeconds(duration);  //Odotetaan taikavoiman keston ajan
        starAudioSource.Stop();  //Pys�ytet��n ��ni, kun taikavoima loppuu
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

    private void PlayShootingSound()  //AUDIO Tarkista, ett� pelaaja on iso ja ��ni on m��ritelty
    {
        if (big && shootingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
    }
}
