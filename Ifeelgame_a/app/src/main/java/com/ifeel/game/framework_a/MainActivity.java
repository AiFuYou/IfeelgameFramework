package com.ifeel.game.framework_a;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.util.Log;

import com.ifeel.game.framework.NativeUnityPlugin;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.v("MainActivity", "测试");
        Log.v("MainActivity", NativeUnityPlugin.GetRegionName());
    }
}