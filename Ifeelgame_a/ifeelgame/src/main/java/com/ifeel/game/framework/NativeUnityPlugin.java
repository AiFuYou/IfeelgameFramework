package com.ifeel.game.framework;

public class NativeUnityPlugin {
    public static String GetRegionName(){
        return NativeAPI.Instance().GetRegionName();
    }
}
