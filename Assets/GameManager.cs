using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int rows = 20;
    const int columns = 20;

    public GameObject backgroundCube;
    public GameObject lifeCube;

    public int generationLifeTime = 5;
    public List<GameObject> cells;

    private bool _gameOn = true;
    private State[,] grid;
    private State[,] nextGeneration;

    public enum State
    {
        Dead,
        Alive,
    }


    // Start is called before the first frame update
    void Start()
    {
        grid = new State[rows, columns];

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                int random = Random.Range(0, 2);
                grid[row, column] = (State)random;
                Debug.Log(grid.GetType());

                GameObject spawnEntity;

                if (random == 0)
                {
                    spawnEntity = backgroundCube;
                }
                else
                {
                    spawnEntity = lifeCube;
                }
                var instance = Instantiate(spawnEntity, new Vector3(row, 0, column), Quaternion.identity);
                cells.Add(instance);
            }
        }
        StartCoroutine(ChangeGeneration());
    }

    private IEnumerator ChangeGeneration()
    {
        while (_gameOn)
        {
            yield return new WaitForSeconds(generationLifeTime);
            NextGeneration();
            ChangeVisualization();
            grid = nextGeneration;
        }

    }

    private void ChangeVisualization()
    {
        List<GameObject> futureCells = new List<GameObject>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var cell = nextGeneration[row, column];

                if(cell == State.Alive)
                {
                    foreach (var item in cells)
                    {
                        float x = item.transform.position.x;
                        float z = item.transform.position.z;

                        if ( x == row && z == column)
                        {
                            Destroy(item);
                            var instance = Instantiate(lifeCube, new Vector3(row, 0, column), Quaternion.identity);
                            futureCells.Add(instance);
                        }
                    }
                }
                else
                {
                    foreach (var item in cells)
                    {
                        float x = item.transform.position.x;
                        float z = item.transform.position.z;

                        if (x == row && z == column)
                        {
                            Destroy(item);
                            var instance = Instantiate(backgroundCube, new Vector3(row, 0, column), Quaternion.identity);
                            futureCells.Add(instance);
                        }
                    }
                }
            }
        }
        cells = futureCells;
    }

    private void NextGeneration()
    {
        nextGeneration = new State[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var aliveNeighbors = 0;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        try
                        {
                            aliveNeighbors += grid[row + i, column + j] == State.Alive ? 1 : 0;
                        }
                        catch (System.Exception)
                        {
                            aliveNeighbors += grid[row, column] == State.Alive ? 1 : 0;
                        }
                    }

                }

                var currentCell = grid[row, column];

                aliveNeighbors -= currentCell == State.Alive ? 1 : 0;

                if (currentCell == State.Alive && aliveNeighbors < 2)
                {
                    nextGeneration[row, column] = State.Dead;
                }

                else if (currentCell == State.Alive && aliveNeighbors > 3)
                {
                    nextGeneration[row, column] = State.Dead;
                }

                else if (currentCell == State.Dead && aliveNeighbors == 3)
                {
                    nextGeneration[row, column] = State.Alive;
                }

                else
                {
                    nextGeneration[row, column] = currentCell;
                }
            }
        }
    }
}