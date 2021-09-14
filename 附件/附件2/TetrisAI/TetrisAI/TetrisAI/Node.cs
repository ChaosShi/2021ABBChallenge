using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisAI
{
	public class Node
	{
		/// <summary>
		/// 世界坐标系下x坐标
		/// </summary>
		public int x;
		/// <summary>
		/// 世界坐标系下y坐标
		/// </summary>
		public int y;
		/// <summary>
		/// 重载运算符+使得Node类可以直接相加
		/// </summary>
		/// <param name="a">Node类对象</param>
		/// <param name="b">Node类对象</param>
		/// <returns>Node类对象</returns>
		public static Node operator +(Node a, Node b)
		{
			Node node = new Node
			{
				x = a.x + b.x,
				y = a.y + b.y
			};
			return node;
		}
		/// <summary>
		/// 使Node类对象逆时针旋转90度
		/// </summary>
		/// <returns>逆时针旋转90度后Node类对象</returns>
		public Node Trans()
		{
			Node new_node = new Node
			{
				x = -y,
				y = x
			};
			return new_node;
		}
	}
}
