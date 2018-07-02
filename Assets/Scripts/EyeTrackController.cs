using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTrackController : MonoBehaviour {

    EyeTrackClient client = new EyeTrackClient();

    public Texture circle;
    private Vector2 circleLocation = new Vector2(0.0f, 0.0f);
    private bool ifStarted;

	public void Start()
	{
        LaunchSession();
	}

	public void LaunchSession(){
        client.SetOnRecvFixationPointListener(OnRecvFixationPoint)
              .SetSessionSuccessListener(OnSessionSuccess)
              .SetSessionFailedListener(OnSessionFailed)
              .SetIP(Values.ip)
              .SetPort(BaseClient.debugPort)
              .InitSocket()
              .LaunchSession();
    }

    void OnGUI(){
        //if (ifStarted)
        //{
        //    Rect rect = new Rect(
        //        Values.eyeX - circle.width / 2,
        //        Screen.height - Values.eyeY - circle.height / 2,
        //        circle.width/8,
        //        circle.height/8
        //    );
        //    GUI.DrawTexture(rect, circle);
        //}
    }

    void OnRecvFixationPoint(int x, int y){
        ifStarted = true;
        Values.eyeX = x;
        Values.eyeY = Screen.height - y;
    }

    void OnSessionSuccess()
    {
        if (ifStarted)
            ifStarted = false;
    }

    void OnSessionFailed()
    {
        if (ifStarted)
            ifStarted = false;
    }
}
