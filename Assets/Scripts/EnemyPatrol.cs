using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;  //Luodaan Inspectoriin laatikot, johon PatrolPoint1 ja 2 raahataan.
    public float moveSpeed;  //Luodaan Inspectoriin laatikko vihollisen liikkumisnopeuden määrittämiselle.
    public int patrolDestination;  //Luodaan Inspectoriin laatikko partiointi määränpäälle.

    void Update()
    {
        if(patrolDestination == 0)  //Jos partiointi määränpää on nolla (Inspectorissa Element 0)...
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[0].position, moveSpeed * Time.deltaTime);  //Nykyinen sijainti, haluttu sijainti (0) ja liikkumisnopeus.
            
            if(Vector2.Distance(transform.position, patrolPoints[0].position) < .2f)  //Jos vihollisen sijainti ja partiointi pisteiden sijainti on alle 2f...
            {
                transform.localScale = new Vector3(-1, 1, 1);  //Vihollisen spriten kääntö kulkusuuntaan.
                patrolDestination = 1;  //...lähetä hahmo uuteen määränpäähän.
            }
        }

        if (patrolDestination == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[1].position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, patrolPoints[1].position) < .2f)
            {
                transform.localScale = new Vector3(1, 1, 1);
                patrolDestination = 0;
            }
        }
    }
}
