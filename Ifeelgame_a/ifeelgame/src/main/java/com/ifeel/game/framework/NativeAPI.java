package com.ifeel.game.framework;

import java.util.Locale;

public class NativeAPI {
    private static NativeAPI _instance;

    public static NativeAPI Instance(){
        if (_instance == null) {
            synchronized (NativeAPI.class) {
                if (_instance == null) {
                    _instance = new NativeAPI();
                }
            }
        }
        return _instance;
    }

    private NativeAPI(){ }

    public String GetRegionName(){
        return Locale.getDefault().getCountry().trim().toUpperCase();
    }
}
