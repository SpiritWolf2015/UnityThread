using UnityEngine;
using System.Collections;

public class TestThread : MonoBehaviour {

    int num;	
	void Start () {
        num = 0;
	}
	void Update () {
	    if(Input.GetKey(KeyCode.Space)){
            Testdo();
        }
	}

    void Testdo() {

        // 在新线程上运行的代码
        Loom.RunAsync(() => {
            for (int i = 0; i < 50; i++) {
                num++;
                Debug.LogWarning("AKB输出第" + i + "行" + ", num = " + num);
            }            
        });
        //在主线程上运行一些代码        
        Loom.QueueOnMainThread(() => {
            for ( int i = 0; i < 50; i++ ) {
                num -= 50;
                Debug.Log("SKE输出第" + i + "行" + ", num = " + num);
            }
            this.transform.position = new Vector3(0, num,0);
        },1);
    }

}
