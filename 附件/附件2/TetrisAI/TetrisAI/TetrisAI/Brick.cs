using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisAI
{
    public class Brick
    {
        /// <summary>
        /// 初始化俄罗斯方块类型
        /// </summary>
        public static int Random_TetrisID = 0;
        /// <summary>
        /// 以稀疏矩阵的方式存储每个方块相对板块的位置
        /// </summary>
        public List<Node> typeNodes;
        /// <summary>
        /// 以稀疏矩阵的方式存储每个方块对应背景矩阵arr的位置
        /// </summary>
        public List<Node> posNodes;
        /// <summary>
        /// 初始下落点板块中心位置
        /// </summary>
        public Node pos = new Node
        {
            x = CodeBehind.columns / 2 - 1,
            y = CodeBehind.rows + 1
        };
        /// <summary>
        /// 新建指定类型方块
        /// </summary>
        public Brick(int index)
        {
            typeNodes = new List<Node>();
            Node node1, node2, node3, node4;
            switch (index)
            {
                //均以每个块的左下角为旋转原点，需要与RS中设置旋转轴一致，坐标与ABB给的图一一对应
                case 1:
                    //右7字形
                    Random_TetrisID = 1;
                    node1 = new Node() { x = 1, y = 2 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 0, y = 2 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 0, y = 1 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 2:
                    //右Z字形
                    Random_TetrisID = 2;
                    node1 = new Node() { x = 1, y = 2 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 1, y = 1 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 0, y = 1 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 3:
                    //7字形
                    Random_TetrisID = 3;
                    node1 = new Node() { x = -1, y = 2 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 0, y = 2 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 0, y = 1 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 4:
                    //左Z字形
                    Random_TetrisID = 4;
                    node1 = new Node() { x = -1, y = 2 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = -1, y = 1 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 0, y = 1 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 5:
                    //一字形
                    Random_TetrisID = 5;
                    node1 = new Node() { x = 3, y = 0 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 2, y = 0 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 1, y = 0 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 6:
                    //田字形
                    Random_TetrisID = 6;
                    node1 = new Node() { x = 0, y = 1 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 1, y = 1 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 1, y = 0 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
                case 7:
                    //山字形
                    Random_TetrisID = 7;
                    node1 = new Node() { x = 1, y = 1 };
                    typeNodes.Add(node1);
                    node2 = new Node() { x = 2, y = 0 };
                    typeNodes.Add(node2);
                    node3 = new Node() { x = 1, y = 0 };
                    typeNodes.Add(node3);
                    node4 = new Node() { x = 0, y = 0 };
                    typeNodes.Add(node4);
                    break;
            }
        }
        /// <summary>
        /// 仅改变typeNode来逆时针旋转，不考虑越界
        /// </summary>
        public void Rotate()
        {
            List<Node> new_typenodes = new List<Node>();
            foreach (Node item in typeNodes)
                new_typenodes.Add(item.Trans());
            typeNodes = new_typenodes;
        }
        /// <summary>
        /// 仅改变typeNode来逆时针旋转，不考虑越界
        /// </summary>
        /// <param name="time">旋转次数</param>
        public void Rotate(int time)
        {
            for (int i = 0; i < time; i++)
                Rotate();
        }

        /// <summary>
        /// 尝试下移，如能就下移
        /// </summary>
        /// <returns></returns>
        public bool DropMove()
        {
            if (CanMove(dpos + pos))
            {
                pos += dpos;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 判断能否移动到new_pos
        /// </summary>
        /// <param name="new_pos">Node类坐标</param>
        /// <returns>返回能否</returns>
        public bool CanMove(Node new_pos)
        {
            List<Node> new_posNodes = new List<Node>();
            foreach (Node item in typeNodes)
            {
                Node new_item = new_pos + item;
                //三边满足
                if (new_item.x >= 0 && new_item.x < CodeBehind.columns && new_item.y >= 0)
                {
                    //上越界
                    if (new_item.y > CodeBehind.rows - 1)
                        continue;
                    //四边满足有重合
                    else if (CodeBehind.arr[new_item.x, new_item.y] == 1)
                        return false;
                    //四边满足无重合
                    else
                        new_posNodes.Add(new_item);
                }
                else return false;
            }
            posNodes = new_posNodes;
            return true;
        }

        /// <summary>
        /// 预定义向下偏移
        /// </summary>
        private static Node dpos = new Node
        {
            x = 0,
            y = -1
        };
    }
}
