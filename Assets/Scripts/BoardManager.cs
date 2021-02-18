using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int min;
        public int max;

        public Count(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public int Cols = 8;
    public int Rows = 8;
    public Count WallCount = new Count(5, 9);
    public Count FoodCount = new Count(1, 5);
    public GameObject Exit;
    public GameObject[] WallTiles;
    public GameObject[] FoodTiles;
    public GameObject[] FloorTiles;
    public GameObject[] EnemyTiles;
    public GameObject[] OuterWallTiles;

    private Transform _boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    private void InitialiseList()
    {
        gridPositions.Clear();

        for (int i = 1; i < Cols - 1; i++)
        {
            for (int j = 1; j < Rows - 1; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    private void BoardSetup()
    {
        _boardHolder = new GameObject("Board").transform;

        for (int i = -1; i < Cols + 1; i++)
        {
            for (int j = -1; j < Rows + 1; j++)
            {
                GameObject toInstantiate = FloorTiles[Random.Range(0, FloorTiles.Length)];
                if (i == -1 || i == Cols || j == -1 || j == Rows)
                    toInstantiate = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity);
                instance.transform.SetParent(_boardHolder);
            }
        }
    }

    private Vector3 RandomPos()
    {
        int randIndex = Random.Range(0, gridPositions.Count);
        Vector3 randPos = gridPositions[randIndex];
        gridPositions.RemoveAt(randIndex);
        return randPos;
    }

    private void LayoutObjectsAtRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randPos = RandomPos();
            GameObject randTile = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(randTile, randPos, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectsAtRandom(WallTiles, WallCount.min, WallCount.max);
        LayoutObjectsAtRandom(FoodTiles, FoodCount.min, FoodCount.max);
        
        //layout enemies logarithmically with ascending level number
        int enemyCount = (int) Mathf.Log(level, 2f);
        LayoutObjectsAtRandom(EnemyTiles, enemyCount, enemyCount);

        Instantiate(Exit, new Vector3(Cols - 1, Rows - 1, 0f), Quaternion.identity);
    }
}
