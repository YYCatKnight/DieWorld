
//----------------------------------------------
//  F2DEngine: time: 2017.6  by fucong QQ:353204643
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace F2DEngine
{
    
    public class NeuralAlgorithm
    {
        //普通遗传算法
        public static FIntelligenc<GeneData> CreateNormalGene(int maxGeneNum, int maxElementNum, int elementTypeNum, Func<GeneData, float> callBack = null)
        {
            FIntelligenc<GeneData> gd = new FIntelligenc<GeneData>();
            gd.CreateGene(maxGeneNum, maxElementNum, elementTypeNum, callBack);
            return gd;
        }

        //不重复遗传算法
        public static FIntelligenc<GeneOnlyData> CreateOnlyGene(int maxGeneNum,int elementTypeNum, Func<GeneOnlyData, float> callBack = null)
        {
            FIntelligenc<GeneOnlyData> gd = new FIntelligenc<GeneOnlyData>();
            gd.CreateGene(maxGeneNum, elementTypeNum, elementTypeNum, callBack);
            return gd;
        }


        //非监督神经遗传算法
        public static FIntelligenc<UnLearnNeural> CreateUnLearnNeural(int maxGeneNum,int intputNum, int outputNum,int layerNum = 1, int neuralNum = 10, Func<UnLearnNeural, float> callBack = null)
        {
            FIntelligenc<UnLearnNeural> gd = new FIntelligenc<UnLearnNeural>();
            gd.CreateGene(maxGeneNum,0,0, callBack,(i,f) =>
            {
                f.CreateNet(intputNum,outputNum,layerNum,neuralNum);
            });
            return gd;
        }
    }


    //遗传算法
    public class FIntelligenc<T> : FBaseIntelligence where T : baseGene<T>, new()
    {
        private List<T> mGenes = new List<T>();
        private int mElementTypeNum = 0;
        private int mMaxElementNum;
        private int mMaxGenesNum;
        private float mAllFit = 0;
        private int mEvolveNum = 1;

        public int GetEvolveNum()
        {
            return mEvolveNum;
        }

        public List<T> GetGenes()
        {
            return mGenes;
        }


        //fit分越大,越fit
        public T GetBestFit()
        {
            float fit = 0;
            T t = null;
            for(int i = 0; i < mGenes.Count;i++)
            {
                float f = mGenes[i].GetFit();
                if(f > fit)
                {
                    t = mGenes[i];
                    fit = f;
                }
            }
            return t;
        }


        //maxGeneNum 基因种族最大数，maxElementNum 基因长度，elementTypeNum 基因元素种类
        public void CreateGene(int maxGeneNum, int maxElementNum, int elementTypeNum, Func<T, float> callBack,Action<int,T> InitCallBack = null)
        {
            mElementTypeNum = elementTypeNum;
            mMaxElementNum = maxElementNum;
            mMaxGenesNum = maxGeneNum;
            for (int i = 0; i < maxGeneNum; i++)
            {
                T tempGene = new T();
                if (InitCallBack != null)
                {
                    InitCallBack(i, tempGene);
                }
                tempGene.Init(i, maxGeneNum, maxElementNum, elementTypeNum, callBack);
                mGenes.Add(tempGene);
            }
            UpdateGenesFit();
        }

        public void EvolveGenes(int num)
        {
            for(int i = 0; i < num;i++)
            {
                UpdateEvolveGenes();
            }
        }

        private void UpdateEvolveGenes()
        {
            UpdateGenesFit();
            List<T> sortGenes = new List<T>(mGenes);
            sortGenes.Sort((t,f) => { return (int)(f.GetFit() * 10000 - t.GetFit() * 10000); });

            //选取适合分数最高的基因1%
            int fitNum = (int)(mGenes.Count * 0.01f+1);
            List<T> newChildGenes = new List<T>(sortGenes.GetRange(0,fitNum));

            //正常赌轮产生基因
            int leftChildsNum = Mathf.Clamp(mGenes.Count - fitNum,0,mGenes.Count);
            for(int i = 0; i < leftChildsNum;i+=2)
            {
              T one = RouletteWheel(Random.Range(0, mAllFit));
              T two = RouletteWheel(Random.Range(0, mAllFit));
              T[] childs = GetNewChildGenes(one, two);
              newChildGenes.AddRange(childs);
            }

            mGenes.Clear();
            mGenes.AddRange(newChildGenes.GetRange(0, mMaxGenesNum));
            UpdateGenesFit();
            mEvolveNum++;
        }


        private void UpdateGenesFit()
        {
            mAllFit = 0;
            FEngineManager.SetList(mGenes, (f, i) =>
            {
                mAllFit += f.UpdateFit();
            });
        }
        private T RouletteWheel(float fit)
        {
            for (int j = 0; j < mGenes.Count; j++)
            {
                if ((fit -= mGenes[j].GetFit()) <= 0)
                {
                    return mGenes[j];
                }
            }
            return default(T);
        }


        T[] GetNewChildGenes(T m, T b)
        {
            T[] childs = new T[2];
            int index = Random.Range(0, m._Code.Count);
            T[] temps = new T[2];
            temps[0] = m;
            temps[1] = b;
            for (int i = 0; i < 2; i++)
            {
                T newData = null;

                //70%概率杂交
                if (Random.Range(0, 100) < 70)
                {
                    newData = temps[0].GetCrossGenes(temps[1], index);
                }
                else
                {
                    newData = temps[0];
                }

                //变异0.001
                for (int j = 0; j < newData._Code.Count; j++)
                {
                    if (Random.Range(0, 1000) == 100)
                    {
                        newData.ChangeGenes(j);
                    }
                }

                newData.CopyData(temps[0]);
                
                childs[i] = newData;
                temps[0] = b;
                temps[1] = m;
            }
            return childs;
        }


    }


    public class baseGene<T> where T:baseGene<T>,new()
    {
        public List<int> _Code = new List<int>();
        protected int mMaxGeneNum;
        protected int mMaxElementNum;
        protected int mElementTypeNum;
        protected int mId;
        protected float mFit = 0;
        protected Func<T, float> mCallBack;     

        public void SetFit(float fit)
        {
            mFit = fit;
            if(mFit < 0)
            {
                mFit = 0.0001f;
            }
        }

        public float GetFit()
        {
            return mFit;
        }

        public float UpdateFit()
        {
            if(mCallBack != null)
            {
                mFit = mCallBack((T)this);
            }
            return mFit;
        }
        public virtual void CopyData(T mother)
        {
            mId = mother.mId;
            mMaxGeneNum = mother.mMaxGeneNum;
            mMaxElementNum = mother.mMaxElementNum;
            mElementTypeNum = mother.mElementTypeNum;
            mCallBack = mother.mCallBack;
        }

        public void Init(int id, int maxGeneNum, int maxElementNum, int elementTypeNum,Func<T,float> callBack)
        {
            mId = id;
            mMaxGeneNum = maxGeneNum;
            mMaxElementNum = maxElementNum;
            mElementTypeNum = elementTypeNum;
            mCallBack = callBack;
            InitData();
        }

        public virtual void InitData()
        {
           
            
        }

        public virtual T GetCrossGenes(T other,int index)
        {
            return default(T);
        }

        public  virtual void ChangeGenes(int index)
        {

        }

    }


    //无监督神经算法
    public class UnLearnNeural: GeneDataTemplate<UnLearnNeural>
    {
        public FNeuralNet mNeuralNet;

        public void CreateNet(int intputNum, int outputNum,int layerNum = 1, int neuralNum = 10)
        {
            mNeuralNet = new FNeuralNet();
            mNeuralNet.CreateNet(intputNum,outputNum,layerNum,neuralNum);
        }

        public override void CopyData(UnLearnNeural mother)
        {
            base.CopyData(mother);
            CreateNet(mother.mNeuralNet.mIntputNum, mother.mNeuralNet.mOutputNum, mother.mNeuralNet.mLayerNum, mother.mNeuralNet.mNeuralNum);
            _UpdateNeural();
        }

        private void _UpdateNeural()
        {
            List<double> doubleCode = new List<double>();
            for(int i = 0; i < _Code.Count;i++)
            {
                doubleCode.Add(_Code[i]/100000.0);
            }
            mNeuralNet.SetWeights(doubleCode);
        }

        public override void InitData()
        {
            int num = mNeuralNet.GetWeightNum();
            for (int i = 0; i < num; i++)
            {
                _Code.Add(Random.Range(-100000, 100000));
            }
            _UpdateNeural();
        }

        public override void ChangeGenes(int index)
        {
            _Code[index] = Random.Range(-100000,100000);
        }
    }

    public class GeneDataTemplate<T>:baseGene<T> where T : baseGene<T>, new()
    {
        public override void ChangeGenes(int index)
        {
            _Code[index] = Random.Range(0, mElementTypeNum);
        }

        public override T GetCrossGenes(T other, int index)
        {
            T gd = new T();
            for (int x = 0; x < _Code.Count; x++)
            {
                if (x < index)
                {
                    gd._Code.Add(_Code[x]);
                }
                else
                {
                    gd._Code.Add(other._Code[x]);
                }
            }
            return gd;
        }

        public override void InitData()
        {
            for (int i = 0; i < mMaxElementNum; i++)
            {
                _Code.Add(Random.Range(0, mElementTypeNum));
            }
        }
    }

    public class GeneData : GeneDataTemplate<GeneData>
    {
    }

    public class GeneOnlyData: baseGene<GeneOnlyData>
    {
        public override void InitData()
        {
            List<int> tempIndex = new List<int>();
            for (int i = 0; i < mElementTypeNum; i++)
            {
                tempIndex.Add(i);
            }

            for (int i = 0; i < mElementTypeNum; i++)
            {
                int range = Random.Range(0, tempIndex.Count);
                _Code.Add(tempIndex[range]);
                tempIndex.RemoveAt(range);
            }
        }


        public override GeneOnlyData GetCrossGenes(GeneOnlyData other, int index)
        {
            int minIndex = index;
            int maxindex = Random.Range(0,_Code.Count);
            if (minIndex > maxindex)
            {
                minIndex = maxindex;
                maxindex = index;
            }

            GeneOnlyData gd = new GeneOnlyData();
            Dictionary<int, int> newMDic = new Dictionary<int, int>();
            for (int i = minIndex; i < _Code.Count && i < maxindex; i++)
            {
                newMDic[_Code[i]] = other._Code[i];
            }
            gd._Code.AddRange(_Code);

            foreach (var key in newMDic)
            {
                int temp = key.Key;
                int tempValue = key.Value;
                int lookNum = 0;

                for (int x = 0; x < gd._Code.Count&& lookNum < 2; x++)
                {
                    int value = gd._Code[x];
                    if(value == temp)
                    {
                        gd._Code[x] = tempValue;
                        lookNum++;
                    }
                    else if(value == tempValue)
                    {
                        gd._Code[x] = temp;
                        lookNum++;
                    }
                }
            }
            return gd;
        }

        public override void ChangeGenes(int index)
        {
            int switchIndex = Random.Range(0,_Code.Count);
            int curTemp = _Code[index];
            _Code[index] = _Code[switchIndex];
            _Code[switchIndex] = curTemp;
        }

    }
}
