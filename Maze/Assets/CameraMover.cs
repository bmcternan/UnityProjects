using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
			this.transform.Rotate (new Vector3(0.0f, -30.0f, 0.0f)) ;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			this.transform.Rotate (new Vector3(0.0f, 30.0f, 0.0f)) ;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			this.transform.position += (this.transform.forward * 0.25f) ;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			this.transform.position -= (this.transform.forward * 0.25f) ;
		}
	}
}
