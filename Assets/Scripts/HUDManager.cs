using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SerializeField] private Text messageText;
    [SerializeField] private GameObject messagePanel;

    [SerializeField] private Text ammoText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Text weaponName;
    [SerializeField] private GameObject weaponPanel;

    [SerializeField] private RawImage aimRetical;
    [SerializeField] private Texture2D defaultAimRetical;
    private RectTransform aimRect;
    private float fadeSpeed = 10;
    private Color defaultReticalColor;
    private Color wantedColor;

    //Notifications
    private Queue notifications = new Queue();
    private List<Notification> activeNotifications = new List<Notification>();
    private float notificationInterval = 0.3f;
    private float notificationLifeTime = 5;
    private float timeLastNotfied = 0;

    private void Start()
    {
        ShowMessage(null, false);

        aimRect = aimRetical.gameObject.GetComponent<RectTransform>();
        defaultReticalColor = aimRetical.color;
        wantedColor = defaultReticalColor;

        SetDefaultAimRetical();
    }

    public void ShowMessage(string msg, bool show)
    {
        if (show)
        {
            messageText.text = msg;
            messagePanel.SetActive(true);
        } else
        {
            messagePanel.SetActive(false);
        }
    }

    public void UpdateWeaponPanel(bool show)
    {
        if (show)
        {
            weaponPanel.SetActive(true);
        }
        else
        {
            weaponIcon.gameObject.SetActive(false);
            weaponPanel.SetActive(false);
        }
    }

    public void UpdateWeaponPanel(Sprite icon, string name)
    {
        weaponIcon.sprite = icon;
        weaponName.text = name;

        weaponIcon.gameObject.SetActive(true);
    }

    public void UpdateAimRetical(Texture2D newRetical, Vector2 dimensions)
    { 
        if (dimensions == Vector2.zero)
        {
            dimensions = new Vector2(20, 20);
        }

        aimRetical.texture = newRetical;
        aimRect.sizeDelta = dimensions;
    }

    public void SetDefaultAimRetical()
    {
        aimRetical.texture = defaultAimRetical;
        aimRect.sizeDelta = new Vector2(20, 20);
    }

    public void FadeRetical(bool fade)
    {
        if (fade)
        {
            wantedColor.a = 0;
        }
        else
        {
            wantedColor.a = defaultReticalColor.a;
        }
    }

    public void UpdateWeaponPanel(int mag, int ammo)
    {
        ammoText.text = mag + "/" + ammo;
    }

    public void UpdateHealth(float value)
    {
        healthSlider.value = value;
    }

    public void UpdateHealth(float value, float maxValue)
    {
        healthSlider.maxValue = maxValue;
        healthSlider.value = value;
    }    

    public void AddNotification(string msg)
    {
        //Enque messages
        notifications.Enqueue(msg);

    }

    private void LateUpdate()
    {
        //There are notifications to be shown
        if (notifications.Count > 0)
        {
            //Delay showing notifications
            if (activeNotifications.Count < 5 && Time.time >= (timeLastNotfied + notificationInterval))
            {
                //If there are other notifications on screen, move them down
                if (activeNotifications.Count > 0)
                {
                    foreach (Notification n in activeNotifications)
                    {
                        n.MoveDown();
                    }
                }

                //Create and initialize the notifications
                GameObject obj = new GameObject("Notification", typeof(RectTransform));
                Notification newNotification = obj.AddComponent<Notification>();
                newNotification.Initialize(notifications.Dequeue().ToString(), transform);
                activeNotifications.Add(newNotification);

                //Update the time notified for interval tracking
                timeLastNotfied = Time.time;
            }
        }

        //If there are active notifications, check if any have timed out
        if (activeNotifications.Count > 0)
        {
            foreach (Notification n in activeNotifications)
            {
                //If the notification has timed out, destroy them
                if (n.timeShown + notificationLifeTime <= Time.time)
                {
                   if (!n.expired)
                    {
                        n.DestroyMessage();
                    }
                }

                if (n.destroy)
                {
                    activeNotifications.Remove(n);
                    Destroy(n);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Color reticalColor = aimRetical.color;
        reticalColor.a = Mathf.Lerp(reticalColor.a, wantedColor.a, fadeSpeed * Time.deltaTime);
        aimRetical.color = reticalColor;
    }
}
