using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionElement
{
    string _name;
    int _collisionCount = 0;
    
    public CollisionElement (string newName)
    {
        _name = newName;
        _collisionCount = 1;
    }
    public string Name() { return _name; }
    public int CollisionCount() {  return _collisionCount; }
    public int IncCollisonCount() { _collisionCount++; return _collisionCount; }
    public int DecCollisonCount() { _collisionCount--; return _collisionCount; }

}; 

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


    private List<CollisionElement> _collisionList = new List<CollisionElement>();

    // Use this for initialization
    void Start ()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();

    }
	
	// Update is called once per frame
	void Update ()
    {
        //if ((Timer == null) ||
        //    (RedTarget == null) ||
        //    (GreenTarget == null) ||
        //    (BlueTarget == null) ||
        //    (CyanTarget == null))
        //    return;

        for (int i = (_collisionList.Count - 1); i >= 0; i--)
        {
            Debug.Log("Col " + i + ": name \"" + _collisionList[i].Name() + "\" count " + _collisionList[i].CollisionCount());
            if (_collisionList[i].DecCollisonCount() < 0)
            {
                Debug.Log("REMOVING Col " + i);
                _collisionList.RemoveAt(i);
            }
        }
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
        else if (name.Substring(0,4) == "Wall")
        {
            var element = _collisionList.Find(e => e.Name() == name);
            if (element == null)
            {
                Debug.Log("Adding \"" + name + "\"");
                _collisionList.Add(new CollisionElement(name));
                Timer.AddPenalty(1.0f);
            }
            else
            {
                Debug.Log("INCing \"" + name + "\"");
                element.IncCollisonCount();
            }
        }

        if (_foundRed && _foundGreen && _foundBlue && _foundCyan)
            Timer.PauseTimer();
    }
}
