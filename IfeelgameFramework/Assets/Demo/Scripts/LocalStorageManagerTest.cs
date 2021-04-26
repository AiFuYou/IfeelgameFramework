using System;
using System.Collections.Generic;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class LocalStorageManagerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitTest()
    {
        var btnBase = transform.Find("btnBase").gameObject;
        var btnTextArr = new List<string>
        {
            "SetValue",
            "GetValue",
            "Save",
            "SaveAsync",
            "ClearDefaultData",
            "ClearTestData",
            "ClearAllData"
        };
        
        var offsetY = 150;
        var posY = btnTextArr.Count * (offsetY / 2f) - offsetY / 2f;
        for (var i = 0; i < btnTextArr.Count; i++)
        {
            var btnText = btnTextArr[i];
            var btn = Instantiate(btnBase, transform, false);
            btn.name = btnText;
            btn.transform.GetComponentInChildren<Text>().text = btnText;
            btn.transform.localPosition = new Vector2(0, posY);
            posY -= offsetY;
            
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                switch (btnText)
                {
                    case "SetValue":
                        SetValue();                        
                        break;
                    case "GetValue":
                        GetValue();
                        break;
                    case "Save":
                        LocalStorageManager.Instance.Save(true);
                        break;
                    case "SaveAsync":
                        LocalStorageManager.Instance.SaveAsync(true);
                        break;
                    case "ClearTestData":
                        LocalStorageManager.Instance.ClearData("test", true);
                        break;
                    case "ClearDefaultData":
                        LocalStorageManager.Instance.ClearData();
                        break;
                    case "ClearAllData":
                        LocalStorageManager.Instance.ClearAllData();
                        break;
                }
            });
        }
        
        btnBase.gameObject.SetActive(false);
    }

    private void SetValue()
    {
        var lsT = LocalStorageManager.Instance.GetLocalStorage("test");
        
        lsT.SetValue("short", (short)1);
        lsT.SetValue("int", 1);
        lsT.SetValue("float", 1.1f);
        lsT.SetValue("long", (long)1);
        lsT.SetValue("bool", false, true);
        lsT.SetValue("string", "string1111", false, true);
        lsT.SetValue("double", 11.1, true, true);

        var myClass1 = new MyClass {A = 1};
        var dic1 = new Dictionary<string, int> {{"s", 1}};
        myClass1.E.Add(dic1);
        myClass1.D.Add("a", false);
        
        lsT.SetValue("MyClass", myClass1);

        var lsD = LocalStorageManager.Instance;
        lsD.SetValue("short", (short)2);
        lsD.SetValue("int", 2);
        lsD.SetValue("float", 2.2f);
        lsD.SetValue("long", (long)2);
        lsD.SetValue("bool", false, true);
        lsD.SetValue("string", "string2222", false, true);
        lsD.SetValue("double", 22.2, true, true);
        
        var myClass2 = new MyClass {A = 2};
        var dic2 = new Dictionary<string, int> {{"s", 2}};
        myClass2.E.Add(dic2);
        myClass2.D.Add("a", true);
        
        lsD.SetValue("MyClass", myClass2);
    }

    private void GetValue()
    {
        DebugEx.Log("lsT");
        var lsT = LocalStorageManager.Instance.GetLocalStorage("test");
        var logStr = "";
        logStr += "short: " + lsT.GetValue<short>("short") + "\n";
        logStr += "int: " + lsT.GetValue<int>("int") + "\n";
        logStr += "Int16: " + lsT.GetValue<Int16>("int") + "\n";
        logStr += "Int32: " + lsT.GetValue<Int32>("int") + "\n";
        logStr += "Int64: " + lsT.GetValue<Int64>("int") + "\n";
        logStr += "float: " + lsT.GetValue<float>("float") + "\n";
        logStr += "Single: " + lsT.GetValue<Single>("float") + "\n";
        logStr += "long: " + lsT.GetValue<long>("long") + "\n";
        logStr += "bool: " + lsT.GetValue<bool>("bool") + "\n";
        logStr += "Boolean: " + lsT.GetValue<Boolean>("bool") + "\n";
        logStr += "string: " + lsT.GetValue<string>("string") + "\n";
        logStr += "String: " + lsT.GetValue<String>("string") + "\n";
        logStr += "double: " + lsT.GetValue<double>("double") + "\n";
        logStr += "Double: " + lsT.GetValue<Double>("double") + "\n";
        logStr += "MyClass: " + JsonConvert.SerializeObject(lsT.GetValue<MyClass>("MyClass")) + "\n";
        
        DebugEx.Log(logStr);

        DebugEx.Log("lsD");
        var lsD = LocalStorageManager.Instance;
        logStr = "";
        logStr += "short: " + lsD.GetValue<short>("short") + "\n";
        logStr += "int: " + lsD.GetValue<int>("int") + "\n";
        logStr += "Int16: " + lsD.GetValue<Int16>("int") + "\n";
        logStr += "Int32: " + lsD.GetValue<Int32>("int") + "\n";
        logStr += "Int64: " + lsD.GetValue<Int64>("int") + "\n";
        logStr += "float: " + lsD.GetValue<float>("float") + "\n";
        logStr += "Single: " + lsD.GetValue<Single>("float") + "\n";
        logStr += "long: " + lsD.GetValue<long>("long") + "\n";
        logStr += "bool: " + lsD.GetValue<bool>("bool") + "\n";
        logStr += "Boolean: " + lsD.GetValue<Boolean>("bool") + "\n";
        logStr += "string: " + lsD.GetValue<string>("string") + "\n";
        logStr += "String: " + lsD.GetValue<String>("string") + "\n";
        logStr += "double: " + lsD.GetValue<double>("double") + "\n";
        logStr += "Double: " + lsD.GetValue<Double>("double") + "\n";
        logStr += "MyClass: " + JsonConvert.SerializeObject(lsD.GetValue<MyClass>("MyClass")) + "\n";
        
        DebugEx.Log(logStr);
    }

    private class MyClass
    {
        public int A = 1;
        public List<Dictionary<string, int>> E = new List<Dictionary<string, int>>();
        public Dictionary<string, object> D = new Dictionary<string, object>();
    }
}
