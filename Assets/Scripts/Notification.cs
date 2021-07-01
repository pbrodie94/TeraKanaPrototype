using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    float _timeShown;
    public float timeShown
    {
        get
        {
            return _timeShown;
        }
    }

    string message;

    RectTransform rect;
    ContentSizeFitter sizeFitter;
    Text notificationText;
    Vector2 wantedPos;
    float canvasWidth;
    float canvasHeight;
    float smoothSpeed = 15;

    public void Initialize(string msg, Transform canvas)
    {
        //Set the time that notification is shown
        _timeShown = Time.time;
        //Set the message
        message = msg;

        //Create the ui object, add a text component and set the text
        rect = gameObject.GetComponent<RectTransform>();
        notificationText = gameObject.AddComponent<Text>();
        sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        notificationText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        notificationText.fontSize = 15;
        notificationText.text = message;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        gameObject.transform.SetParent(canvas);

        //Get the canvas' dimensions
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasWidth = canvasRect.rect.width;
        canvasHeight = canvasRect.rect.height;

        //Set the dimensions of the object's rect
        float w = rect.rect.width;
        float h = rect.rect.height;
        rect.sizeDelta = new Vector2(w, h);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);

        wantedPos = new Vector2(canvasWidth - w - 20, canvasHeight - h - (canvasHeight / 5));
        Vector2 startingPos = wantedPos;
        startingPos.x = canvasWidth;

        rect.anchoredPosition = startingPos;
    }

    private void FixedUpdate()
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, wantedPos, smoothSpeed * Time.deltaTime);
    }

    public void MoveDown()
    {
        wantedPos.y -= rect.rect.height + 5;
    }

    public void OnDestroy()
    {
        notificationText.enabled = false;
        Destroy(gameObject);
    }
}
