using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    public GameTimer Timer = null ;
    public GameObject RedTarget = null;
    public GameObject GreenTarget = null;
    public GameObject BlueTarget = null;
    public GameObject CyanTarget = null;

    private bool _foundRed = false;
    private bool _foundGreen = false;
    private bool _foundBlue = false;
    private bool _foundCyan = false;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ((Timer == null) ||
            (RedTarget == null) ||
            (GreenTarget == null) ||
            (BlueTarget == null) ||
            (CyanTarget == null))
            return;

    }

    void ManageHit (GameObject target)
    {
        target.SetActive(false);
    }

    public void TargetHit (string name)
    {
        Debug.Log("TargetHit name \"" + name + "\"");
        if (name == "Red")
        {
            Debug.Log("Red Name \"" + RedTarget.name + "\"");
            ManageHit(RedTarget);
            _foundRed = true;
        }
        else if (name == "Green")
        {
            ManageHit(GreenTarget);
            _foundGreen = true;
        }
        else if (name == "Blue")
        {
            ManageHit(BlueTarget);
            _foundBlue = true;
        }
        else if (name == "Cyan")
        {
            ManageHit(CyanTarget);
            _foundCyan = true;
        }
        else if (name == "Wall")
        {
            Timer.AddPenalty(1.0f);
        }

        if (_foundRed && _foundGreen && _foundBlue && _foundCyan)
            Timer.PauseTimer();
    }
}
