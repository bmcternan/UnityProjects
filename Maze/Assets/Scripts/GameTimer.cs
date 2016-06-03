using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public GameObject Controller = null;
    public GameObject Timer = null;
    public GameObject Penalty = null;
    public GameObject Total = null;

    private float _totalTime = 0.0f;
    private float _runTime = 0.0f;
    private float _accumulatedTime = 0.0f;
    private float _lastStartTime = 0.0f;
    private float _penaltyTime = 0.0f;
    private bool _running = false;
    public bool _runTimer = false;

	// Use this for initialization
	void Start ()
    {
	
	}

	string formatTime (float timeIn)
    {
        float mins = (float)((int)(timeIn / 60.0f));
        float secs = timeIn - (mins * 60.0f);
        return string.Format("{0:00}:{1:00.00}", mins, secs);
    }

    public void RunTimer()
    {
        _runTimer = true;
    }

    public void PauseTimer()
    {
        _runTimer = false;
    }

    public void AddPenalty (float penaltyAdd)
    {
        _penaltyTime += penaltyAdd;
    }

    public float GetTotalTime()
    {
        return _totalTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((Timer == null) || (Penalty == null) || (Total == null) || (Controller == null))
            return;

        if (_runTimer)
        {
            if (!_running)
            {
                _lastStartTime = Time.realtimeSinceStartup;
                _running = true;
            }
            _runTime = (Time.realtimeSinceStartup - _lastStartTime) + _accumulatedTime;
        }
        else
        {
            if (_running)
            {
                {
                    _accumulatedTime += _runTime;
                    _running = false;
                }
            }
        }

        _totalTime = _runTime + _penaltyTime;
        TextMesh mesh = Timer.GetComponent<TextMesh>();
        mesh.text = formatTime(_runTime);
        mesh = Penalty.GetComponent<TextMesh>();
        mesh.text = formatTime(_penaltyTime) + " Penalty" ;
        mesh = Total.GetComponent<TextMesh>();
        mesh.text = formatTime(_totalTime) + " Total" ;

        this.transform.position = Controller.transform.position;
        this.transform.eulerAngles = Controller.transform.eulerAngles ;
    }
}
