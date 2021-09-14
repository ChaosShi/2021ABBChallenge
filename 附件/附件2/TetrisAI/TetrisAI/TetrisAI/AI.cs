using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisAI
{
    public partial class CodeBehind
    {
        //代码中左下角为原点，向右X正，向上Y正。在Rapid语言中需要对应转换坐标系。
        #region 初始化数据区
        /// <summary>
        /// 列数，最大x坐标+1
        /// </summary>
        public static readonly int columns = 10;
        /// <summary>
        /// 行数，最大y坐标+1
        /// </summary>
        public static readonly int rows = 20;
        /// <summary>
        /// 背景图矩阵，存放0无方块，1有方块
        /// </summary>
        public static double[,] arr;
        /// <summary>
        /// 基于PD算法的AI插空版按钮中间变量
        /// </summary>
        public static bool AI_PD_Fill_In_Btn;
        /// <summary>
        /// 基于PD算法的AI不插空版按钮中间变量
        /// </summary>
        public static bool AI_PD_No_Fill_In_Btn;
        /// <summary> 
        /// 基于PD算法的AI值，返回值依次为：ai_rotate_times [0], ai_pos_x [1], ai_pos_y [2]，方块中心为逆时针旋转中心
        /// </summary>
        public static int[] ai_answer;
        /// <summary>
        /// 单位转化
        /// </summary>
        public static double[] transform_xy;
        /// <summary>
        /// 当前方块对象
        /// </summary>
        public Brick curBrick;
        /// <summary>
        /// 当前分数
        /// </summary>
        public int score = 0;
        /// <summary>
        /// 总方块数
        /// </summary>
        public int brickNum = 0;
        /// <summary>
        /// <returns>List（int）类型，第一个参数是可消总行数，接下来的项则是可消行的y坐标，从大到小排列</returns>
        /// </summary>
        List<int> countRows = new List<int> { 0 };
        /// <summary>
        /// 预定义的int型二维矩阵
        /// </summary>
        private double[,] arr2;
        /// <summary>
        /// 预定义的int型二维矩阵
        /// </summary>
        private double[,] arr3;
        #endregion 初始化数据区

        /// <summary>
        /// AI的控制函数（PD算法，插空版），返回值依次为：ai_rotate_times [0], ai_pos_x [1], ai_pos_y [2]，方块中心为逆时针旋转中心
        /// 需要使用Transform_xy进行坐标转化，ai_rotate_times是相对方块中心的旋转次数，需在RS内设置虚拟旋转轴。
        /// 插空版本
        /// </summary>
        public int[] AI_PD_Fill_In()
        {
            CopyArr2();
            Brick testBrick = new Brick(Brick.Random_TetrisID);
            double index1 = -4.500158825082766;
            double index2 = 3.4181268101392694;
            double index3 = -3.2178882868487753;
            double index4 = -9.348695305445199;
            double index5 = -7.899265427351652;
            double index6 = -3.3855972247263626;
            int[] BuiHeight = new int[columns];//第一个砖块，下面往上数
            int rowTransitions = 0;//行变换
            int holes = 0;//空洞数
            int columnTransitions = 0;//列变换
            int wellSum = 0;//井
            double landingHeight = 0;//落地高度
            int clearRows = 0;//消行数
            int contribution = 0;//贡献数
            List<int> countRows;
            double flag = Double.MinValue;
            double result = 0;
            int ai_rotate_times = 0;
            int ai_pos_x = testBrick.pos.x;
            int ai_pos_y = 0;
            for (int k = 0; k < 4; k++)
            {
                testBrick.pos.x = columns / 2 - 1;
                testBrick.pos.y = rows - 1;
                testBrick.Rotate();
                for (int i0 = 0; i0 < columns; i0++)
                {
                    int Bui = 0;
                    for (int j = 0; j < rows; j++)
                    {
                        if (arr2[i0, j] == 0)
                        {
                            Bui = j;//第i0列的楼高为j
                            break;
                        }
                    }
                    testBrick.pos.x = i0;
                    testBrick.pos.y = Bui;
                    //按列寻找碰撞点
                    while (!testBrick.CanMove(testBrick.pos) && testBrick.pos.y <= rows)
                    {
                        testBrick.pos.y++;
                    }
                    if (testBrick.pos.y > rows)
                        continue;
                    landingHeight = testBrick.pos.y
                        + (testBrick.typeNodes[0].y + testBrick.typeNodes[1].y
                        + testBrick.typeNodes[2].y + testBrick.typeNodes[3].y) / 4;
                    testBrick.CanMove(testBrick.pos);
                    FillArr(arr2, testBrick.posNodes);
                    countRows = CountRow(arr2);
                    //消行数
                    clearRows = countRows[0];
                    countRows.RemoveAt(0);
                    //贡献数
                    foreach (int item in countRows)
                        for (int j = 0; j < columns; j++)
                        {
                            if (arr3[j, item] == 0)
                                contribution++;
                        }
                    //新背景图矩阵
                    foreach (int item in countRows)
                        for (int i = 0; i < columns; i++)
                            for (int j = item; j < rows - 1; j++)
                            {
                                arr2[i, j] = arr2[i, j + 1];
                            }
                    //这里是xy坐标系楼高
                    for (int i = 0; i < columns; i++)
                        for (int j = 0; j < rows; j++)
                            if (arr2[i, j] == 0)
                            {
                                BuiHeight[i] = j - 1;
                                break;
                            }
                    for (int i = 0; i < columns; i++)
                        for (int j = 0; j < BuiHeight[i] - clearRows; j++)
                        {
                            //洞数
                            if (arr2[i, j] == 0)
                            {
                                holes++;
                            }
                        }
                    arr2 = ExpandArr(arr2);
                    //行变换
                    for (int i = 0; i < rows + 2; i++)
                        for (int j = 0; j < columns + 2; j++)
                        {
                            if (j > 0 && arr2[j, i] != arr2[j - 1, i])
                            {
                                rowTransitions++;
                            }
                        }
                    //列变换
                    for (int i = 0; i < columns + 2; i++)
                        for (int j = 0; j < rows + 2; j++)
                        {
                            if (j > 0 && arr2[i, j] != arr2[i, j - 1])
                            {
                                columnTransitions++;
                            }
                        }
                    //井数
                    int temp = 0;
                    for (int i = 1; i <= columns; i++)
                        for (int j = 1; j <= rows; j++)
                        {
                            if (arr2[i, j] == 0 && arr2[i - 1, j] == 1 && arr2[i + 1, j] == 1)
                            {
                                temp++;
                            }
                            else
                            {
                                wellSum += temp * (temp + 1) / 2;
                                temp = 0;
                            }
                        }
                    result = index1 * landingHeight + index2 * clearRows * contribution + index3 * rowTransitions +
                             index4 * columnTransitions + index5 * holes + index6 * wellSum;
                    if (result > flag)
                    {
                        flag = result;
                        ai_rotate_times = k + 1;
                        ai_pos_x = i0;
                        ai_pos_y = testBrick.pos.y;
                    }
                    landingHeight = 0;
                    clearRows = 0;
                    contribution = 0;
                    rowTransitions = 0;
                    columnTransitions = 0;
                    holes = 0;
                    wellSum = 0;
                    CopyArr();
                }
            }
            curBrick.Rotate(ai_rotate_times);
            curBrick.pos.x = ai_pos_x;
            curBrick.pos.y = ai_pos_y;
            int[] AI_answer = new int[] { ai_rotate_times, ai_pos_x, ai_pos_y };
            return AI_answer;
        }

        /// <summary>
        /// AI的控制函数（PD算法，不插空版），返回值依次为：ai_rotate_times [0], ai_pos_x [1], ai_pos_y [2]，方块中心为逆时针旋转中心
        /// 需要使用Transform_xy进行坐标转化，ai_rotate_times是相对方块中心的旋转次数，需在RS内设置虚拟旋转轴。
        /// </summary>
        public int[] AI_PD_No_Fill_In()
        {
            CopyArr2();
            Brick testBrick = new Brick(Brick.Random_TetrisID);
            double index1 = -4.500158825082766;
            double index2 = 3.4181268101392694;
            double index3 = -3.2178882868487753;
            double index4 = -9.348695305445199;
            double index5 = -7.899265427351652;
            double index6 = -3.3855972247263626;
            int[] BuiHeight = new int[columns];//第一个砖块，下面往上数
            int Rowtransitions = 0;//行变换
            int Holes = 0;//空洞数
            int Columntransitions = 0;//列变换
            int Wellsum = 0;//井
            double LandingHeight = 0;//落地高度
            int clearrows = 0;//消行数
            int contribution = 0;//贡献数
            List<int> countrows;
            double flag = Double.MinValue;
            double result = 0;
            int ai_rotate_times = 0;
            int ai_pos_x = testBrick.pos.x;
            int ai_pos_y = testBrick.pos.y;
            for (int k = 0; k < 4; k++)
            {
                testBrick.pos.x = columns / 2 - 1;
                testBrick.pos.y = rows - 1;
                testBrick.Rotate();
                for (int i0 = 0; i0 < columns; i0++)
                {
                    testBrick.pos.x = i0;
                    testBrick.pos.y = rows - 1;
                    //按列寻找碰撞点
                    if (!testBrick.CanMove(testBrick.pos)) continue;
                    while (testBrick.CanMove(testBrick.pos))
                    {
                        testBrick.pos.y--;
                    }
                    if (testBrick.pos.y < 0) continue;
                    LandingHeight = testBrick.pos.y + (testBrick.typeNodes[0].y + testBrick.typeNodes[1].y + testBrick.typeNodes[2].y + testBrick.typeNodes[3].y) / 4;
                    testBrick.CanMove(testBrick.pos);
                    FillArr(arr2, testBrick.posNodes);
                    countrows = CountRow(arr2);
                    //消行数
                    clearrows = countrows[0];
                    countrows.RemoveAt(0);
                    //贡献数
                    foreach (int item in countrows)
                        for (int j = 0; j < columns; j++)
                        {
                            if (arr3[j, item] == 0) contribution++;
                        }
                    //新背景图矩阵
                    foreach (int item in countrows)
                        for (int i = 0; i < columns; i++)
                            for (int j = item; j < rows - 1; j++)
                            {
                                arr2[i, j] = arr2[i, j + 1];
                            }
                    //这里是xy坐标系楼高
                    for (int i = 0; i < columns; i++)
                        for (int j = rows - 1; j >= 0; j--)
                            if (arr2[i, j] == 1)
                            {
                                BuiHeight[i] = j; break;
                            }
                    for (int i = 0; i < columns; i++)
                        for (int j = 0; j < BuiHeight[i] - clearrows; j++)
                        {
                            //洞数
                            if (arr2[i, j] == 0)
                            {
                                Holes++;
                            }
                        }
                    arr2 = ExpandArr(arr2);
                    //行变换
                    for (int i = 0; i < rows + 2; i++)
                        for (int j = 0; j < columns + 2; j++)
                        {
                            if (j > 0 && arr2[j, i] != arr2[j - 1, i])
                            {
                                Rowtransitions++;
                            }
                        }
                    //列变换
                    for (int i = 0; i < columns + 2; i++)
                        for (int j = 0; j < rows + 2; j++)
                        {
                            if (j > 0 && arr2[i, j] != arr2[i, j - 1])
                            {
                                Columntransitions++;
                            }
                        }
                    //井数
                    int temp = 0;
                    for (int i = 1; i <= columns; i++)
                        for (int j = 1; j <= rows; j++)
                        {
                            if (arr2[i, j] == 0 && arr2[i - 1, j] == 1 && arr2[i + 1, j] == 1)
                            {
                                temp++;
                            }
                            else
                            {
                                Wellsum += temp * (temp + 1) / 2;
                                temp = 0;
                            }
                        }
                    result = index1 * LandingHeight + index2 * clearrows * contribution + index3 * Rowtransitions + index4 * Columntransitions + index5 * Holes + index6 * Wellsum;
                    if (result > flag)
                    {
                        flag = result;
                        ai_rotate_times = k + 1;
                        ai_pos_x = i0;
                        ai_pos_y = testBrick.pos.y;
                    }
                    LandingHeight = 0;
                    clearrows = 0;
                    contribution = 0;
                    Rowtransitions = 0;
                    Columntransitions = 0;
                    Holes = 0;
                    Wellsum = 0;
                    CopyArr();
                }
            }
            curBrick.Rotate(ai_rotate_times);
            curBrick.pos.x = ai_pos_x;
            while (curBrick.DropMove())
                curBrick.DropMove();
            if (!curBrick.DropMove())
            {
                FillArr(arr, curBrick.posNodes);
            }

            int[] AI_answer = new int[] { ai_rotate_times, curBrick.pos.x, curBrick.pos.y };
            return AI_answer;
        }


        /// <summary>
        /// 深复制背景图矩阵arr2
        /// </summary>
        private void CopyArr()
        {
            arr2 = new double[columns, rows];
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                {
                    arr2[i, j] = arr[i, j];
                }
        }

        /// <summary>
        /// 深复制背景图矩阵arr2和arr3
        /// </summary>
        private void CopyArr2()
        {
            arr2 = new double[columns, rows];
            arr3 = new double[columns, rows];
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                {
                    arr2[i, j] = arr[i, j];
                    arr3[i, j] = arr[i, j];
                }
        }

        /// <summary>
        /// 将int型二维数组增加一圈
        /// </summary>
        /// <param name="arr2">一个int型二维数组</param>
        /// <returns>一个扩大的int型二维数组</returns>
        private double[,] ExpandArr(double[,] arr2)
        {
            double[,] t_arr = new double[columns + 2, rows + 2];
            for (int i = 1; i <= columns; i++)
                for (int j = 1; j <= rows; j++)
                    t_arr[i, j] = arr2[i - 1, j - 1];
            for (int i = 0; i < columns + 2; i++)
            {
                t_arr[i, 0] = 1;
                t_arr[i, rows + 1] = 1;
            }
            for (int j = 0; j < rows + 2; j++)
            {
                t_arr[0, j] = 1; t_arr[columns + 1, j] = 1;
            }
            return t_arr;
        }



        /// <summary>
        /// 用于把方块写入背景图arr中，只有方块不能下落时才可以调用
        /// </summary>
        /// <param name="arr">背景图矩阵，int型二维数组</param>
        /// <param name="posNodes">以稀疏矩阵的方式存储每个方块对应背景矩阵arr的位置，List（Node）格式</param>
        public void FillArr(double[,] arr, List<Node> posNodes)
        {
            foreach (Node item in posNodes)
            {
                arr[item.x, item.y] = 1;
            }
        }

        /// <summary>
        /// 对一个二维数组（矩阵）计算可消行数，与Cleanrows配套使用
        /// </summary>
        /// <param name="arr">一个二维数组</param>
        /// <returns>List（int）类型，第一个参数是可消总行数，接下来的项则是可消行的z坐标，从大到小排列</returns>
        public List<int> CountRow(double[,] arr)
        {
            List<int> countRows = new List<int>
            {
                0
            };
            bool isFull;
            for (int i = rows - 1; i >= 0; i--)
            {
                isFull = true;
                for (int j = 0; j < columns; j++)
                {
                    if (arr[j, i] == 0)
                    {
                        isFull = false;
                        break;
                    }
                }
                if (isFull)
                {
                    countRows[0]++;
                    countRows.Add(i);
                }
            }
            return countRows;
        }

        /// <summary>
        /// 清除已满的行，与CountRow配套使用
        /// </summary>
        /// <param name="countRows">List（int）类型，第一个参数是可消总行数，接下来项的则是可消行的y坐标，从大到小排列</param>
        private void CleanRows(List<int> countRows)
        {
            score += countRows[0];
            countRows.RemoveAt(0);
            foreach (int item in countRows)
                for (int i = 0; i < columns; i++)
                    for (int j = item; j < rows - 1; j++)
                    {
                        arr[i, j] = arr[i, j + 1];
                    }
        }

        /// <summary>
        ///返回值第一个[0]为x坐标，第二个[1]为y坐标，单位转化：分米到毫米
        /// </summary>
        public double[] Transform_xy(int[] AI_answer)
        {
            double[] AI_answer_Transformed_xy = new double[] { 0, 0 };
            AI_answer_Transformed_xy[0] = AI_answer[1] * 100;
            AI_answer_Transformed_xy[1] = AI_answer[2] * 100;
            return AI_answer_Transformed_xy;
        }
    }
}
