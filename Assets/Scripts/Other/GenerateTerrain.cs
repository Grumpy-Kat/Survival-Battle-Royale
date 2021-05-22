using System.Collections.Generic;
using UnityEngine;

namespace CrimsonPlague {
    public class GenerateTerrain : MonoBehaviour {
        [Header("Loading")]
        [SerializeField]
        GameObject loadingMenu;

        [Space(10)]

        [Header("Voronoi")]
        [SerializeField]
        int minNumPeaks = 3;
        [SerializeField]
        int maxNumPeaks = 7;
        [SerializeField]
        float minPeakHeight = 0.1f;
        [SerializeField]
        float maxPeakHeight = 0.5f;
        [SerializeField]
        float minSlope = 1.5f;
        [SerializeField]
        float maxSlope = 2.5f;
        [SerializeField]
        float minCurve = 2.5f;
        [SerializeField]
        float maxCurve = 5f;

        [Space(10)]

        [Header("Canyon")]
        float digAmt = 0.05f;
        float slope = 0.001f;

        [Space(10)]

        [Header("Smooth")]
        [SerializeField]
        int smoothAmt = 1;

        [Space(10)]

        [Header("Water")]
        [SerializeField]
        GameObject water;
        [SerializeField]
        public float waterLevel = 22.5f;
        [SerializeField]
        Material seaFoam;
        [SerializeField]
        float shoreSize = 15f;

        [Space(10)]

        [Header("Terrain Types")]
        [SerializeField]
        int index = -1;
        [SerializeField]
        List<TerrainType> terrainTypes = new List<TerrainType>();

        [HideInInspector]
        public int seed = 0;

        public float Generate() {
            Random.InitState(seed);

            // Start loading
            loadingMenu.SetActive(true);

            // Create terrain
            Terrain terrain = gameObject.GetComponent<Terrain>();
            TerrainCollider collider = gameObject.GetComponent<TerrainCollider>();
            TerrainData data = new TerrainData();
            data.heightmapResolution = 1024;
            data.baseMapResolution = 1024;
            data.alphamapResolution = 1024;
            data.SetDetailResolution(1024, 32);
            data.size = new Vector3(1024, 256, 1024);
            terrain.detailObjectDistance = 400;

            // Choose random type of terrain, such as desert, snowy, fall, or spring
            TerrainType currType;
            if (index == -1 || index >= terrainTypes.Count) {
                currType = terrainTypes[Random.Range(0, terrainTypes.Count)];
            } else {
                currType = terrainTypes[index];
            }

            // Create textures
            SplatPrototype[] splatPrototypes = new SplatPrototype[1];
            splatPrototypes[0] = new SplatPrototype();
            splatPrototypes[0].texture = currType.normalTex;
            splatPrototypes[0].tileSize = new Vector2(150, 150);
            data.splatPrototypes = splatPrototypes;

            // Generate heights of terrains
            float[,] heights = new float[data.heightmapWidth, data.heightmapHeight];
            heights = Voronoi(heights, data.heightmapWidth, data.heightmapHeight, minNumPeaks, maxNumPeaks, minPeakHeight, maxPeakHeight, minSlope, maxSlope, minCurve, maxCurve);
            heights = CreateCanyon(heights, data.heightmapWidth, data.heightmapHeight, digAmt, slope);
            heights = Smooth(heights, data.heightmapWidth, data.heightmapHeight, smoothAmt);
            data.SetHeights(0, 0, heights);
            
            // Create trees
            if (currType.trees.Count > 0) {
                data.treePrototypes = AddTrees(currType.trees);
                data.treeInstances = GenerateTrees(currType.trees, data).ToArray();
            }

            // Create details, such as bushes, plants, and rocks
            if (currType.details.Count > 0) {
                data.detailPrototypes = AddDetails(currType.details);
                for (int i = 0; i < currType.details.Count; i++) {
                    data.SetDetailLayer(0, 0, i, GenerateDetails(currType.details[i], data));
                }
            }

            // Add water
            GenerateWater(water, waterLevel, seaFoam, shoreSize, data);

            // Set skybox
            if (currType.skybox != null && RenderSettings.skybox != currType.skybox) {
                RenderSettings.skybox = currType.skybox;
            }

            // Finalize data
            terrain.terrainData = data;
            collider.terrainData = data;
            
            // Stop loading
            loadingMenu.SetActive(false);
            
            // Return temperature modifier
            return currType.tempModifier;
        }

        float[,] PerlinNoise(float[,] heights, int worldWidth, int worldHeight, float minPerlinNoiseScale, float maxPerlinNoiseScale, float smoothScale) {
            float pnScale = Random.Range(minPerlinNoiseScale, maxPerlinNoiseScale);

            for (int x = 0; x < worldWidth; x++) {
                for (int y = 0; y < worldHeight; y++) {
                    heights[x, y] = Mathf.PerlinNoise(seed + ((float)x / worldWidth / pnScale), seed + ((float)y / worldHeight / pnScale)) / smoothScale;
                }
            }

            return heights;
        }

        float[,] Voronoi(float[,] heights, int worldWidth, int worldHeight, int minNumPeaks, int maxNumPeaks, float minPeakHeight, float maxPeakHeight, float minSlope, float maxSlope, float minCurve, float maxCurve) {
            int peaks = Random.Range(minNumPeaks, maxNumPeaks);

            for (int i = 0; i < peaks; i++) {
                // Choose random peak that is lower than peak height
                Vector3 peak = new Vector3(Random.Range(0, worldWidth), Random.Range(minPeakHeight, maxPeakHeight), Random.Range(0, worldHeight));
                if (heights[(int)peak.x, (int)peak.z] < peak.y) {
                    heights[(int)peak.x, (int)peak.z] = peak.y;
                } else {
                    i--;
                    continue;
                }

                // Randomize values for peak
                float slope = Random.Range(minSlope, maxSlope);
                float curve = Random.Range(minCurve, maxCurve);

                // Gradually change terrain based on new peak
                float maxDist = Vector2.Distance(new Vector2(0, 0), new Vector2(worldWidth, worldHeight));
                for (int x = 0; x < worldWidth; x++) {
                    for (int y = 0; y < worldHeight; y++) {
                        if (!(x == peak.x && y == peak.z)) {
                            float distToPeak = Vector2.Distance(new Vector2(peak.x, peak.z), new Vector2(x, y)) / maxDist;
                            float height = peak.y - distToPeak * slope - Mathf.Pow(distToPeak, curve);
                            if (height > heights[x, y]) {
                                heights[x, y] = height;
                            }
                        }
                    }
                }
            }

            return heights;
        }

        float[,] CreateCanyon(float[,] heights, int worldWidth, int worldHeight, float digAmt, float slope) {
            int x = 0;
            int y = Random.Range(10, worldHeight - 10);

            while (y >= 0 && y < worldHeight && x >= 0 && x < worldWidth) {
                // Add canyon based on random position and vary it slightly each iteration until went across whole map
                heights = Dig(heights, worldWidth, worldHeight, x, y, heights[x, y] - digAmt, slope);
                x += Random.Range(-1, 3);
                y += Random.Range(-2, 3);
            }

            return heights;
        }

        float[,] Dig(float[,] heights, int worldWidth, int worldHeight, int x, int y, float height, float slope) {
            if (x < 0 || x >= worldWidth) {
                return heights;
            }
            if (y < 0 || y >= worldHeight) {
                return heights;
            }
            if (height < 0) {
                return heights;
            }
            if (heights[x, y] <= height) {
                return heights;
            }

            // Able to dig canyon at this coordinate
            heights[x, y] = height;

            // Recursively dig around it
            heights = Dig(heights, worldWidth, worldHeight, x + 1, y, height + Random.Range(slope, slope + 0.01f), slope);
            heights = Dig(heights, worldWidth, worldHeight, x - 1, y, height + Random.Range(slope, slope + 0.01f), slope);
            heights = Dig(heights, worldWidth, worldHeight, x + 1, y + 1, height + Random.Range(slope, slope + 0.01f), slope);
            heights = Dig(heights, worldWidth, worldHeight, x - 1, y - 1, height + Random.Range(slope, slope + 0.01f), slope);
            heights = Dig(heights, worldWidth, worldHeight, x, y + 1, height + Random.Range(slope, slope + 0.01f), slope);
            heights = Dig(heights, worldWidth, worldHeight, x, y - 1, height + Random.Range(slope, slope + 0.01f), slope);

            return heights;
        }

        TreePrototype[] AddTrees(List<Tree> trees) {
            TreePrototype[] treeProtos = new TreePrototype[trees.Count];

            for (int i = 0; i < trees.Count; i++) {
                //Change tree game objects into trees that Unity terrain system can use
                treeProtos[i] = new TreePrototype();
                treeProtos[i].prefab = trees[i].tree;
            }

            return treeProtos;
        }

        List<TreeInstance> GenerateTrees(List<Tree> trees, TerrainData data) {
            List<TreeInstance> treeObjs = new List<TreeInstance>();

            for (int i = 0; i < trees.Count; i++) {
                for (int x = 0; x < data.size.x; x += trees[i].spacing) {
                    for (int y = 0; y < data.size.z; y += trees[i].spacing) {
                        // Check if tree can be spawned based on density chance
                        if (Random.Range(0f, 1f) < trees[i].density) {
                            float height = data.GetHeight(x, y) / data.size.y;
                            float steepness = data.GetSteepness(x / (float)data.size.x, y / (float)data.size.z);

                            // Check if valid position for tree based on its height and steepness settings
                            if (height >= trees[i].minSpawnHeight && height <= trees[i].maxSpawnHeight && steepness >= trees[i].minSpawnSlope && steepness <= trees[i].maxSpawnSlope) {
                                TreeInstance treeObj = new TreeInstance();
                                float actualX = Mathf.Clamp(x + Random.Range((float)-trees[i].spacing, (float)trees[i].spacing), 0, data.size.x);
                                float actualY = Mathf.Clamp(y + Random.Range((float)-trees[i].spacing, (float)trees[i].spacing), 0, data.size.z);
                                
                                treeObj.position = new Vector3(actualX / data.size.x, (data.GetHeight((int)actualX, (int)actualY) / data.size.y) - 0.01f, actualY / data.size.z);
                                treeObj.rotation = Random.Range(0, 360);
                                treeObj.prototypeIndex = i;
                                treeObj.color = Color.Lerp(trees[i].color1, trees[i].color2, Random.Range(0f, 1f));
                                treeObj.lightmapColor = trees[i].lightColor;
                                treeObj.heightScale = Random.Range(trees[i].minHeight, trees[i].maxHeight);
                                treeObj.widthScale = Random.Range(trees[i].minWidth, trees[i].maxWidth);
                                
                                treeObjs.Add(treeObj);
                            }
                        }
                    }
                }
            }

            return treeObjs;
        }

        void GenerateStructures(float worldWidth, float worldHeight, int minNumStructures, int maxNumStructures, List<GameObject> structures) {
            int numStructures = Random.Range(minNumStructures, maxNumStructures);
            if (structures.Count > 0) {
                for (int i = 0; i < numStructures;) {
                    float x = (Random.Range(0.0f, 1.0f) * worldWidth) + transform.position.x;
                    float y = 500.0f;
                    float z = (Random.Range(0.0f, 1.0f) * worldHeight) + transform.position.z;
                    GameObject gameObj = GameObject.Instantiate(structures[Random.Range(0, structures.Count)], new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                    RaycastHit hit;
                    Ray ray = new Ray(new Vector3(x, y, z), Vector3.down);
                    if (Physics.Raycast(ray, out hit)) {
                        y -= hit.distance;
                        if (hit.transform.tag == "Terrain" && y > 25f) {
                            gameObj.transform.rotation = Quaternion.FromToRotation(gameObj.transform.up, hit.normal) * gameObj.transform.rotation;
                            i++;
                        }
                    }
                }
            }
        }

        DetailPrototype[] AddDetails(List<Detail> details) {
            DetailPrototype[] detailProtos = new DetailPrototype[details.Count];
            for (int i = 0; i < details.Count; i++) {
                detailProtos[i] = new DetailPrototype();
                detailProtos[i].prototype = details[i].detail;
                detailProtos[i].prototypeTexture = details[i].detailTex;
                detailProtos[i].dryColor = details[i].color1;
                detailProtos[i].healthyColor = details[i].color2;
                detailProtos[i].minWidth = details[i].minWidth;
                detailProtos[i].maxWidth = details[i].maxWidth;
                detailProtos[i].minHeight = details[i].minHeight;
                detailProtos[i].maxHeight = details[i].maxHeight;
                if (details[i].detail != null) {
                    detailProtos[i].usePrototypeMesh = true;
                    detailProtos[i].renderMode = DetailRenderMode.VertexLit;
                } else {
                    detailProtos[i].usePrototypeMesh = false;
                    detailProtos[i].renderMode = DetailRenderMode.GrassBillboard;
                }
            }
            return detailProtos;
        }

        int[,] GenerateDetails(Detail detail, TerrainData data) {
            int[,] details = new int[data.detailWidth, data.detailHeight];
            for (int x = 0; x < data.detailWidth; x += detail.spacing) {
                for (int y = 0; y < data.detailHeight; y += detail.spacing) {
                    if (Random.Range(0f, 1f) < detail.density) {
                        int worldX = (int)(x / (float)data.detailWidth * data.heightmapWidth);
                        int worldY = (int)(y / (float)data.detailHeight * data.heightmapHeight);
                        float noise = MapValue(Mathf.PerlinNoise(seed + (x * detail.feather), seed + (y * detail.feather)), 0, 1, 0.75f, 1);
                        float minSpawnHeight = detail.minSpawnHeight * noise - detail.overlap * noise;
                        float maxSpawnHeight = detail.maxSpawnHeight * noise + detail.overlap * noise;
                        float height = data.GetHeight(worldY, worldX) / data.size.y;
                        float steepness = data.GetSteepness(worldX / (float)data.size.x, worldY / (float)data.size.z);
                        if (height >= minSpawnHeight && height <= maxSpawnHeight && steepness >= detail.minSpawnSlope && steepness <= detail.maxSpawnSlope) {
                            details[y, x] = 1;
                        }
                    }
                }
            }
            return details;
        }

        void GenerateWater(GameObject water, float waterLevel, Material seaFoam, float shoreSize, TerrainData data) {
            if (water != null) {
                GameObject waterObj = GameObject.Instantiate(water, new Vector3(0, waterLevel, 0), Quaternion.identity);
                waterObj.transform.localScale = new Vector3(1024f, 1, 1024f);
                waterObj.layer = 4;
                float[,] heights = data.GetHeights(0, 0, data.heightmapWidth, data.heightmapHeight);
                List<GameObject> shores = new List<GameObject>();
                for (int x = 0; x < data.heightmapWidth; x++) {
                    for (int y = 0; y < data.heightmapHeight; y++) {
                        Vector2 pos = new Vector2(x, y);
                        List<Vector2> neighbors = GetNeighbors(pos, data.heightmapWidth, data.heightmapHeight);
                        foreach (Vector2 neighbor in neighbors) {
                            if (heights[x, y] < waterLevel / data.size.y && heights[(int)neighbor.x, (int)neighbor.y] > waterLevel / data.size.y) {
                                GameObject shore = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                shore.transform.localScale *= shoreSize;
                                shore.transform.position = new Vector3(y / (float)data.heightmapHeight * data.size.z, waterLevel, x / (float)data.heightmapWidth * data.size.x);
                                shore.transform.LookAt(new Vector3(neighbor.y / (float)data.heightmapHeight * data.size.z, waterLevel, neighbor.x / (float)data.heightmapWidth * data.size.x));
                                shore.transform.Rotate(new Vector3(90, 0, 0));
                                shores.Add(shore);
                            }
                        }
                    }
                }
                MeshFilter[] meshFilters = new MeshFilter[shores.Count];
                for (int i = 0; i < shores.Count; i++) {
                    meshFilters[i] = shores[i].GetComponent<MeshFilter>();
                }
                CombineInstance[] combine = new CombineInstance[shores.Count];
                for (int i = 0; i < shores.Count; i++) {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.SetActive(false);
                }
                GameObject shoreLine = new GameObject();
                shoreLine.name = "ShoreLine";
                shoreLine.layer = 4;
                shoreLine.AddComponent<WaveAnimation>();
                shoreLine.transform.position = this.transform.position;
                shoreLine.transform.rotation = this.transform.rotation;
                MeshFilter mesh = shoreLine.AddComponent<MeshFilter>();
                mesh.mesh = new Mesh();
                shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
                MeshRenderer renderer = shoreLine.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = seaFoam;
                for (int i = 0; i < shores.Count; i++) {
                    DestroyImmediate(shores[i]);
                }
            }
        }

        float[,] Smooth(float[,] heights, int worldWidth, int worldHeight, int smoothAmt) {
            for (int i = 0; i < smoothAmt; i++) {
                for (int x = 0; x < worldWidth; x++) {
                    for (int y = 0; y < worldHeight; y++) {
                        List<Vector2> neighbors = GetNeighbors(new Vector2(x, y), worldWidth, worldHeight);
                        float total = heights[x, y];
                        foreach (Vector2 neighbor in neighbors) {
                            total += heights[(int)neighbor.x, (int)neighbor.y];
                        }
                        heights[x, y] = total / ((float)neighbors.Count + 1);
                    }
                }
            }
            return heights;
        }

        List<Vector2> GetNeighbors(Vector2 pos, int worldWidth, int worldHeight) {
            List<Vector2> neighbors = new List<Vector2>();
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if (!(x == 0 && y == 0)) {
                        Vector2 neighorPos = new Vector2(Mathf.Clamp(pos.x + x, 0, worldWidth - 1), Mathf.Clamp(pos.y + y, 0, worldHeight - 1));
                        if (!neighbors.Contains(neighorPos)) {
                            neighbors.Add(neighorPos);
                        }
                    }
                }
            }
            return neighbors;
        }

        float MapValue(float val, float orgMin, float orgMax, float newMin, float newMax) {
            return (val - orgMin) * (newMax - newMin) / (orgMax - orgMin) + newMin;
        }
    }
}