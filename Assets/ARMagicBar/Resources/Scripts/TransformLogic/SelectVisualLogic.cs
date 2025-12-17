using System.Collections.Generic;
using System.Linq;
using ARMagicBar.Resources.Scripts.Debugging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.TransformLogic
{
    
    //Sits on each transformable object, handles the "selected material" if clicked on
    [RequireComponent(typeof(TransformableObject))]
    public class SelectVisualLogic : MonoBehaviour
    {
        // [SerializeField] private Transform visual;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material selectedMaterialURP;

        private Material materialToApply;
        
        // private Material[] baseSharedMaterials;
        // private List<(Renderer renderer, Material[] baseMaterial, Material[] selectedMaterial)> rendererToMaterials = new List<(Renderer, Material[], Material[])>();
        // private Material[] selectMaterials;

        [FormerlySerializedAs("objectRenderer")]
        [Tooltip(
            "If the objects renderer is somewhere hidden in the hierarchy, rather drag it in, otherwise it will be automatically added.")]
        [SerializeField]
        private List<Renderer> objectRenderers = new();

        [SerializeField] private UnityEngine.Transform parentOfRenderer;
        
        private Dictionary<Renderer, (Material[], Material[])> rendererBaseSelectMaterialDict = new();
        
        private TransformableObject _transformableObject;

        private void Awake()
        {
            objectRenderers = GetRenderer();

            if (objectRenderers != null)
            {
                SetBaseMaterials();
                SetSelectMaterials();
            }
        }
        
        public List<Renderer> ReturnRenderer()
        {
            return objectRenderers == null ? new List<Renderer>() : objectRenderers;
        }
        
        //Get all renderer types
        List<Renderer> GetRenderer()
        {
            List<Renderer> childRenderer = new List<Renderer>();
            CustomLog.Instance.InfoLog("Get Renderer");
            if (GetComponentInChildren<PlacementObjectVisual.PlacementObjectVisual>())
            {
                PlacementObjectVisual.PlacementObjectVisual objectVisual = GetComponentInChildren<PlacementObjectVisual.PlacementObjectVisual>();
                parentOfRenderer = objectVisual.transform;
                
                
                if (objectVisual.GetComponent<Renderer>())
                {
                    if (!objectVisual.GetComponent<ParticleSystem>())
                    {
                        Renderer objectRenderer = objectVisual.GetComponent<Renderer>();
                        childRenderer.Add(objectVisual.GetComponent<Renderer>());
                    }
                }
                else
                {
                    UnityEngine.Transform[] childObjects = objectVisual.GetComponentsInChildren<UnityEngine.Transform>();
                    foreach (var childtransf in childObjects)
                    {
                        if (childtransf.GetComponent<Renderer>())
                        {
                            if (childtransf == null) continue;
                            if (childtransf.GetComponent<Renderer>() == null) continue;
                            if (childtransf.GetComponent<ParticleSystem>()) { continue; }
                            CustomLog.Instance.InfoLog("Adding " + childtransf.GetComponent<Renderer>().name);
                            childRenderer.Add(childtransf.GetComponent<Renderer>());
                        }
                        else { continue; }

                    }
                }
            }
            return childRenderer;
        }

        //Add base material state
        void SetBaseMaterials()
        {
            rendererBaseSelectMaterialDict.Clear();
            if (objectRenderers == null) return;

            foreach (var renderer in objectRenderers)
            {
                if (renderer == null) continue;
                try
                {
                    Material[] baseMaterials = renderer.sharedMaterials;
                    rendererBaseSelectMaterialDict[renderer] = (baseMaterials, null);
                }
                catch (MissingReferenceException)
                {
                    // renderer's native object was destroyed between enumeration and access; skip it
                    continue;
                }

                // (Renderer renderer, Material[] baseMaterials, Material[] selectedMaterials) rendererToMaterial = new(renderer, renderer.sharedMaterials, null);
                // rendererToMaterials.Add(rendererToMaterial);
            }
        }


        
        //Check for URP
        void SetSelectedMaterialDependingOnRP()
        {
            bool isURP = GraphicsSettings.defaultRenderPipeline != null &&
                         GraphicsSettings.defaultRenderPipeline.GetType().Name.Contains("Universal");
            
            if (isURP)
            {
                materialToApply = selectedMaterialURP;
            }
            else
            {
                materialToApply = selectedMaterial;
            }
            
            CustomLog.Instance.InfoLog("Detected Render Pipeline URP? " + isURP);

        }

        //Add selected Material state 
        private void SetSelectMaterials()
        {
            SetSelectedMaterialDependingOnRP();

            if (rendererBaseSelectMaterialDict == null || rendererBaseSelectMaterialDict.Count == 0) return;
            var keys = rendererBaseSelectMaterialDict.Keys.ToList();

            foreach (var rend in keys)
            {
                if (rend == null)
                {
                    rendererBaseSelectMaterialDict.Remove(rend);
                    continue;
                }

                var baseMaterials = rendererBaseSelectMaterialDict[rend].Item1;
                if (baseMaterials == null) continue;

                Material[] selectMaterials = new Material[baseMaterials.Length + 1];
                for (int j = 0; j < baseMaterials.Length; j++)
                {
                    selectMaterials[j] = baseMaterials[j];
                }

                selectMaterials[selectMaterials.Length - 1] = materialToApply;
                rendererBaseSelectMaterialDict[rend] = (baseMaterials, selectMaterials);
            }
        }

        private void Start()
        {
            Hide();
            _transformableObject = GetComponent<TransformableObject>();
            if (_transformableObject != null)
                _transformableObject.OnWasSelected += TransformableObjectWasSelected;
            if (SelectObjectsLogic.Instance != null)
                SelectObjectsLogic.Instance.OnDeselectAll += Hide;
        }
        
        void TransformableObjectWasSelected(bool wasSelected)
        {
           CustomLog.Instance.InfoLog("transformable was selected " +  wasSelected +  " " + transform.name);
            if (wasSelected)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void OnDestroy()
        {
            if (SelectObjectsLogic.Instance != null)
                SelectObjectsLogic.Instance.OnDeselectAll -= Hide;

            if (_transformableObject != null)
                _transformableObject.OnWasSelected -= TransformableObjectWasSelected;
        }
        
        void Show()
        {
            if (rendererBaseSelectMaterialDict == null) return;

            // iterate over a copy of keys to allow removing invalid entries safely
            var keys = rendererBaseSelectMaterialDict.Keys.ToList();
            foreach (var renderer in keys)
            {
                if (renderer == null)
                {
                    rendererBaseSelectMaterialDict.Remove(renderer);
                    continue;
                }

                var selected = rendererBaseSelectMaterialDict[renderer].Item2;
                if (selected == null) continue;

                try
                {
                    renderer.sharedMaterials = selected;
                }
                catch (MissingReferenceException)
                {
                    // renderer was destroyed; remove from dictionary to avoid future attempts
                    rendererBaseSelectMaterialDict.Remove(renderer);
                }
            }
        }

        void Hide()
        {
            if (rendererBaseSelectMaterialDict == null) return;

            var keys = rendererBaseSelectMaterialDict.Keys.ToList();
            foreach (var renderer in keys)
            {
                if (renderer == null)
                {
                    rendererBaseSelectMaterialDict.Remove(renderer);
                    continue;
                }

                var baseMats = rendererBaseSelectMaterialDict[renderer].Item1;
                if (baseMats == null) continue;

                try
                {
                    renderer.sharedMaterials = baseMats;
                }
                catch (MissingReferenceException)
                {
                    // renderer was destroyed; remove its entry
                    rendererBaseSelectMaterialDict.Remove(renderer);
                }
            }
        }
    }
}