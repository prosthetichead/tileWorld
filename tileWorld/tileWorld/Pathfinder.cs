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

        public List<Cell> FindCellPath(Vector2 startPoint, Vector2 endPoint)
        {
            List<PathNode> closedList = new List<PathNode>();
            List<PathNode> openList = new List<PathNode>();
            List<Cell> cellPath = new List<Cell>();

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
                    if (!cell.Collision & !nodeInList(cell, closedList))
                    {
                        int distance = (int)Vector2.Distance(cell.tilePosition, endCell.tilePosition);
                        int g;
                        if (cell.tilePosition.X != currentCell.tilePosition.X & cell.tilePosition.Y != currentCell.tilePosition.Y)
                        {
                            g = 14;
                        }
                        else
                        {
                            g= 10;
                        }

                        if (!nodeInList(cell, openList))
                        {
                            openList.Add(new PathNode(cell, current, g, distance));
                        }
                        else
                        {
                            PathNode node = getPathNodeByCell(cell, openList);
                            node.cost = g + distance;
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
                closedList.Add(current);


                current = nextCurrent;
                if (current.mapCell.tilePosition == endCell.tilePosition | closedList.Count > 1000)
                {
                    
                    closedList.Add(current);
                    break;
                }
            } //while loop


            while (current != null)
            {
                cellPath.Insert(0, current.mapCell);
                //current.mapCell.color = Color.Red;
                current = current.parent;
            }
            
            return cellPath;
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


        private PathNode getPathNodeByCell(Cell cell, List<PathNode> nodes)
        {
            foreach (PathNode node in nodes)
            {
                if (cell == node.mapCell)
                    return node;
            }

            return null;
        }

    }
}
