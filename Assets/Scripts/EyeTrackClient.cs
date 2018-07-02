using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnRecvFixationPoint(int x, int y);

public class EyeTrackClient : BaseClient {

    const int REQUEST_EYE_TRACKING = 2;

    private OnRecvFixationPoint onRecvFixationPoint;

    public EyeTrackClient SetOnRecvFixationPointListener(OnRecvFixationPoint listener){
        onRecvFixationPoint = listener;
        return this;
    }

    public override SessionResultCodes OnSession()
    {
        if (SendData(REQUEST_EYE_TRACKING) == -1) 
            return SessionResultCodes.Fail;

        string data = RecvData();
        if (data == null) return SessionResultCodes.Fail;
        while (IsConnected())
        {
            // 调整绘制内容
            string[] point = data.Split(' ');
            onRecvFixationPoint(int.Parse(point[0]), int.Parse(point[1]));

            data = RecvData();
            if (data == null) return SessionResultCodes.Fail;
        }

        return SessionResultCodes.Success;
    }

    public void Close(){
        close();
    }
}
