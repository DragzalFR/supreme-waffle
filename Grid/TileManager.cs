using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    private bool targetable;
    private string characterName = null;

	// Use this for initialization
	void Start () {
        targetable = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Si la tuile est active, la definie comme cible du personnage ("Female").
    // TODO : remplacer "Female" par le personnage selectionne avant.
    void OnMouseDown()
    {
        if(targetable 
            && !string.IsNullOrEmpty(characterName))
        {
            //GameObject.Find("Female").transform.position = new Vector3(
            //    this.transform.position.x,
            //    0,
            //    this.transform.position.z);

            var go = GameObject.Find(characterName);
            Character other = go.GetComponent(typeof(Character)) as Character;
            Vector3 target = transform.position;
            target.y += 1;
            other.goToDestination(target);
        }
        
    }

    // Mise en surbrillance de la tuile pointe.
    private void OnMouseEnter()
    {
        if (string.IsNullOrEmpty(characterName))
            return;

        var color = GetComponent<Renderer>().material.GetColor("_Color");
        color.a *= 3f;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
    // Retire la surbrillance de la tuile pointe.
    private void OnMouseExit()
    {
        if (string.IsNullOrEmpty(characterName))
            return;

        var color = GetComponent<Renderer>().material.GetColor("_Color");
        color.a /= 3f;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    // Rend la tuile active (targetable).
    // setActive(false) = setUnactive()
    public void setActive(string name = null, bool activate = true)
    {
        if(!activate)
        {
            setUnactive();
            return;
        }

        targetable = true;
        characterName = name;

        var color = GetComponent<Renderer>().material.GetColor("_Color");
        color.a = 0.2f;
        if (string.IsNullOrEmpty(characterName))
        {
            color.a *= 0.75f;
            color.r = color.b;
        }
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    // Rend la tuile inactive (!targetable).
    public void setUnactive()
    {
        targetable = false;
        characterName = null;

        var color = GetComponent<Renderer>().material.GetColor("_Color");
        color.a = 0f;
        // Inutile si le caractere selectionne n'est pas un enemy.
        color.r = 0.2734623f;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    // Outdated fonction
    public void switchActive()
    {
        targetable = !targetable;

        // Si la tuile est occupe, on la rend inactive.
        float sphereRadius = 0.30f;
        Vector3 sphereCenter = transform.position;
        sphereCenter.y += 0.5f;
        if (Physics.CheckSphere(sphereCenter, sphereRadius))
        {
            targetable = false;
        }

        // Defini le rendu en fonction de targetable.
        if(targetable)
        {
            var color = GetComponent<Renderer>().material.GetColor("_Color");
            color.a = 0.2f;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        else
        {
            var color = GetComponent<Renderer>().material.GetColor("_Color");
            color.a = 0f;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }
}
