using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour

{

    public class Cell
    {
        public bool visited = false; //check if the place already have a room
        public bool [] status = new bool[4]; //check which door's gonna open
    }

    public Vector2Int size; //size of the dungeon
    public int startPos = 0; //initial position
    public GameObject[] rooms; //use this prefab
    public Vector2 offset; //distance between each room

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
      MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)//go through every x
        {
            for (int j = 0; j < size.y; j++)// go through every y
            {
                Cell currentCell = board[Mathf.FloorToInt(i+j*size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = Random.Range(0, rooms.Length);
                    var newRoom = Instantiate(rooms[randomRoom], new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehavior>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                }
                
            }
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i< size.x; i++)//go through every x
        {
            for (int j = 0; j < size.y; j++)//go through every y
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos; //keeping track which position we are at

        Stack<int> path = new Stack<int>(); //keeping track the path

        int k =0;

        while (k < 1000)
        {
            k++;
            board[currentCell].visited = true; //make visited true

            if(currentCell == board.Count -1)
            {
                break;
            }

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0) //no available neighbors
            {
                if (path.Count == 0) //reach to the last cell
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop(); //current cell is added to the last path
                }
            }
            else // if there IS an available neighbor
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)]; //new cell would be placed randomly in the available neighbors

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 ==currentCell) 
                    {
                        board[currentCell].status[2] = true; //current room right open
                        currentCell = newCell;
                        board[currentCell].status[3] = true; //new room left open
                    }
                    else
                    {
                        board[currentCell].status[1] = true; //current room down true
                        currentCell = newCell;
                        board[currentCell].status[0] = true; //new room up true
                    }
                }
                else
                {
                    if (newCell + 1 ==currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
