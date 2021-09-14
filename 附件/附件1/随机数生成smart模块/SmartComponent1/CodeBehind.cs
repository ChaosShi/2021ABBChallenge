using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Stations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography; //需要添加的类

/// <summary>
/// 给定一个固定的随机规则：要求参赛选手使用.net framework里的RNGCryptoServiceProvider来生成方块标识随机数，
/// 随机量包括俄罗斯方块的类型，位置和姿态信息。
/// 本设计具体为: 1.{1-7}的7种俄罗斯方块标识数；
///              2.俄罗斯方块在世界坐标系下的X坐标值；
///              3.俄罗斯方块在世界坐标系下的Y坐标值；
///              4.俄罗斯方块绕本地坐标系旋转值(deg)，范围[0-360]；
/// 将上述条件进行修改, 其他保持不变。主要考察选手对整体俄罗斯方块游戏的解决方案的设计及优化，关注全局性：
/// ABB现场提供俄罗斯方块列表，供选手程序使用；或者提供一筐给定的俄罗斯方块模型（选手在设计时需考虑列表的读取和相应的数据结构)。
/// </summary>

namespace SmartComponent1
{
    /// <summary>
    /// Code-behind class for the SmartComponent1 Smart Component.
    /// </summary>
    /// <remarks>
    /// The code-behind class should be seen as a service provider used by the 
    /// Smart Component runtime. Only one instance of the code-behind class
    /// is created, regardless of how many instances there are of the associated
    /// Smart Component.
    /// Therefore, the code-behind class should not store any state information.
    /// Instead, use the SmartComponent.StateCache collection.
    /// </remarks>
    public class CodeBehind : SmartComponentCodeBehind
    {
        /// <summary>
        /// Called when the value of a dynamic property value has changed.
        /// </summary>
        /// <param name="component"> Component that owns the changed property. </param>
        /// <param name="changedProperty"> Changed property. </param>
        /// <param name="oldValue"> Previous value of the changed property. </param>
        public override void OnPropertyValueChanged(SmartComponent component, DynamicProperty changedProperty, Object oldValue)
        {
        }

        /// <summary>
        /// Called when the value of an I/O signal value has changed.
        /// </summary>
        /// <param name="component"> Component that owns the changed signal. </param>
        /// <param name="changedSignal"> Changed signal. </param>
        public override void OnIOSignalValueChanged(SmartComponent component, IOSignal changedSignal)
        {
            if ((changedSignal.Name == "Execute") && ((int)changedSignal.Value == 1))
            {
                // 随机产生1-7的俄罗斯方块标识数（TetrisID），double2int强制转化是舍尾的，RNGCryptoServiceProvider
                // 分辨率为1/255，因此设置（1,8 - 1/255），实际为[1,7]。
                int TetrisID = (int)EncryptedRandom(1,8 - 1/255) ;
                // 初始化变量Tetris_X，Tetris_Y
                double Tetris_X = 0;
                double Tetris_Y = 0;
                // 随机产生1,2,3,4,7型俄罗斯方块的X和Y坐标[300-700],防止极端情况下出界。
                double Tetris_X_12347 = EncryptedRandom(300,700);
                double Tetris_Y_12347 = EncryptedRandom(300,700);
                // 随机产生5型俄罗斯方块的X和Y坐标[400-600],防止极端情况下出界。
                double Tetris_X_5 = EncryptedRandom(400, 600);
                double Tetris_Y_5 = EncryptedRandom(400, 600);
                // 随机产生6型俄罗斯方块的X和Y坐标[200-800],防止极端情况下出界。
                double Tetris_X_6 = EncryptedRandom(200, 800);
                double Tetris_Y_6 = EncryptedRandom(200, 800);
                if (TetrisID == 1 || TetrisID == 2 || TetrisID == 3 || TetrisID == 4 || TetrisID == 7)
                {
                    Tetris_X = Tetris_X_12347;
                    Tetris_Y = Tetris_Y_12347;
                }
                else if (TetrisID == 5)
                {
                    Tetris_X = Tetris_X_5;
                    Tetris_Y = Tetris_Y_5;
                }
                else if (TetrisID == 6)
                {
                    Tetris_X = Tetris_X_6;
                    Tetris_Y = Tetris_Y_6;
                }

                // 随机产生俄罗斯方块绕本地坐标系旋转值(deg)，范围[0-360]。
                double Tetris_Rz = EncryptedRandom(0,360);
                component.Properties["Random_TetrisID"].Value = TetrisID.ToString();
                component.Properties["Random_Tetris_X"].Value = Tetris_X.ToString();
                component.Properties["Random_Tetris_Y"].Value = Tetris_Y.ToString();
                component.Properties["Random_Tetris_Rz"].Value = Tetris_Rz.ToString();
                Logger.AddMessage(new LogMessage("随机生成俄罗斯方块"));
                //用于根据俄罗斯方块标识数执行对应source模块
                switch (TetrisID)
                {
                    case 1:
                        component.IOSignals["Executed_Tetris_1"].Value = 1;
                        break;

                    case 2:
                        component.IOSignals["Executed_Tetris_2"].Value = 1;
                        break;

                    case 3:
                        component.IOSignals["Executed_Tetris_3"].Value = 1;
                        break;

                    case 4:
                        component.IOSignals["Executed_Tetris_4"].Value = 1;
                        break;

                    case 5:
                        component.IOSignals["Executed_Tetris_5"].Value = 1;
                        break;

                    case 6:
                        component.IOSignals["Executed_Tetris_6"].Value = 1;
                        break;

                    case 7:
                        component.IOSignals["Executed_Tetris_7"].Value = 1;
                        break;
                    default:
                        break;
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
        public static double EncryptedRandom(double min, double max)
        {
            // 创建一个字节数组来保存随机值。
            byte[] randomNumber = new byte[1];
            // 创建RNGCryptoServiceProvider的新实例。
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            // 用一个随机值填充数组，范围0-255。
            Gen.GetBytes(randomNumber);
            // 将字节转换为整数值。
            int rand = Convert.ToInt32(randomNumber[0]);
            // 将一个字节的随机数转化为整数。
            double result;
            result = rand / 255.0;
            result = result * (max - min) + min;
            return result;
        }
    }
}
