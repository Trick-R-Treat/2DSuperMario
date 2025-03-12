using UnityEngine;

public static class Extensions  //Laajennusmenetelm�t vaativat, ett� ne m��ritell��n staattisessa luokassa, jota se ei peri mist��n.
{

    //Pelaajan Layer (oletustaso) on k�yty Inspectorissa muuttamassa Defaultista Playeriksi ja t�ss� pyydet��n sen "maski" hakemaan.
    private static LayerMask layerMask = LayerMask.GetMask("Default");

    //T�ss� halutaan laajentaa Rigidbody2D ja l�hett�� se Vector2 suuntaan. Raycast on "kutsumanimi" jolla voidaan t�t� kutsua t�ss� ja muissa scripteissa.
    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        //Kinematic tarkoittaa, ett� fysiikan moottori ei ohjaa sit�.
        if (rigidbody.isKinematic)  //Jos hahmo on paikoillaan...
        {
            return false;   //...palautetaan arvo ep�tosi.
        }
        float radius = 0.25f;  //Hahmon koko on 1 yksikk�, joten s�de on nelj�sosa siit�.
        float distance = 0.375f;

        //K�ytet��n Unityn omaa fysiikkaa, jossa huomioidaan hahmon sijainti, s�de, suunta ja et�isyys.
        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection)
    {
        Vector2 direction = other.position - transform.position;
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }
}