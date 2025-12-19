using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    private class RewardEntry
    {
        public string tag;
        public int score;
    }

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private RewardEntry[] rewardEntries = new RewardEntry[]
    {
        new RewardEntry { tag = "Crystal", score = 35 },
        new RewardEntry { tag = "Ingot", score = 25 },
        new RewardEntry { tag = "Jar", score = 15 }
    };

    [SerializeField] 
    private bool destroyOnHit = false;

    [SerializeField]
    private int destroyDelay = 5;

    private Dictionary<string, int> rewardMap = new Dictionary<string, int>(StringComparer.Ordinal);

    public static event Action OnScoreChanged;

    private void Awake()
    {
        BuildRewardMap();
        UIButtonHandler.OnUIRestartButtonPressed += () => SetScore(0);
        UIButtonHandler.OnUIResetButtonPressed += () => SetScore(0);
        UIButtonHandler.OnUIScoreButtonPressed += HandleScoreChanged;
        TimeHandler.OnCountdownChanged += HandleTimeChanged;

    }

    private void HandleTimeChanged()
    {
        score -= 15;
        RefreshScoreText();
    }

    private void HandleScoreChanged()
    {
        OnScoreChanged?.Invoke();
        AddScore(5);
    }

    private void Start()
    {
        RefreshScoreText();
    }

    private void BuildRewardMap()
    {
        rewardMap.Clear();
        if (rewardEntries == null) return;

        for (int i = 0; i < rewardEntries.Length; i++)
        {
            var e = rewardEntries[i];
            if (e == null || string.IsNullOrEmpty(e.tag)) 
                continue;
            rewardMap[e.tag] = e.score;
        }
    }

    private void AttachForwardersToAllSpheres()
    {
        var spheres = GameObject.FindGameObjectsWithTag("Sphere");
        for (int i = 0; i < spheres.Length; i++)
        {
            var s = spheres[i];
            if (s == null) continue;
            if (s.GetComponent<CollisionForwarder>() == null)
            {
                var forwarder = s.AddComponent<CollisionForwarder>();
                forwarder.SetOwner(this);
            }
        }
    }

    private void Update()
    {

        AttachForwardersToAllSpheres();
        RefreshScoreText();


    }

    private void HandleCollision(GameObject other)
    {
        if (other == null) return;

        var otherTag = other.tag;
        if (string.IsNullOrEmpty(otherTag)) return;

        if (rewardMap.TryGetValue(otherTag, out int points))
        {
            AddScore(points);

            if (destroyOnHit)
                Destroy(other);

            if (!other.TryGetComponent<PendingDespawn>(out _))
            {
                var pd = other.AddComponent<PendingDespawn>();
                pd.Initialize(Color.red, 10.0f, destroyDelay);
                Destroy(other, destroyDelay);
            }

            return;
        }
    }

    public void AddScore(int amount)
    {
        if (amount == 0) return;
        score += amount;
        RefreshScoreText();
    
    }

    private void RefreshScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public int GetScore() => score;

    public void SetScore(int newScore)
    {
        score = newScore;
        RefreshScoreText();

       
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIRestartButtonPressed -= () => SetScore(0);
        UIButtonHandler.OnUIResetButtonPressed -= () => SetScore(0);
        UIButtonHandler.OnUIScoreButtonPressed -= HandleScoreChanged;
        TimeHandler.OnCountdownChanged -= HandleTimeChanged;

    }

    private class PendingDespawn : MonoBehaviour
    {
    private Color outlineColor = Color.red;
    private float pulseSpeed = 10f;
    private float emissionMultiplier = 2f;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private Material[][] modifiedMaterials;

    private Coroutine pulseCoroutine;

    public void Initialize(Color outlineColor, float pulseSpeed = 2f, float destroyDelay = 5f)
    {
        this.outlineColor = outlineColor;
        this.pulseSpeed = pulseSpeed;
        StartEffect();
    }

    private void Start()
    {
      
        if (renderers == null)
            StartEffect();
    }

    private void StartEffect()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalMaterials = new Material[renderers.Length][];
        modifiedMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;


            var mats = r.materials; 
            originalMaterials[i] = new Material[mats.Length];
            modifiedMaterials[i] = new Material[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                var mat = mats[j];
                if (mat == null) continue;

                
                originalMaterials[i][j] = new Material(mat);

                
                var mod = new Material(mat);
                if (mod.HasProperty("_EmissionColor"))
                {
                   
                    mod.EnableKeyword("_EMISSION");
                    Color baseEmission = mod.GetColor("_EmissionColor");
                    mod.SetColor("_EmissionColor", outlineColor * (emissionMultiplier * 0.5f));
                }
                else if (mod.HasProperty("_Color"))
                {
                    
                    Color baseCol = mod.GetColor("_Color");
                    mod.SetColor("_Color", Color.Lerp(baseCol, outlineColor, 0.5f));
                }

                modifiedMaterials[i][j] = mod;
                mats[j] = mod;
            }

      
            r.materials = mats;
        }

        pulseCoroutine = StartCoroutine(PulseEmission());
    }

    private IEnumerator PulseEmission()
    {
   
        bool anyHasEmission = false;
        for (int i = 0; i < modifiedMaterials.Length && !anyHasEmission; i++)
        {
            var arr = modifiedMaterials[i];
            if (arr == null) continue;
            for (int j = 0; j < arr.Length; j++)
            {
                var m = arr[j];
                if (m != null && m.HasProperty("_EmissionColor"))
                {
                    anyHasEmission = true;
                    break;
                }
            }
        }

        if (!anyHasEmission) yield break;

        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f) + 0.5f; // 0..1
            for (int i = 0; i < modifiedMaterials.Length; i++)
            {
                var arr = modifiedMaterials[i];
                if (arr == null) continue;
                for (int j = 0; j < arr.Length; j++)
                {
                    var m = arr[j];
                    if (m == null) continue;
                    if (m.HasProperty("_EmissionColor"))
                    {
                        
                        m.SetColor("_EmissionColor", outlineColor * (emissionMultiplier * Mathf.Lerp(0.25f, 1.0f, t)));
                    }
                }
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (r == null) continue;

                var originals = originalMaterials[i];
                if (originals == null) continue;

                try
                {
                    r.materials = originals;
                }
                catch
                {
                    
                }
            }
        }

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);
    }
}

private class CollisionForwarder : MonoBehaviour
    {
        private ScoreHandler owner;

        public void SetOwner(ScoreHandler owner)
        {
            this.owner = owner;
        }

        private void OnTriggerEnter(Collider other)
        {
            owner?.HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            owner?.HandleCollision(collision.gameObject);
        }
    }
}