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
                    if ((!cell.Collision) & !nodeInList(cell, closedList))
                    {
                        //int distance = (int)Vector2.Distance(cell.tilePosition, endCell.tilePosition);
                        int h = 10 * (int)(Math.Abs(cell.tilePosition.X - endCell.tilePosition.X) + Math.Abs(cell.tilePosition.Y - endCell.tilePosition.Y));
                        int g = 10;

                        if (!nodeInList(cell, openList))
                        {
                            openList.Add(new PathNode(cell, current, g, h));
                        }
                        else
                        {
                            PathNode node = getPathNodeByCell(cell, openList);
                            node.cost = g + h;
                        }
                        cell.cost = h;
                    }
                }


                int testCost = 100000; 
                
                openList.Remove(current);
                closedList.Add(current);
                
                foreach (PathNode node in openList)
                {
                    if (node.cost <= testCost)
                    {
                        current = node;
                        testCost = node.cost;
                    }
                }

                if (closedList.Count > 5000)
                {
                    return null;
                }
                else if (current.mapCell.tilePosition == endCell.tilePosition )
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
