using UnityEngine;

namespace Dizzy.Calendar
{
    internal static class CalendarPrefabFactory
    {
        public const float BoardWidth = 0.42f;
        public const float BoardHeight = 0.52f;
        public const float BoardDepth = 0.03f;

        public static GameObject Create()
        {
            var root = new GameObject("calendar");
            root.SetActive(false);
            Object.DontDestroyOnLoad(root);

            var filter = root.AddComponent<MeshFilter>();
            filter.sharedMesh = BuildBoardMesh();

            var renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = CreateBoardMaterial();

            var box = root.AddComponent<BoxCollider>();
            box.size = new Vector3(BoardWidth, BoardHeight, BoardDepth);
            box.isTrigger = true;

            var rb = root.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            var saveable = root.AddComponent<SaveablePrefab>();
            saveable.prefabIndex = CalendarItem.PrefabIndex;

            var item = root.AddComponent<CalendarItem>();
            item.holdDistance = 0.82f;
            item.furniturePlaceHeight = 0.15f;
            item.mass = 1f;
            item.value = 200;
            item.name = "calendar";
            item.category = TransactionCategory.furniture;
            item.inventoryScale = 1f;
            item.inventoryRotation = 180f;
            item.floaterHeight = 1.6f;
            item.wallAttachment = true;

            var textGo = new GameObject("date_text");
            textGo.transform.SetParent(root.transform, false);
            textGo.transform.localPosition = new Vector3(0f, 0.02f, -BoardDepth * 0.5f - 0.002f);
            textGo.transform.localRotation = Quaternion.identity;
            textGo.transform.localScale = Vector3.one;

            var text = textGo.AddComponent<TextMesh>();
            text.alignment = TextAlignment.Center;
            text.anchor = TextAnchor.MiddleCenter;
            text.characterSize = 0.035f;
            text.fontSize = 48;
            text.color = new Color(0.12f, 0.1f, 0.08f, 1f);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = CalendarItem.FormatDayLabel();

            item.BindDateText(text);

            return root;
        }

        private static Material CreateBoardMaterial()
        {
            Shader shader = Shader.Find("Standard") ?? Shader.Find("Diffuse") ?? Shader.Find("Sprites/Default");
            var mat = new Material(shader)
            {
                name = "Dizzy.Calendar.Board",
                color = new Color(0.72f, 0.55f, 0.32f, 1f),
                hideFlags = HideFlags.HideAndDontSave,
            };
            return mat;
        }

        private static Mesh BuildBoardMesh()
        {
            float w = BoardWidth * 0.5f;
            float h = BoardHeight * 0.5f;
            float d = BoardDepth * 0.5f;

            var mesh = new Mesh { name = "calendar_board" };
            mesh.vertices = new[]
            {
                new Vector3(-w, -h, -d), new Vector3(w, -h, -d), new Vector3(w, h, -d), new Vector3(-w, h, -d),
                new Vector3(-w, -h, d), new Vector3(w, -h, d), new Vector3(w, h, d), new Vector3(-w, h, d),
            };
            mesh.triangles = new[]
            {
                0, 2, 1, 0, 3, 2,
                4, 5, 6, 4, 6, 7,
                0, 1, 5, 0, 5, 4,
                2, 3, 7, 2, 7, 6,
                0, 4, 7, 0, 7, 3,
                1, 2, 6, 1, 6, 5,
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
