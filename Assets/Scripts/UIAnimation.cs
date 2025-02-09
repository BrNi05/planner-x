using System.Collections; using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    GameObject currentObj;
    bool detectMousePress = false;

    public GameObject atCatArrow;
    public GameObject atImpArrow;
    public GameObject catArrow;
    public GameObject colorScemeArrow;
    public GameObject displayModeArrow;
    public GameObject secQ1Arrow;
    public GameObject secQ2Arrow;
    public GameObject secQ3Arrow;

    Quaternion start = Quaternion.Euler(Vector3.zero);
    Quaternion end = Quaternion.Euler(Vector3.forward * 180f);

    private void Update() { if (detectMousePress && Input.GetMouseButtonDown(0) && currentObj.transform.eulerAngles != new Vector3(0, 0, 0)) { StartCoroutine(Rotate(end, start)); } }

    public void RotateDropdownArrow(string objName)
    {
        detectMousePress = true;

        if (objName == "categoryArrow") { currentObj = atCatArrow; }
        if (objName == "importanceArrow") { currentObj = atImpArrow; }
        if (objName == "catArrow") { currentObj = catArrow; }
        if (objName == "colorSchemeArrow") { currentObj = colorScemeArrow; }
        if (objName == "displayModeArrow") { currentObj = displayModeArrow; }
        if (objName == "secQ1") { currentObj = secQ1Arrow; }
        if (objName == "secQ2") { currentObj = secQ2Arrow; }
        if (objName == "secQ3") { currentObj = secQ3Arrow; }

        if (currentObj.transform.eulerAngles.z > 0) { StartCoroutine(Rotate(end, start)); }
        else { StartCoroutine(Rotate(start, end)); }
    }

    IEnumerator Rotate(Quaternion start, Quaternion end)
    {
        System.Diagnostics.Stopwatch stopper = new System.Diagnostics.Stopwatch();
        stopper.Reset(); stopper.Start();
        while (currentObj.transform.rotation != end)
        {
            Quaternion rot = Quaternion.Inverse(Quaternion.Lerp(start, end, stopper.ElapsedMilliseconds / 100f));
            currentObj.transform.rotation = rot;
            yield return new WaitForEndOfFrame();
        }
    }
}
