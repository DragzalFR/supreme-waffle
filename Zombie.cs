using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Character
{

    private void rotateToRandom()
    {
        int rand = Random.Range(1, 4);
        Debug.Log("Random : " + rand);

        //transform.rotation;
        Vector3 newDir;
        if (rand == 1)
            newDir = Vector3.left;
        else if (rand == 2)
            newDir = Vector3.right;
        else
            newDir = Vector3.forward;
        Debug.DrawRay(transform.position, newDir, Color.red);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(transform.rotation * newDir);
    }

    public void ChooseDestination()
    {
        anim.SetTrigger("attackTrigger");
    }

    // Turn all reacheable tile active.
    // activeTile(false) = unactiveTile().
    protected override void activeTile(bool activated = true)
    {
        if (!activated)
        {
            unactiveTile();
            return;
        }

        defineReacheableTile();

        foreach (var key in reacheableTile.Keys)
            GameObject.Find("Tile [" + key.x + ", " + key.y + "]").GetComponent<TileManager>().setActive();
    }
}
