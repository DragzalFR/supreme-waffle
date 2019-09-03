using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    protected Animator anim;
    protected float speed;
    //private Vector3 target;
    protected Vector3[] targetPath;
    protected int targetPathStep;

    //public int rangeMove = 6;
    public int movement; // rangeMove * 2
    protected Dictionary<Vector2Int, int> reacheableTile = new Dictionary<Vector2Int, int>();

    // Use this for initialization
    protected void Start () {
        //this.transform.position = new Vector3(
        //        3.5f, 0f, 3.5f
        //        );

        anim = GetComponent<Animator>();
        speed = 0;
    }
	
	// Update is called once per frame
	protected void Update () {

        if (targetPath != null && rotateToTarget())
        {
            float d = Vector3.Distance(transform.position, targetPath[targetPathStep]);
            if (d < 0.2f)
            {
                targetPathStep++;

                if (targetPathStep == targetPath.Length)
                    stopMove();
            }
        }
    }

    // Modifie les tuiles de deplacements si le personnage est a l'arret.
    private void OnMouseDown()
    {
        // Modifie les tuiles de deplacements si a l'arret.
        if(speed < 0.01f)
        {
            // Si pas de tuile atteignable il faut les activer.
            // Si la liste des tuiles n'est pas vide, alors on les desactives.
            activeTile(0 == reacheableTile.Count);
        }
    }

    // Arrete le mouvement du personnage.
    // Change l'etat d'activation des cases de la position initiale.
    protected void stopMove()
    {
        if (speed > 0.01f)
        {
            speed = 0f;
            anim.SetFloat("walkingSpeed", speed);
            unactiveTile();
            targetPath = null;
            reacheableTile.Clear();
        }
    }

    // Create the list of tile who can be reached.
    protected void defineReacheableTile()
    {
        reacheableTile.Clear();

        int width = GridManager.width;
        int lenght = GridManager.lenght;

        var posx = Mathf.FloorToInt(transform.position.x);
        var posz = Mathf.FloorToInt(transform.position.z);
        var pos = new Vector2Int(posx, posz);

        reacheableTile.Add(pos, 0);

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);

        while (queue.Count != 0)
        {
            var front = queue.Dequeue();
            var value = reacheableTile[front];

            for (int i = 0; i < 9; i++)
            {
                // x - 1 + (i % 3)
                // y - 1 + (i / 3)
                var vec2 = new Vector2Int(front.x - 1 + (i % 3), front.y + 1 - (i / 3));
                // Si la tuile est hors terrain on passe a l'iteration suivante.
                if (vec2.x < 0 || vec2.x >= width || vec2.y < 0 || vec2.y >= lenght) { continue; }

                // Si pair (i % 2 = 0) +3
                // Si impair (i % 2 = 1) +2
                // Si la tuile est hors porte de mouvement on passe a l'iteration suivante.
                int nextValue = value + 3 - (i % 2);
                if (nextValue > movement) { continue; }

                // On verifie que la case de destination soit vide.
                float sphereRadius = 0.3f;
                Vector3 sphereCenter = new Vector3(vec2.x + 0.5f, 0.5f, vec2.y + 0.5f); // +0.5 pour aller au centre
                if (Physics.CheckSphere(sphereCenter, sphereRadius)) { continue; }

                // Si i est paire, on est sur un mouvement diagonale
                if ((i % 2) == 0)
                {
                    // Un mouvement diagonal est possible si les deux cases adjacantes sont accecible.
                    sphereCenter = new Vector3(vec2.x + 0.5f, 0.5f, front.y + 0.5f);
                    if (Physics.CheckSphere(sphereCenter, sphereRadius)) { continue; }
                    sphereCenter = new Vector3(front.x + 0.5f, 0.5f, vec2.y + 0.5f);
                    if (Physics.CheckSphere(sphereCenter, sphereRadius)) { continue; }
                }

                if (!reacheableTile.ContainsKey(vec2))
                {
                    reacheableTile.Add(vec2, nextValue);
                    queue.Enqueue(vec2);
                }
                else if (reacheableTile[vec2] > nextValue)
                {
                    reacheableTile[vec2] = nextValue;
                    queue.Enqueue(vec2);
                }
            }
        }
    }

    // Turn all reacheable tile active.
    // activeTile(false) = unactiveTile().
    protected virtual void activeTile(bool activated = true)
    {
        if (!activated)
        {
            unactiveTile();
            return;
        }

        defineReacheableTile();

        foreach (var key in reacheableTile.Keys)
            GameObject.Find("Tile [" + key.x + ", " + key.y + "]").GetComponent<TileManager>().setActive(name);
    }

    // Turn all tile unactive.
    protected void unactiveTile()
    {
        foreach (var key in reacheableTile.Keys)
            GameObject.Find("Tile [" + key.x + ", " + key.y + "]").GetComponent<TileManager>().setUnactive();
    }

    // Tourne le personnage en vers sa cible (target).
    protected bool rotateToTarget()
    {
        //transform.rotation;
        Vector3 targetDir = targetPath[targetPathStep] - transform.position;

        // The step size is equal to speed times frame time.
        //float step = speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 1, 0.0f);
        bool finalRotation = (newDir == Vector3.RotateTowards(transform.forward, targetDir, 2*Mathf.PI, 0.0f));
        Debug.DrawRay(transform.position, newDir, Color.red);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);

        return finalRotation;
    }

    // Lance le mouvement pour aller vers destination.
    public void goToDestination(Vector3 destination)
    {
        goToDestination(new Vector2Int(Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.z)));
    }
    public void goToDestination(Vector2Int destination)
    {
        if (createPath(destination))
        {
            targetPathStep = 0;
            speed = 1;
            anim.SetFloat("walkingSpeed", speed);
        }
    }

    // Cree le chemin targetPath qui nous amene a destination
    protected bool createPath(Vector3 destination)
    {
        return createPath(new Vector2Int(Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.z)));
    }
    protected bool createPath(Vector2Int destination)
    {
        // Si notre destination n'est pas atteignable, on ne cree pas de chemin.
        if (!reacheableTile.ContainsKey(destination)) { return false; }

        // Si on est deja a notre destination, on ne cree pas de chemin.
        Vector2Int transformPosition2 = new Vector2Int(
            Mathf.FloorToInt(transform.position.x), 
            Mathf.FloorToInt(transform.position.z));
        if (transformPosition2 == destination) { return false; }

        Queue<Vector2Int[]> paths = new Queue<Vector2Int[]>();
        paths.Enqueue(new Vector2Int[1] { destination });

        int loopSecurity = 0;
        while (paths.Count != 0 && ++loopSecurity < 2 * movement)
        {
            // On recupere le premier element.
            Vector2Int[] path = paths.Dequeue();
            Vector2Int currentStep = path[0];
            int value = reacheableTile[currentStep];

            // On cherche a ajouter les destination possible.
            for (var i = 0; i < 9; i++)
            {
                var vec2 = new Vector2Int(currentStep.x - 1 + (i % 3), currentStep.y + 1 - (i / 3));

                // On verifie que la tuile n'est pas deja dans le chemin
                bool backward = false;
                for (var k = 0; k < path.Length; k++)
                {
                    backward = (vec2 == path[k]);
                    if (backward)
                        continue;
                }

                // Si i est paire, on est sur un mouvement diagonale
                // On verifie que la diagonale est possible (les deux projection le sont).
                if ((i % 2) == 0)
                {
                    var projectionX = new Vector2Int(vec2.x, currentStep.y);
                    var projectionY = new Vector2Int(currentStep.x, vec2.y);
                    if (!reacheableTile.ContainsKey(projectionX) 
                        || !reacheableTile.ContainsKey(projectionY))
                        continue;
                }

                int nextValue = value - (3 - (i % 2));
                if (reacheableTile.ContainsKey(vec2)
                    && reacheableTile[vec2] == nextValue)
                {
                    Vector2Int[] newPath = new Vector2Int[path.Length + 1];
                    newPath[0] = vec2;
                    path.CopyTo(newPath, 1);
                    newPath[0] = vec2;
                    if (vec2 == transformPosition2)
                    {
                        setTargetPath(newPath);
                        return true;
                    }
                    paths.Enqueue(newPath);
                }
            }
        }

        return false;
    }

    // Remplit le tableau de Vector3:targetPath a partir d'un tableau Vector2Int:path
    protected void setTargetPath(Vector2Int[] path)
    {
        targetPath = new Vector3[path.Length];
        for (var i = 0; i < path.Length; i++)
            targetPath[i] = new Vector3(path[i].x + 0.5f, transform.position.y, path[i].y + 0.5f);

        targetPathStep = 0;
        rotateToTarget();
    }
}
