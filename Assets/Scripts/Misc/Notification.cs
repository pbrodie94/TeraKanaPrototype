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
    Color wantedColor;
    float canvasWidth;
    float canvasHeight;
    float smoothSpeed = 15;

    bool _expired = false;
    public bool expired
    {
        get
        {
            return _expired;
        }
    }
    bool _destroy = false;
    public bool destroy
    {
        get
        {
            return _destroy;
        }
    }
    float timeExpired = 0;

    private void Awake()
    {
        wantedColor = Color.white;
    }

    public void Initialize(string msg, Transform canvas, Color baseColor)
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
        notificationText.alignment = TextAnchor.MiddleRight;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        gameObject.transform.SetParent(canvas);

        //Set text alpha so that it fades in
        Color color = baseColor;
        color.a = 0;
        notificationText.color = color;

        wantedColor = baseColor;

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

        wantedPos = new Vector2(canvasWidth - w - 20, canvasHeight - h - (canvasHeight / 10));
        Vector2 startingPos = wantedPos;
        startingPos.x = canvasWidth;

        rect.anchoredPosition = startingPos;
    }

    private void FixedUpdate()
    {
        //Smoothly move position
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, wantedPos, smoothSpeed * Time.deltaTime);

        //Alpha fade in
        Color color = notificationText.color;
        color.a = Mathf.Lerp(color.a, wantedColor.a, (smoothSpeed / 4) * Time.deltaTime);
        notificationText.color = color;

        if (_expired && Time.time >= timeExpired + 0.1f)
        {
            _destroy = true;
        }
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

    public void DestroyMessage()
    {
        wantedPos.x = canvasWidth + rect.rect.width + 5;
        wantedColor.a = 0;
        _expired = true;
        timeExpired = Time.time;
    }
}
