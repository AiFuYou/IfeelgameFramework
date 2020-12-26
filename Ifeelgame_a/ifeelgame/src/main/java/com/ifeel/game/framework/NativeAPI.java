package com.ifeel.game.framework;

import android.content.Context;
import android.provider.Settings;
import android.util.Log;

import java.util.Locale;

public class NativeAPI {
    private static NativeAPI _instance;
    private Context _context;

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

    public void Init(Context context){
        _context = context;
    }

    public String GetRegionName(){
        return Locale.getDefault().getCountry().trim().toUpperCase();
    }

    public String GetAndroidId(){
        if (_context == null){
            Log.v("NativeAPI GetAndroidId", "NativeAPI not init，can not get ANDROID_ID");
            return "";
        }

        String androidId = null;
        try {
            androidId = Settings.System.getString(_context.getContentResolver(), Settings.System.ANDROID_ID);
        } catch (Exception e){
            Log.v("NativeAPI GetAndroidId", "Something went wrong when GetAndroidId");
            e.printStackTrace();
        } finally {
            if (androidId == null){
                androidId = "";
            }
        }

        return androidId;
    }
}
