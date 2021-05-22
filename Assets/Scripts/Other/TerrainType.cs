using System.Collections.Generic;
using UnityEngine;

namespace CrimsonPlague {
    [System.Serializable]
    public class TerrainType {
        [Header("Textures")]
        public Texture2D normalTex;

        [Space(5)]

        [Header("Temperature")]
        public float tempModifier;

        [Space(5)]

        [Header("Trees")]
        public List<Tree> trees;

        [Space(5)]

        [Header("Structures")]
        public List<GameObject> structures;

        [Space(5)]

        [Header("Details")]
        public List<Detail> details;

        [Space(5)]

        [Header("Skybox")]
        public Material skybox;
    }

    [System.Serializable]
    public class Tree {
        public GameObject tree;

        public Color color1;
        public Color color2;
        public Color lightColor;

        public float minSpawnHeight = 0f;
        public float maxSpawnHeight = 1f;

        public float minSpawnSlope = 0;
        public float maxSpawnSlope = 90f;

        public float minWidth = 0.75f;
        public float maxWidth = 1.5f;

        public float minHeight = 0.5f;
        public float maxHeight = 2.5f;

        public float density = 0.5f;
        public int spacing = 5;
    }

    [System.Serializable]
    public class Detail {
        public GameObject detail;
        public Texture2D detailTex;

        public Color color1;
        public Color color2;

        public float minSpawnHeight = 0f;
        public float maxSpawnHeight = 1f;

        public float minSpawnSlope = 0;
        public float maxSpawnSlope = 90f;

        public float minWidth = 0.75f;
        public float maxWidth = 1.5f;

        public float minHeight = 0.5f;
        public float maxHeight = 2.5f;

        public float overlap = 0.01f;
        public float feather = 0.05f;
        public float density = 0.5f;
        public int spacing = 5;
    }
}