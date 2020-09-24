using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Utils;
using UnityEngine;

public class FileRecordQueueTest : MonoBehaviour
{
    private FileRecordQueue _rec;
    
    // Start is called before the first frame update
    async void Start()
    {
        DebugEx.Log(Application.persistentDataPath);
        _rec = new FileRecordQueue(Application.persistentDataPath + "test.dat");

        var count = 0;
        for (var i = 0; i < 100; ++i)
        {
            _rec.WriteRecord(i.ToString());
            if (i % 10 == 0)
            {
                _rec.AddTask(_ =>
                {
                    ++count;
                    DebugEx.Log("插入10条数据", count);
                    DebugEx.Log("当前数据条数", _rec.RecordCount());
                });
            }
        }

        await Task.Delay(3000);
        for (var i = 0; i < 100; ++i)
        {
            _rec.WriteRecord((i + 1000).ToString());
            if (i % 10 == 0)
            {
                _rec.AddTask(_ =>
                {
                    ++count;
                    DebugEx.Log("插入10条数据", count);
                    DebugEx.Log("当前数据条数", _rec.RecordCount());
                });
            }
        }
    }
}
