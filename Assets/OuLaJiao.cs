using UnityEngine;
using System.Collections;

// 2013年9月10日14:40:59，郭志程
// 通信数据转换测试

public class OuLaJiao : MonoBehaviour {

    public float waiBu = -5f;
    public float yRotation = 0.0F;
    float temp = 0f;

    void Update() {
        //yRotation += Input.GetAxis("Horizontal");
        temp = Mathf.Clamp(waiBu, -60, 60);
        if (temp > 0) {
            yRotation += Mathf.Clamp01(waiBu);
        } else {
            temp = Mathf.Clamp01(-waiBu) * -1;
            yRotation += temp;
        }
        transform.eulerAngles = new Vector3(0, yRotation, 0);   // 实际的时候是，将yRotation当做输入值赋给车控制脚本里的变量，代替Input.GetAxis("Horizontal");
        show();
        resetRotation();
    }

    public void show() {        
        //print("欧拉角Y轴="+transform.eulerAngles.y);
        print("Y轴 = " + Mathf.Clamp(yRotation, -5, 5));
    }

    void resetRotation() {
        yRotation = 0.0F;
    }

}
