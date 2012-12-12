using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace tileWorld
{
    class PathNode
    {
        public Cell mapCell;
        public PathNode parent;
        public int g;
        public int cost;
        
        public PathNode(Cell mapCell, PathNode parent, int g, int h)
        {
            this.mapCell = mapCell;
            this.parent = parent;
            this.g = g;
            this.cost = g + h;
        }
    }

    class Pathfinder
    {
        World world;


        public Pathfinder(World world)
        {
            this.world = world;
        }

        public void FindPath(Vector2 startPoint, Vector2 endPoint)
        {
            List<PathNode> closedList = new List<PathNode>();
            List<PathNode> openList = new List<PathNode>();

            Cell currentCell = world.getCellFromPixelPos(startPoint);
            Cell endCell = world.getCellFromPixelPos(endPoint);
            PathNode current = new PathNode(currentCell, null, 0, 0);
            openList.Add(current);
         
            while (true)
            {
                // get adjacent cells
                Cell[] adjacentcells = world.getCellArray(current.mapCell);
                foreach (Cell cell in adjacentcells)
                {
                    if (!cell.Collision)
                    {
                        if (!nodeInList(cell, openList) & !nodeInList(cell, closedList) )
                        {
                            int distance = (int)Vector2.Distance(cell.tilePosition, endCell.tilePosition);
                            openList.Add(new PathNode(cell, current, 0, distance));                            
                        }
                    }
                }

                
                PathNode nextCurrent = openList[0];
                foreach (PathNode node in openList)
                {
                    if (node.cost < nextCurrent.cost)
                    {
                        nextCurrent = node;
                    }
                }
               
                openList.Remove(current);
                current.mapCell.color = Color.Red;
                closedList.Add(current);


                current = nextCurrent;
                if (current.mapCell.tilePosition == endCell.tilePosition | closedList.Count > 1000)
                {
                    current.mapCell.color = Color.Red;
                    closedList.Add(current);
                    break;
                }
            }

            
        }

        private bool nodeInList(Cell cell, List<PathNode> nodes)
        {
            foreach (PathNode node in nodes)
            {
                if (cell == node.mapCell)
                    return true; 
            }

            return false;
        }

    }
}
