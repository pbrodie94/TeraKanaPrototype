using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;

    [Header("Message Panel")]
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject messagePanel;

    [Header("Weapon Panel")]
    [SerializeField] private Text ammoText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Text weaponName;
    [SerializeField] private GameObject weaponPanel;

    [Header("Aim Reticle")]
    [SerializeField] private RawImage aimRetical;
    [SerializeField] private Texture2D defaultAimRetical;
    private RectTransform aimRect;
    private float fadeSpeed = 10;
    private Color defaultReticalColor;
    private Color wantedColor;

    [Header("Progress Panel")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private GameObject progressPanel;
    private bool activeProgress = false;
    private float timeProgressBarCompleted = 0;

    [Header("Objective Panel")]
    [SerializeField] private Text objectiveText;
    [SerializeField] private GameObject objectivePanel;

    [Header("Notifications")]
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

        progressBar.value = 0;
        progressPanel.SetActive(false);
        //objectivePanel.SetActive(false);
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

    public void UpdateObjective(string objective)
    {
        objectiveText.text = objective;
        objectivePanel.SetActive(true);
    }

    public void UpdateObjective(string objective, int progress, int goal)
    {
        objectiveText.text = objective + progress + " / " + goal;
        objectivePanel.SetActive(true);
    }

    public void InitializeProgressBar(string message, float value)
    {
        progressText.text = message + Mathf.RoundToInt(value) + "%";
        progressBar.value = value;
        activeProgress = true;
        progressPanel.SetActive(true);
    }

    public void UpdateProgressBar(string message, float progress)
    {
        progressText.text = message + Mathf.RoundToInt(progress) + "%";
        progressBar.value = progress;

        if (progressBar.value >= 100)
        {
            timeProgressBarCompleted = Time.time;
            activeProgress = false;
        }
    }

    public void HideProgressBar()
    {
        progressPanel.SetActive(false);
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

        if (progressPanel.activeSelf && !activeProgress)
        {
            if (Time.time >= timeProgressBarCompleted + 5)
            {
                progressPanel.SetActive(false);
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
