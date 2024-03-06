using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    [SerializeField] private Players player;

    [SerializeField] public GameObject character;
    [SerializeField] private GameObject cameraCenter;
    public float yOffset = 1f;
    public float sensitivity = 1f;
    [SerializeField] private Camera cam;

    public float scrollSensitivity = 2f;
    public float scrollDampening = 6f;

    public float zoomMin = 1f;
    public float zoomMax = 3f;
    public float zoomDefault = 2f;
    public float zoomDistance;

    public float collisionSensitivity = 1f;

    private RaycastHit camHit;
    private Vector3 camDist;

    void Start()
    {
        camDist = cam.transform.localPosition;
        zoomDistance = zoomDefault;
        camDist.z = zoomDistance;
    }

    // Update is called once per frame
    void Update()
    {
        cameraCenter.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + yOffset, character.transform.position.z);

        if (player.lockedCursor == false)
        {
            return;
        }

        if (cameraCenter.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2 >= 3.9f && cameraCenter.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2 <= 50f)
        {
            var rotation = Quaternion.Euler(cameraCenter.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity,
            cameraCenter.transform.rotation.eulerAngles.z);
            cameraCenter.transform.rotation = rotation;
        }
        else
        {
            var rotation = Quaternion.Euler(cameraCenter.transform.rotation.eulerAngles.x,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity,
            cameraCenter.transform.rotation.eulerAngles.z);
            cameraCenter.transform.rotation = rotation;
        }

        if (cameraCenter.transform.localEulerAngles.x < 3.9f)
        {
            cameraCenter.transform.rotation = Quaternion.Euler(3.9f,
            cameraCenter.transform.rotation.eulerAngles.y,
            cameraCenter.transform.rotation.eulerAngles.z);
        }

        if (cameraCenter.transform.localEulerAngles.x > 50f)
        {
            cameraCenter.transform.rotation = Quaternion.Euler(50f,
            cameraCenter.transform.rotation.eulerAngles.y, cameraCenter.transform.rotation.eulerAngles.z);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            var scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            scrollAmount *= zoomDistance * 0.3f;
            zoomDistance += -scrollAmount;
            zoomDistance = Mathf.Clamp(zoomDistance, zoomMin, zoomMax);
        }

        if (camDist.z != -zoomDistance)
        {
            camDist.z = Mathf.Lerp(camDist.z, -zoomDistance, Time.deltaTime * scrollDampening);
        }

        cam.transform.localPosition = camDist;

        GameObject obj = new GameObject();
        obj.transform.SetParent(cam.transform.parent);
        obj.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, cam.transform.localPosition.z - collisionSensitivity);

        Destroy(obj);

        if (cam.transform.localPosition.z > -1f)
        {
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -1f);
        }
    }
}
