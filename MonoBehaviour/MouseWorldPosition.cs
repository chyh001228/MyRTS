using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance {  get; private set; } //创造静态实例，以便DOTS脚本访问

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPosition() { 
        Ray mouseCameraRay =  Camera.main.ScreenPointToRay(Input.mousePosition);

        /*if(Physics.Raycast(mouseCameraRay, out RaycastHit hitInfo))
        {
            return hitInfo.point; // 使用物理系统
        }*/

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if(plane.Raycast(mouseCameraRay,out float distance))
        {
            return mouseCameraRay.GetPoint(distance); //不使用物理系统
        }
        else { return Vector3.zero; }

        
    }
}
