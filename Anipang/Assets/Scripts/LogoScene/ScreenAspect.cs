using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAspect : MonoBehaviour
{

    float defaultScreenWidth = 1440f;
    float defaultScreenHeight = 2560f;

    public void Awake()
    {
        SetScreenSizeAspect();
    }
    public void SetScreenSizeAspect()
    {
        float deviceWidth = Screen.width;
        float deviceHeight = Screen.height;

        Screen.SetResolution((int)defaultScreenWidth,(int)((deviceHeight / deviceWidth) * defaultScreenWidth), true);

        if(defaultScreenWidth / defaultScreenHeight < deviceWidth / deviceHeight)
        {
            float newWidth = ((defaultScreenWidth / defaultScreenHeight) / (deviceWidth / deviceHeight));
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = ((deviceWidth / deviceHeight) / (defaultScreenWidth / defaultScreenHeight));
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}
