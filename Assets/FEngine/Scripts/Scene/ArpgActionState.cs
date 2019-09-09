//----------------------------------------------
//  F2DEngine: time: 2017.2  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public class ArpgActionState
    {
        public class PlayerArpgData : FArpgMachine.FArgpBaseData
        {
            public PlayerArpgType nType;
            public ArpgData mArpgData;
        }

        public enum PlayerArpgType
        {
            PAT_IDLE = 0,  //空闲
            PAT_SHOOT,   //普通攻击
            PAT_SHOOT_01,//普通01
            PAT_SHOOT_02,//普通01
            PAT_SHOOT_03,//普通01
            PAT_JUMPSHOOT,//跳跃攻击
            PAT_JUMPSHOOT_01,//跳跃攻击01
            PAT_JUMP,//跳跃
            PAT_WALK,//走
            PAT_WALKSHOOT,//走射
            PAT_DEF,//受击

        }

        [Serializable]
        public class ArpgData
        {
            public int A = 5;
        }

        static public void SetArpgData(PlayerArpgData pad,Dictionary<PlayerArpgType, ArpgData> data)
        {
            if (data != null && data.ContainsKey(pad.nType))
            {
                pad.mArpgData = data[pad.nType];
            }
        }

        static  public void InitArpg(FArpgMachine arpgMachine,Dictionary<PlayerArpgType, ArpgData> buffs)
        {
            FArpgMachine mFArpgMachine = arpgMachine;
            FArpgMachine.FArpgNode root = mFArpgMachine.GetRoots();

            //待机文件
            PlayerArpgData idleData = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 0,
                nType = PlayerArpgType.PAT_IDLE,
               
            };
            SetArpgData(idleData,buffs);

            PlayerArpgData shootDataOver = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 0.5f,
                nType = PlayerArpgType.PAT_IDLE,
            };

            SetArpgData(shootDataOver, buffs);
            PlayerArpgData shootData = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "Shoot",
                conditionTimes = 0,
                nType = PlayerArpgType.PAT_SHOOT,
            };
            SetArpgData(shootData, buffs);
            PlayerArpgData shootData01 = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "Shoot01",
                conditionTimes = 0.6f,
                nType = PlayerArpgType.PAT_SHOOT_01,
            };
            SetArpgData(shootData01, buffs);
            PlayerArpgData shootData02 = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "Shoot02",
                conditionTimes = 0.6f,
                nType = PlayerArpgType.PAT_SHOOT_02,
            };
            SetArpgData(shootData02, buffs);
            PlayerArpgData shootData03 = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "Shoot03",
                conditionTimes = 0.6f,
                nType = PlayerArpgType.PAT_SHOOT_03,
            };
            SetArpgData(shootData03, buffs);
            PlayerArpgData shootAllOver = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 1.2f,
                nType = PlayerArpgType.PAT_IDLE,
            };

            SetArpgData(shootAllOver, buffs);
            PlayerArpgData jumpShoot01 = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "JumpShoot01",
                conditionTimes = 0f,
                nType = PlayerArpgType.PAT_JUMPSHOOT,
            };
            SetArpgData(jumpShoot01, buffs);
            PlayerArpgData jumpShoot02 = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "JumpShoot02",
                conditionTimes = 0.4f,
                nType = PlayerArpgType.PAT_JUMPSHOOT_01,
            };
            SetArpgData(jumpShoot02, buffs);
            PlayerArpgData jumpData = new PlayerArpgData()
            {
                conditionState = "Jump",
                nodeName = "Jump",
                conditionTimes = 0,
                nType = PlayerArpgType.PAT_JUMP,
            };
            SetArpgData(jumpData, buffs);
            PlayerArpgData jumpOverData = new PlayerArpgData()
            {
                conditionState = "JumpOver",
                nodeName = "Idle",
                conditionTimes = 0,
                nType = PlayerArpgType.PAT_IDLE,
            };
            SetArpgData(jumpOverData, buffs);
            PlayerArpgData walkData = new PlayerArpgData()
            {
                conditionState = "Walk",
                nodeName = "Walk",
                conditionTimes = 0,
                nType = PlayerArpgType.PAT_WALK,
            };
            SetArpgData(walkData, buffs);
            PlayerArpgData walkDataEx = new PlayerArpgData()
            {
                conditionState = "Walk",
                nodeName = "Walk",
                conditionTimes = 0.7f,
                nType = PlayerArpgType.PAT_WALK,
            };
            SetArpgData(walkDataEx, buffs);
            PlayerArpgData walkOverData = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 0.2f,
                nType = PlayerArpgType.PAT_IDLE,
            };
            SetArpgData(walkOverData, buffs);
            PlayerArpgData shootOverData = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 0.6f,
                nType = PlayerArpgType.PAT_IDLE,
            };
            SetArpgData(shootOverData, buffs);
            PlayerArpgData walkshootData = new PlayerArpgData()
            {
                conditionState = "Shoot",
                nodeName = "Walkshoot",
                conditionTimes = 0.1f,
                nType = PlayerArpgType.PAT_WALKSHOOT,
            };
            SetArpgData(walkshootData, buffs);

            PlayerArpgData defData = new PlayerArpgData()
            {
                conditionState = "Def",
                nodeName = "Def",
                conditionTimes = 0.0f,
                nType = PlayerArpgType.PAT_DEF,
            };
            SetArpgData(defData, buffs);
            PlayerArpgData defOverData = new PlayerArpgData()
            {
                conditionState = "",
                nodeName = "Idle",
                conditionTimes = 0.5f,
                nType = PlayerArpgType.PAT_IDLE,
            };

            SetArpgData(defOverData, buffs);


            //状态机设置

            //待机
            FArpgMachine.FArpgNode idel = root.Regs(idleData);
            {
                //受击
                idel.Regs(defData).Regs(defOverData,false,FArpgMachine.Greater);

                //射击
                FArpgMachine.FArpgNode shoot = idel.Regs(shootData, true);
                {
                    shoot.Regs(jumpData, false);
                    shoot.Regs(walkDataEx, false);
                    shoot.Regs(shootOverData, false, FArpgMachine.Greater);
                    {
                        FArpgMachine.FArpgNode shoot1 = shoot.Regs(shootData01, true, FArpgMachine.TotalGreater);
                        {
                            shoot1.Regs(shootDataOver, false, FArpgMachine.Greater);
                            shoot1.Regs(walkDataEx, false, FArpgMachine.TotalGreater);
                            shoot1.Regs(jumpData, false, FArpgMachine.TotalGreater);
                            {
                                FArpgMachine.FArpgNode shoot2 = shoot1.Regs(shootData02, true, FArpgMachine.TotalGreater);
                                {
                                    shoot2.Regs(shootDataOver, false, FArpgMachine.Greater);
                                    shoot2.Regs(walkDataEx, false, FArpgMachine.TotalGreater);
                                    shoot2.Regs(jumpData, false, FArpgMachine.TotalGreater);
                                    {
                                        FArpgMachine.FArpgNode shoot3 = shoot2.Regs(shootData03, true, FArpgMachine.TotalGreater).Regs(shootAllOver, false, FArpgMachine.TotalGreater);
                                    }
                                }
                            }
                        }
                    }
                }

                //跳跃
                FArpgMachine.FArpgNode jump = idel.Regs(jumpData, true, FArpgMachine.TotalGreater);
                {
                    jump.Regs(jumpOverData, false, FArpgMachine.TotalGreater);
                    {
                        FArpgMachine.FArpgNode jumpShoot = jump.Regs(jumpShoot01, true, FArpgMachine.TotalGreater);
                        jumpShoot.Regs(jumpOverData, false, FArpgMachine.TotalGreater);
                        {
                            FArpgMachine.FArpgNode jumpShoott = jumpShoot.Regs(jumpShoot02, true, FArpgMachine.TotalGreater);
                            jumpShoott.Regs(jumpOverData, false, FArpgMachine.Greater);
                            jumpShoott.Regs(jumpShoot02, false, FArpgMachine.TotalGreater);
                        }
                    }
                }

                //走路
                FArpgMachine.FArpgNode walk = idel.Regs(walkData, true, FArpgMachine.TotalGreater);
                {
                    walk.Regs(walkOverData, false, FArpgMachine.Greater);
                    walk.Regs(jumpData, false, FArpgMachine.TotalGreater);
                    {
                        FArpgMachine.FArpgNode walkShoot = walk.Regs(walkshootData, true, FArpgMachine.TotalGreater);
                        walkShoot.Regs(walkOverData, false, FArpgMachine.Greater);
                        //walkShoot.Regs(walkData, false);
                        walkShoot.Regs(shootData01, false, FArpgMachine.TotalGreater);
                    }
                }
            }

        }
    }
}
