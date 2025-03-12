using System.Collections;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    public GameObject item;  //Kolikko
    public Sprite emptyBlock;
    public int maxHits = -1;  //Negatiivinen luku antaa mahdollisuuden iske� kohdetta loputtomasti.

    private bool animating;
    private AnimatedSprite animatedSprite;  //Viittaus AnimatedSprite-skriptiin

    private void Start()
    {
        animatedSprite = GetComponent<AnimatedSprite>();  //Hakee AnimatedSprite-skriptin samasta objektista
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))  //Jos kohde ei ole animoitu ja maksimi osuma ei ole sama kuin nolla ja kyseess� on pelaaja...
        {
            if (collision.transform.DotTest(transform, Vector2.up))  //Jos t�rm�ys kohdistuu yl�sp�in...
            {
                Hit();  //...kohdetta voi ly�d�.
            }
        }
    }

    private void Hit()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;  //MysteryBlock >> Inspector > ota t�pp� pois Sprite Renderer nimen vierest�, niin MysteryBoxista tulee n�kym�t�n. 
        
        maxHits--;

        if (maxHits == 0)
        {
            spriteRenderer.sprite = emptyBlock;

            if (animatedSprite != null)
            {
                animatedSprite.enabled = false;  //Poistaa AnimatedSprite-skriptin k�yt�st�
            }
        }
        
        if (item != null)  //Kolikko
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()  //T�m� vaatii koodin alkuun using System.Collections;
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;

        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }
}

