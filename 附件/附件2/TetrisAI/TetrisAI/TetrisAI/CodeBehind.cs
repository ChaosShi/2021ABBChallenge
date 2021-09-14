using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Stations;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisAI
{
    /// <summary>
    /// Code-behind class for the 俄罗斯方块AI算法smart模块 Smart Component.为了正确载入Smart组件，并共享AI类，将AI类名称为CodeBehind。
    /// </summary>
    /// <remarks>
    /// The code-behind class should be seen as a service provider used by the 
    /// Smart Component runtime. Only one instance of the code-behind class
    /// is created, regardless of how many instances there are of the associated
    /// Smart Component.
    /// Therefore, the code-behind class should not store any state information.
    /// Instead, use the SmartComponent.StateCache collection.
    /// </remarks>
    public partial class CodeBehind : SmartComponentCodeBehind
    {
        /// <summary>
        /// Called when the value of a dynamic property value has changed.
        /// </summary>
        /// <param name="component"> Component that owns the changed property. </param>
        /// <param name="changedProperty"> Changed property. </param>
        /// <param name="oldValue"> Previous value of the changed property. </param>
        public override void OnPropertyValueChanged(SmartComponent component, DynamicProperty changedProperty, Object oldValue)
        {
            //更新俄罗斯方块ID数值变化
            if (changedProperty.Name == "Random_TetrisID")
            {
                Brick.Random_TetrisID = Convert.ToInt32(changedProperty.Value.ToString());
            }
        }

        /// <summary>
        /// Called when the value of an I/O signal value has changed.
        /// </summary>
        /// <param name="component"> Component that owns the changed signal. </param>
        /// <param name="changedSignal"> Changed signal. </param>
        public override void OnIOSignalValueChanged(SmartComponent component, IOSignal changedSignal)
        {
            if ((changedSignal.Name == "NewGame") && ((int)changedSignal.Value == 1))   //新局
            {
                component.Properties["Random_TetrisID"].Value = 0;
                component.Properties["Tetris_AI_x"].Value = 0;
                component.Properties["Tetris_AI_y"].Value = 0;
                component.Properties["Tetris_AI_Rz_Times"].Value = 0;
                component.Properties["GameOver"].Value = 0;
                component.IOSignals["AI_PD_No_Fill_In"].Value = 0;
                component.IOSignals["AI_PD_Fill_In"].Value = 0;

                arr = new double[columns, rows];
                curBrick = null;
                score = 0;
                brickNum = 0;
                Logger.AddMessage(new LogMessage("执行基于PD算法的俄罗斯方块AI程序smart组件，每消除1行为1分"));
            }
            #region AI选择
            if ((changedSignal.Name == "AI_PD_Fill_In") && ((int)changedSignal.Value == 1))   
            {
                component.IOSignals["AI_PD_No_Fill_In"].Value = 0;
                AI_PD_Fill_In_Btn = true;
                AI_PD_No_Fill_In_Btn = false;
                Logger.AddMessage(new LogMessage("执行插空版AI"));
            }
            if ((changedSignal.Name == "AI_PD_No_Fill_In") && ((int)changedSignal.Value == 1))   
            {
                component.IOSignals["AI_PD_Fill_In"].Value = 0;
                AI_PD_Fill_In_Btn = false;
                AI_PD_No_Fill_In_Btn = true;
                Logger.AddMessage(new LogMessage("执行不插空版AI"));
            }
            #endregion
            if ((changedSignal.Name == "NewBrick") && ((int)changedSignal.Value == 1)) //新俄罗斯方块落下
            {
                CleanRows(countRows);                //清除已满的行
                countRows = new List<int> { 0 };     //可清除的行归0
                if (curBrick == null)
                {
                    curBrick = new Brick(Brick.Random_TetrisID);
                    brickNum += 1;
                }
                if (AI_PD_Fill_In_Btn == true && AI_PD_No_Fill_In_Btn == false) 
                {
                    ai_answer = AI_PD_Fill_In();
                    transform_xy = Transform_xy(ai_answer);
                }
                if (AI_PD_Fill_In_Btn == false && AI_PD_No_Fill_In_Btn == true)
                {
                    ai_answer = AI_PD_No_Fill_In();
                    transform_xy = Transform_xy(ai_answer);
                }
                curBrick.CanMove(curBrick.pos);
                lock (curBrick)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        if (arr[i, rows - 1] == 1)
                        {
                            component.Properties["GameOver"].Value = 1;//0为可以继续游戏，1为游戏结束
                            Logger.AddMessage(new LogMessage("游戏结束！"));
                            return;
                        }
                    }
                    if (!curBrick.DropMove())
                    {
                        FillArr(arr, curBrick.posNodes);
                        countRows = CountRow(arr);
                        curBrick = null;
                    }
                    component.Properties["Tetris_AI_x"].Value = transform_xy[0].ToString();
                    component.Properties["Tetris_AI_y"].Value = transform_xy[1].ToString();
                    component.Properties["Tetris_AI_Rz_Times"].Value = ai_answer[0].ToString();
                    Logger.AddMessage(new LogMessage("俄罗斯方块ID为：" + Brick.Random_TetrisID
                                                       + "；旋转标识数：" + ai_answer[0]
                                                       + "；x坐标：" + transform_xy[0] + "mm"
                                                       + "；y坐标：" + transform_xy[1] + "mm"));
                    Logger.AddMessage(new LogMessage("当前分数：" + score + "分"));
                }
                component.IOSignals["Executed"].Value = 1;
            }

        }

        /// <summary>
        /// Called during simulation.
        /// </summary>
        /// <param name="component"> Simulated component. </param>
        /// <param name="simulationTime"> Time (in ms) for the current simulation step. </param>
        /// <param name="previousTime"> Time (in ms) for the previous simulation step. </param>
        /// <remarks>
        /// For this method to be called, the component must be marked with
        /// simulate="true" in the xml file.
        /// </remarks>
        public override void OnSimulationStep(SmartComponent component, double simulationTime, double previousTime)
        {
        }
    }
}
