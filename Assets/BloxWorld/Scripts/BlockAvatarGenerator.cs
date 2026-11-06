using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BloxWorld
{
    // Visual child only: attach movement/collision to the parent player object.
    public sealed class BlockAvatarGenerator : MonoBehaviour
    {
        [SerializeField] Material materialTemplate;
        [SerializeField] AvatarData preview = new AvatarData();
        readonly List<Material> materials = new List<Material>();
        Transform visualRoot;
        GameManager game;

        void Start()
        {
            game = GameManager.Instance;
            if (game != null) game.StateChanged += Refresh;
            Refresh();
        }

        void Refresh()
        {
            Build(game != null ? game.State.avatar : preview,
                game == null || game.State.calmMode);
        }

        public void Build(AvatarData appearance, bool calmMode)
        {
            Clear();
            appearance = appearance ?? new AvatarData();
            visualRoot = new GameObject("GeneratedAvatar").transform;
            visualRoot.SetParent(transform, false);
            Part("Torso", new Vector3(0, 1.35f, 0), new Vector3(0.9f, 0.9f, 0.45f), appearance.shirt);
            Part("Head", new Vector3(0, 2.08f, 0), Vector3.one * 0.6f, appearance.head);
            Part("LeftArm", new Vector3(-0.68f, 1.35f, 0), new Vector3(0.4f, 0.9f, 0.4f), appearance.head);
            Part("RightArm", new Vector3(0.68f, 1.35f, 0), new Vector3(0.4f, 0.9f, 0.4f), appearance.head);
            Part("LeftLeg", new Vector3(-0.24f, 0.45f, 0), new Vector3(0.42f, 0.9f, 0.42f), appearance.trousers);
            Part("RightLeg", new Vector3(0.24f, 0.45f, 0), new Vector3(0.42f, 0.9f, 0.42f), appearance.trousers);
            // Front is local +Z. Faces use geometry rather than external textures/fonts.
            float eyeHeight = appearance.face % 2 == 0 ? 0.08f : 0.035f;
            Part("LeftEye", new Vector3(-0.13f, 2.12f, 0.308f), new Vector3(0.065f, eyeHeight, 0.02f), Color.black);
            Part("RightEye", new Vector3(0.13f, 2.12f, 0.308f), new Vector3(0.065f, eyeHeight, 0.02f), Color.black);
            Part("Smile", new Vector3(0, 1.97f, 0.308f), new Vector3(0.17f, 0.035f, 0.02f), Color.black);
            if (appearance.hat)
            {
                Part("HatBrim", new Vector3(0, 2.41f, 0), new Vector3(0.85f, 0.08f, 0.75f), appearance.shirt);
                Part("HatTop", new Vector3(0, 2.57f, 0), new Vector3(0.5f, 0.28f, 0.5f), appearance.shirt);
            }
            if (appearance.wings)
            {
                var left = Part("LeftWing", new Vector3(-0.7f, 1.55f, -0.35f), new Vector3(0.8f, 0.6f, 0.12f), Color.cyan);
                var right = Part("RightWing", new Vector3(0.7f, 1.55f, -0.35f), new Vector3(0.8f, 0.6f, 0.12f), Color.cyan);
                left.localRotation = Quaternion.Euler(0, 0, -25);
                right.localRotation = Quaternion.Euler(0, 0, 25);
            }
            if (appearance.trail && !calmMode)
            {
                var anchor = new GameObject("Trail");
                anchor.transform.SetParent(visualRoot, false);
                anchor.transform.localPosition = new Vector3(0, 0.5f, -0.3f);
                var trail = anchor.AddComponent<TrailRenderer>();
                trail.sharedMaterial = NewMaterial(appearance.shirt);
                trail.time = 0.35f;
                trail.startWidth = 0.18f;
                trail.endWidth = 0;
                trail.minVertexDistance = 0.1f;
                trail.startColor = appearance.shirt;
                trail.endColor = new Color(appearance.shirt.r, appearance.shirt.g, appearance.shirt.b, 0);
                trail.shadowCastingMode = ShadowCastingMode.Off;
                trail.receiveShadows = false;
            }
        }

        Transform Part(string partName, Vector3 position, Vector3 size, Color color)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = partName;
            part.transform.SetParent(visualRoot, false);
            part.transform.localPosition = position;
            part.transform.localScale = size;
            var collider = part.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
            part.GetComponent<Renderer>().sharedMaterial = NewMaterial(color);
            return part.transform;
        }

        Material NewMaterial(Color color)
        {
            Material material;
            if (materialTemplate != null) material = new Material(materialTemplate);
            else
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                if (shader == null) throw new System.InvalidOperationException("Assign a material template for this render pipeline.");
                material = new Material(shader);
            }
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            materials.Add(material);
            return material;
        }
        void Clear()
        {
            if (visualRoot != null)
            {
                visualRoot.gameObject.SetActive(false);
                Destroy(visualRoot.gameObject);
            }
            foreach (var material in materials) if (material != null) Destroy(material);
            materials.Clear();
        }
        void OnDestroy()
        {
            if (game != null) game.StateChanged -= Refresh;
            Clear();
        }
    }
}
