﻿using System.Collections;
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

    [Header("Interact Panel")]
    [SerializeField] private Slider interactBar;
    [SerializeField] private Text interactText;
    [SerializeField] private GameObject interactPanel;

    [Header("Objective Panel")]
    [SerializeField] private Text objectiveText;
    [SerializeField] private GameObject objectivePanel;

    [Header("Notifications")]
    private Queue notifications = new Queue();
    private List<Notification> activeNotifications = new List<Notification>();
    private float notificationInterval = 0.3f;
    private float notificationLifeTime = 5;
    private float timeLastNotfied = 0;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    bool paused = false;
    public bool isPaused
    {
        get
        {
            return paused;
        }
    }

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

        interactPanel.SetActive(false);

        pauseMenu.SetActive(false);
        paused = false;
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

    public void InitializeProgressBar(string message, float progress)
    {
        progressText.text = message + Mathf.RoundToInt(progress) + "%";
        progressBar.value = progress;
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

    public void UpdateInteractBar(string msg, float progress)
    {
        if (!interactPanel.activeSelf)
            interactPanel.SetActive(true);

        interactText.text = msg + Mathf.RoundToInt(progress) + "%";
        interactBar.value = progress;
    }

    public void HideInteractBar()
    {
        interactPanel.SetActive(true);
    }

    public enum NotificationType
    {
        Normal,
        Alert,
        Warning
    }

    struct NotificationTemplate
    {
        public NotificationType type;
        public string message;
        public Color color;

        public NotificationTemplate (string msg, NotificationType notificationType)
        {
            message = msg;
            type = notificationType;
            color = Color.white;

            SetColor();
        }

        public NotificationTemplate(string msg, Color col)
        {
            message = msg;
            color = col;
            type = NotificationType.Normal;
        }

        void SetColor()
        {
            switch (type)
            {
                case NotificationType.Normal:
                    color = Color.white;
                    break;

                case NotificationType.Warning:
                    color = Color.yellow;
                    break;

                case NotificationType.Alert:
                    color = Color.red;
                    break;
            }
        }
    }

    public void AddNotification(string msg, NotificationType notificationType)
    {
        //Enque messages
        notifications.Enqueue(new NotificationTemplate(msg, notificationType));

    }

    public void AddNotification(string msg, Color color)
    {
        //Enque messages
        notifications.Enqueue(new NotificationTemplate(msg, color));

    }

    public void AddNotification(string msg)
    {
        //Enqueue messages
        notifications.Enqueue(new NotificationTemplate(msg, NotificationType.Normal));

    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        paused = true;
    }

    public void UnPauseGame()
    {
        //Unpause
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        paused = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown(InputManager.Pause))
        {

            Debug.Log("Pause");
            if (!paused)
            {
                PauseGame();
            }
            else
            {
                UnPauseGame();
            }
        }
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


                NotificationTemplate note = (NotificationTemplate)notifications.Dequeue();
                string msg = note.message;
                Color color = note.color;
                
                newNotification.Initialize(note.message, transform, color);
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
