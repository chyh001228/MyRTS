using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance {  get; private set; } //���쾲̬ʵ�����Ա�DOTS�ű�����

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPosition() { 
        Ray mouseCameraRay =  Camera.main.ScreenPointToRay(Input.mousePosition);

        /*if(Physics.Raycast(mouseCameraRay, out RaycastHit hitInfo))
        {
            return hitInfo.point; // ʹ������ϵͳ
        }*/

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if(plane.Raycast(mouseCameraRay,out float distance))
        {
            return mouseCameraRay.GetPoint(distance); //��ʹ������ϵͳ
        }
        else { return Vector3.zero; }

        
    }
}
