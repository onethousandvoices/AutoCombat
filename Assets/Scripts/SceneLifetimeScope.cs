using AutoCombat.AI;
using AutoCombat.Camera;
using AutoCombat.Combat;
using AutoCombat.Core;
using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using AutoCombat.Input;
using AutoCombat.Player;
using AutoCombat.Spawn;
using AutoCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public sealed class SceneLifetimeScope : LifetimeScope
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private EnemyConfig _enemyConfig;
    [SerializeField] private CameraConfig _cameraConfig;
    [SerializeField] private RoomConfig _roomConfig;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private EnemyView _enemyPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_playerConfig);
        builder.RegisterInstance(_enemyConfig);
        builder.RegisterInstance(_cameraConfig);
        builder.RegisterInstance(_roomConfig);
        builder.RegisterInstance(_inputActions);

        builder.Register<PlayerModel>(Lifetime.Singleton);
        builder.Register<CombatModel>(Lifetime.Singleton);

        builder.Register<SteeringAvoidance>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<AttackAnimator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<PatrolWaypointProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register(_ => new ComponentPool<EnemyView>(_enemyPrefab, 5), Lifetime.Singleton);

        SpawnRoom();
        var playerView = SpawnPlayer();
        builder.RegisterComponent(playerView);

        var killCounterView = CreateHud();
        builder.RegisterComponent(killCounterView);

        builder.Register<InputController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<OrbitalCameraController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<PlayerController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<CombatController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<EnemyController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<EnemySpawnController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }

    private void SpawnRoom()
    {
        var roomGo = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        roomGo.transform.position = Vector3.zero;
        if (!roomGo.TryGetComponent<RoomView>(out _))
            roomGo.AddComponent<RoomView>();
    }

    private static PlayerView SpawnPlayer()
    {
        var playerGo = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        playerGo.transform.position = new(0f, 1f, 0f);
        if (!playerGo.TryGetComponent<PlayerView>(out var view))
            view = playerGo.AddComponent<PlayerView>();
        return view;
    }

    private static KillCounterView CreateHud()
    {
        var canvasGo = new GameObject("HudCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        if (!FindFirstObjectByType<EventSystem>())
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        var killGo = new GameObject("KillCounter");
        killGo.transform.SetParent(canvasGo.transform, false);
        var tmp = killGo.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.text = "Kills: 0";
        var killRect = killGo.GetComponent<RectTransform>();
        killRect.anchorMin = new(0, 1);
        killRect.anchorMax = new(0, 1);
        killRect.pivot = new(0, 1);
        killRect.anchoredPosition = new(20, -20);
        killRect.sizeDelta = new(300, 50);
        var killView = killGo.AddComponent<KillCounterView>();

        var btnGo = new GameObject("ExitButton");
        btnGo.transform.SetParent(canvasGo.transform, false);
        var btnImage = btnGo.AddComponent<Image>();
        btnImage.color = new(0.8f, 0.2f, 0.2f, 0.9f);
        var btn = btnGo.AddComponent<Button>();
        var btnRect = btnGo.GetComponent<RectTransform>();
        btnRect.anchorMin = new(1, 1);
        btnRect.anchorMax = new(1, 1);
        btnRect.pivot = new(1, 1);
        btnRect.anchoredPosition = new(-20, -20);
        btnRect.sizeDelta = new(50, 50);

        var btnTextGo = new GameObject("Label");
        btnTextGo.transform.SetParent(btnGo.transform, false);
        var btnTmp = btnTextGo.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "X";
        btnTmp.fontSize = 32;
        btnTmp.color = Color.white;
        btnTmp.alignment = TextAlignmentOptions.Center;
        var btnTextRect = btnTextGo.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        var exitView = btnGo.AddComponent<ExitButtonView>();
        btn.onClick.AddListener(exitView.OnClick);

        return killView;
    }
}
