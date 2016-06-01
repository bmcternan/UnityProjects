using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour
{
    public string Name;
    public GameController GameController = null;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    //void OnCollisionEnter(Collision collision)
    //ContactPoint contact = collision.contacts[0];
    //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
    //Vector3 pos = contact.point;
    //Instantiate(explosionPrefab, pos, rot);
    //Destroy(gameObject);
    void OnCollisionEnter(Collision collision)
    //void OnTriggerEnter()
    {
        Debug.Log("OnCollisionEnter \"" + collision.collider.name + "\" against \"" + this.name +"\"");

        if ((GameController == null) || (Name.Length <= 0))
            return;

        GameController.TargetHit(Name);

    }
}
