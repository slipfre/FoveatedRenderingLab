using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationController : MonoBehaviour {
    public Texture2D circle;
    public float timeOfMoving;
    public float timeOfScaling;
    private bool change = false;
    public Text text;
    public Text inputField;
    private Vector2[] pointLocations = {
        new Vector2(0.5f,0.5f), new Vector2(0f, 0f), new Vector2(0.5f, 1.0f),
        new Vector2(1.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(0.5f, 0.0f),
        new Vector2(0f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(0f, 1.0f)
    };
    public GameObject button;
    public GameObject inF;
    private int pointLocationID = -1;

    private Vector2 ScreenSize;
    private Vector2 CircleInitialSize;
    private Vector2 CircleSize;

    private CalibrationClient client = new CalibrationClient();

    void Start(){
        InvokeRepeating("MoveAndScale", 0.0f, interval);
        ScreenSize.x = Screen.width;
        ScreenSize.y = Screen.height;
        CircleInitialSize.x = circle.width*1.5f;
        CircleInitialSize.y = circle.height*1.5f;
    }

    public void LaunchSession(){
        if(!change){
            Values.ip = inputField.text;
            client.SetOnRevPointIDListner(OnRecvPointID)
              .SetSessionSuccessListener(OnSessionSuccess)
              .SetSessionFailedListener(OnSessionFailed)
              .SetIP(Values.ip)
              .SetPort(BaseClient.debugPort)
              .InitSocket()
              .LaunchSession();

            button.SetActive(false);
            inF.SetActive(false);
        }
        else{
            toScene();
        }

        // debug
        //pointLocationID++;
        //if (pointLocationID > 0)
        //{
        //    Vector2 now = calculatePointLocation(pointLocationID);
        //    Vector2 previous = calculatePointLocation(pointLocationID - 1);
        //    deltaWidth = (now.x - previous.x) / (timeOfMoving / interval);
        //    deltaHeight = (now.y - previous.y) / (timeOfMoving / interval);
        //    deltaSize = -CircleInitialSize.x / (timeOfScaling / interval);
        //    CircleSize = CircleInitialSize;
        //    time = 0.0f;
        //}
        //else
        //{
        //    CircleSize = CircleInitialSize;
        //    deltaSize = -CircleInitialSize.x / (timeOfScaling / interval);
        //    point = calculatePointLocation(pointLocationID);
        //    time = timeOfMoving;
        //}
    }

    void OnGUI()
    {
        if(pointLocationID != -1){
            Rect rect = new Rect(
                point.x - CircleSize.x / 2,
                point.y - CircleSize.y / 2,
                CircleSize.x,
                CircleSize.y
            );
            GUI.DrawTexture(rect, circle);
        }

    }

	private void Update()
	{
        if(change){
            button.SetActive(true);
            text.text = "开始眼动跟踪";
        }
	}

	Vector2 calculatePointLocation(int id){
        float width = ScreenSize.x - CircleInitialSize.x;
        float height = ScreenSize.y - CircleInitialSize.y;
        Vector2 result = new Vector2(
            pointLocations[id].x * width + CircleInitialSize.x/2, pointLocations[id].y * height + CircleInitialSize.y/2
        );
        return result;
    }

    private float deltaWidth;
    private float deltaHeight;
    private float deltaSize;
    private float interval = 0.008f;
    private float time = 0.0f;
    private Vector2 point;
    void MoveAndScale(){
        if(time >= timeOfMoving){
            if(CircleSize.x > CircleInitialSize.x/2){
                CircleSize.x = CircleSize.x + deltaSize;
                CircleSize.y = CircleSize.y + deltaSize;
            }
        }else{
            point.x += deltaWidth;
            point.y += deltaHeight;
            time += interval;
        }
    }

    void OnRecvPointID(int id){
        pointLocationID = id;

        if (pointLocationID > 0)
        {
            Vector2 now = calculatePointLocation(pointLocationID);
            Vector2 previous = calculatePointLocation(pointLocationID - 1);
            deltaWidth = (now.x - previous.x) / (timeOfMoving / interval);
            deltaHeight = (now.y - previous.y) / (timeOfMoving / interval);
            deltaSize = -CircleInitialSize.x / (timeOfScaling / interval);
            CircleSize = CircleInitialSize;
            time = 0.0f;
        }
        else
        {
            CircleSize = CircleInitialSize;
            deltaSize = -CircleInitialSize.x / (timeOfScaling / interval);
            point = calculatePointLocation(pointLocationID);
            time = timeOfMoving;
        }
    }

    void OnSessionSuccess(){
        change = true;
    }

    void OnSessionFailed(){
    }

    private AsyncOperation mAsyncOperation;
    public void toScene(){
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        // u3d 5.3之后使用using UnityEngine.SceneManagement;加载场景
        mAsyncOperation = SceneManager.LoadSceneAsync("Classroom/Scenes/Classroom");
        // 不允许加载完毕自动切换场景，因为有时候加载太快了就看不到加载进度条UI效果了
        mAsyncOperation.allowSceneActivation = true;
        // mAsyncOperation.progress测试只有0和0.9
        while (!mAsyncOperation.isDone && mAsyncOperation.progress < 1)
        {
            yield return mAsyncOperation;
        }
    }
}
