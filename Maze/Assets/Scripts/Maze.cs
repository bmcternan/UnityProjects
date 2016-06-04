using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeighborIndex
{
	public int x;
	public int y;
	public int wallIndex;
	public NeighborIndex(int nx, int ny, int wi)
	{ x = nx; y = ny; wallIndex = wi; }
};
public class Cell
{
	public bool visited = false ;
	public bool[] wall;
};

public class Maze : MonoBehaviour 
{
	public int Width = 3;
	public int Height = 4;
	public float WallThickness = 0.1f ;
	public float CellSize = 1.0f ;
	public float CellHeight = 2.0f ;
	public Material WallMaterial = null;
    public GameController Controller = null;

	private Cell[,] Cells;
	private NeighborIndex[] _neighborSequence;

    private const int WALL_LAYER = 8;

	// Use this for initialization
	void Start () 
	{
		Init (Width, Height);
		GenerateWalls ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void GenerateWall (float x, float y, float sx, float sy, float sz, string name)
	{
		GameObject ptype = null;

		// Transpose from XY to XZ

		ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ptype.transform.position = new Vector3(x, (CellHeight / 2.0f), y);
        ptype.transform.localScale = new Vector3(sx, sz, sy);
		if (WallMaterial != null)
		{
			ptype.GetComponent<Renderer>().material = WallMaterial;
		}
		ptype.transform.parent = transform;
		ptype.name = name;

        Rigidbody wallRigidBody = ptype.AddComponent<Rigidbody>();
        wallRigidBody.isKinematic = false;
        wallRigidBody.useGravity = false;
        wallRigidBody.constraints = RigidbodyConstraints.FreezeAll;

        TargetController wallTarget = ptype.AddComponent<TargetController>();
        wallTarget.Name = "Wall_" + name;
        wallTarget.GameController = Controller;
        ptype.layer = WALL_LAYER;
    }
    private void GenerateWalls ()
	{
		float origin_x = 0.0f;
		float origin_y = 0.0f;
		string name;

		for (int y = 0; y < Height; y++) 
		{
			//origin_y = (float)((float)y -((float)Height/2.0f)) * CellSize ;
			origin_y = ((float)y - ((float)Height /2.0f)) * CellSize ;

			for (int x = 0 ; x < Width ; x++)
			{
				//origin_x = (float)((float)x -((float)Width/2.0f)) * CellSize ;
				origin_x = ((float)x - ((float)Width / 2.0f)) * CellSize ;

				if (y == 0)
				{
					name="c_" +x.ToString() +"_" +y.ToString()+"north";
					// build north wall
					GenerateWall(origin_x+(CellSize/2.0f), origin_y, CellSize, WallThickness, CellHeight,name) ;
				}
				if (x == 0)
				{
					name="c_" +x.ToString() +"_" +y.ToString()+"west";
					// build west wall
					GenerateWall(origin_x, origin_y+(CellSize/2.0f), WallThickness, CellSize, CellHeight,name) ;
				}
				if (Cells[x,y].wall[1])
				{
					name="c_" +x.ToString() +"_" +y.ToString()+"east";
					// build east wall
					GenerateWall(origin_x+CellSize, origin_y+(CellSize/2.0f), WallThickness, CellSize, CellHeight,name) ;
				}
				if (Cells[x,y].wall[2])
				{
					name="c_" +x.ToString() +"_" +y.ToString()+"south";
					// build south wall
					GenerateWall(origin_x+(CellSize/2.0f), origin_y+CellSize, CellSize, WallThickness, CellHeight,name) ;
				}
			}
		}
	}

	private void Init (int desiredWidth, int desiredHeight)
	{
		_neighborSequence = new NeighborIndex[4];
		_neighborSequence[0] = new NeighborIndex(0, -1, 0);
		_neighborSequence[1] = new NeighborIndex(1, 0, 1);
		_neighborSequence[2] = new NeighborIndex(0, 1, 2);
		_neighborSequence[3] = new NeighborIndex(-1, 0, 3) ;
		
		Width = desiredWidth;
		Height = desiredHeight;
		
		Cells = new Cell[Width, Height];
		
		for (int y = 0; y < Height; y++)
			for (int x = 0; x < Width; x++)
		{
			Cells[x, y] = new Cell();
			Cells[x, y].wall = new bool[4] { true, true, true, true };
			
			//if (y != 0)
			//    Cells[x, y].wall[0] = false;
			//if (x != 0)
			//    Cells[x, y].wall[3] = false;
		}
		
		Generate();
	}
	
	public void Generate()
	{
		//Random rnd = new Random();
		//int x = rnd.Next(this.Width);
		//int y = rnd.Next(this.Height);
		int x = Random.Range(0,this.Width);
		int y = Random.Range(0,this.Height);
		int numCells = Width * Height;
		int numVisited = 0;
		int lastDir = -1;
		
		//Debug.WriteLine("start " + x + " " + y);
		
		Stack<NeighborIndex> cellStack = new Stack<NeighborIndex>();
		
		Cells[x, y].visited = true;
		numVisited++;
		
		//Make the initial cell the current cell and mark it as visited
		//While there are unvisited cells
		//        If the current cell has any neighbours which have not been visited
		//            Choose randomly one of the unvisited neighbours
		//            Push the current cell to the stack
		//            Remove the wall between the current cell and the chosen cell
		//            Make the chosen cell the current cell and mark it as visited
		//        Else if stack is not empty
		//            Pop a cell from the stack
		//            Make it the current cell
		while (numVisited < numCells)
		{
			int numUnvisitedNeighbors = 0;
			NeighborIndex[] neighbors = new NeighborIndex[4];
			for (int dir = 0; dir < 4; dir++)
			{
				int dx = x + _neighborSequence[dir].x;
				int dy = y + _neighborSequence[dir].y;
				if ((dx >= 0) && (dx < Width) && (dy >= 0) && (dy < Height))
				{
					if (Cells[dx, dy].visited == false)
					{
						neighbors[numUnvisitedNeighbors] = new NeighborIndex(dx, dy, _neighborSequence[dir].wallIndex);
						numUnvisitedNeighbors++;
					}
				}
			}
			
			if (numUnvisitedNeighbors > 0)
			{
				//int nextIndex = rnd.Next(numUnvisitedNeighbors - 1);
				//int nextIndex = rnd.Next(numUnvisitedNeighbors);
				int nextIndex = Random.Range(0, numUnvisitedNeighbors);

				while ((numUnvisitedNeighbors > 1) && (neighbors[nextIndex].wallIndex == lastDir))
				{
					//nextIndex = rnd.Next(numUnvisitedNeighbors - 1);
					//nextIndex = rnd.Next(numUnvisitedNeighbors);
					nextIndex = Random.Range(0, numUnvisitedNeighbors);
					//if (nextIndex != 0)
					//    Debug.WriteLine("nextIndex " + nextIndex);
				}
				lastDir = neighbors[nextIndex].wallIndex;
				
				cellStack.Push(new NeighborIndex(x, y, -1));
				
				int nx = neighbors[nextIndex].x;
				int ny = neighbors[nextIndex].y;
				int wi = neighbors[nextIndex].wallIndex;
				int nwi = (wi + 2) % 4;
				Cells[x, y].wall[wi] = false;
				Cells[nx, ny].wall[nwi] = false;
				
				x = nx;
				y = ny;
				Cells[x, y].visited = true;
				numVisited++;
			}
			else
			{
				if (cellStack.Count > 0)
				{
					NeighborIndex ni = cellStack.Pop();
					
					x = ni.x;
					y = ni.y;
				}
			}
		}
	}
}
