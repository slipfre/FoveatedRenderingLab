using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnRecvPointID(int id);

public class CalibrationClient : BaseClient
{
    const int REQUEST_CALIBRATION = 1;
    const int RESPONSE_CALIBRATION_COMPLETED = -1;
    private OnRecvPointID onRecvPointID;

    public CalibrationClient SetOnRevPointIDListner(OnRecvPointID onRecvPointID){
        this.onRecvPointID = onRecvPointID;
        return this;
    }

    public override SessionResultCodes OnSession()
    {
        if (SendData(REQUEST_CALIBRATION) == -1) return SessionResultCodes.Fail;

        string data = RecvData();
        if (data == null) return SessionResultCodes.Fail;
        while(int.Parse(data) != RESPONSE_CALIBRATION_COMPLETED){
            // 画点
            onRecvPointID(int.Parse(data));

            data = RecvData();
            if (data == null) return SessionResultCodes.Fail;
        }

        return SessionResultCodes.Success;
    }
}
