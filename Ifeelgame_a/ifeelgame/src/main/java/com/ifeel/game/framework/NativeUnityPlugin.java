package com.ifeel.game.framework;

import android.content.Context;

public class NativeUnityPlugin {
    public static void Init(Context cnt){
        NativeAPI.Instance().Init(cnt);
    }

    public static String GetRegionName(){
        return NativeAPI.Instance().GetRegionName();
    }

    public static String GetAndroidId(){
        return NativeAPI.Instance().GetAndroidId();
    }
}
