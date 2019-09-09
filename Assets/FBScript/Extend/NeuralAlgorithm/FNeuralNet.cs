//----------------------------------------------
//  F2DEngine: time: 2017.6  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace F2DEngine
{
    //神经网络
    [System.Serializable]
    public class FNeuralNet
    {
        private const double BISDP = -1;//神经细胞偏移值
        private const float SIGMOID_DP = 1;//S函数曲度
        private const double MOMENTUM = 0.9;
        private const double ERROR_THRESHOLD = 0.05;//误差域
        //神经细胞
        [System.Serializable]
        public class NeuralData
        {
            public List<double> _Code = new List<double>();
            private int mInputNum = 0;
            //to add momentum
            public List<double> m_vecPrevUpdate = new List<double>();
            //监督学习
            public double mActivation;//激励值
            public double m_dError;//误差值

            public NeuralData(int inputNum)
            {
                mInputNum = inputNum + 1;
                //神经偏移1位
                for (int i = 0; i < inputNum + 1; i++)
                {
                    _Code.Add(0);
                    m_vecPrevUpdate.Add(0);
                }
            }

            public void SetCode(List<double> codes)
            {
                if (mInputNum == codes.Count)
                {
                    _Code = new List<double>(codes);
                }
                else
                {
                    Debug.LogError("NeralData"+"----编码数量对不上");
                }
            }
        }
        //神经细胞层
        [System.Serializable]
        public class NeuralLayerData
        {
            public List<NeuralData> mNeuralDataList = new List<NeuralData>();
            public int _intputNum;
            public int GetWeightNum()
            {
                //神经偏移了1位
                return _intputNum + 1;
            }
            public NeuralLayerData(int neuralNum,int intputNum)
            {
                _intputNum = intputNum;
                for (int i = 0; i < neuralNum;i++)
                {
                    NeuralData nd = new NeuralData(intputNum);
                    mNeuralDataList.Add(nd);
                }
            }
        }
        private List<NeuralLayerData> mNeuralLayerDataList = new List<NeuralLayerData>();
        public double GetEndWights()
        {
            return mNeuralLayerDataList[1].mNeuralDataList[3]._Code[9];
        }
        public void SetWeights(List<double> weights)
        {
            int startIndex = 0;
            FEngineManager.SetList(mNeuralLayerDataList, (f, index) =>
            {
                for(int i = 0; i < f.mNeuralDataList.Count;i++)
                {
                    NeuralData nd = f.mNeuralDataList[i];
                    nd.SetCode(weights.GetRange(startIndex,nd._Code.Count));
                    startIndex += nd._Code.Count;
                }
            });
        }
        public int GetWeightNum()
        {
            int wightNum = 0;
            FEngineManager.SetList(mNeuralLayerDataList, (f, i) => 
            {
                wightNum += (f.mNeuralDataList.Count *f.GetWeightNum());
            });
            return wightNum;
        }
        public int mIntputNum;
        public int mOutputNum;
        public int mLayerNum;
        public int mNeuralNum;
        public void CreateNet(int intputNum,int outputNum,int layerNum = 1,int neuralNum = 10)
        {
            mIntputNum = intputNum;
            mOutputNum = outputNum;
            mLayerNum = layerNum;
            mNeuralNum = neuralNum;
            if (layerNum > 0)
            {
                NeuralLayerData nld = new NeuralLayerData(neuralNum, intputNum);
                mNeuralLayerDataList.Add(nld);
                for (int i = 0; i < layerNum -1;i++)
                {
                    nld = new NeuralLayerData(neuralNum, neuralNum);
                    mNeuralLayerDataList.Add(nld);
                }

                nld = new NeuralLayerData(outputNum, neuralNum);
                mNeuralLayerDataList.Add(nld);
            }
            else
            {
                NeuralLayerData nld = new NeuralLayerData(outputNum, intputNum);
                mNeuralLayerDataList.Add(nld);
            }
        }
        public void CreateLearnNet(int intputNum, int outputNum,double learnRate = 0.2,int neuralNum = 18)
        {
            mErrorSum = 9999;
            mTrained = false;
            mNumEpochs = 0;
            mLearningRate = learnRate;
            CreateNet(intputNum, outputNum, 1, neuralNum);
        }
        public List<double> UpdateOne(List<double> inputs)
        {
            List<double> outPuts = new List<double>();
            for(int i = 0; i < mNeuralLayerDataList.Count;i++)
            {
                if(i > 0)
                {
                    inputs = new List<double>(outPuts);
                }
                outPuts.Clear();
                NeuralLayerData neuralLayer = mNeuralLayerDataList[i];
                for(int j = 0;j < neuralLayer.mNeuralDataList.Count;j++)
                {
                    double netInput = 0;
                    NeuralData ndata = neuralLayer.mNeuralDataList[j];
                    for(int n = 0; n < ndata._Code.Count-1;n++)
                    {
                        netInput += ndata._Code[n] * inputs[n];
                    }
                    netInput += ndata._Code[ndata._Code.Count - 1] * BISDP;
                    ndata.mActivation = GetSigmoid(netInput);
                    outPuts.Add(ndata.mActivation);
                }
            }

            return outPuts;
        }
        private double GetSigmoid(double value)
        {
            return 1 / (1 + Mathf.Exp(-(float)value/SIGMOID_DP));
        }
        //监督学习
        private double mLearningRate;//学习率
        private double mErrorSum = 9999;//累计误差
        public bool mTrained;//是否训练完成
        private int mNumEpochs = 0;//时代计数器

        private void NetworkTrainingEpoch(List<TrainData> data)
        {
            double WeightUpdate = 0;
            mErrorSum = 0;
            for(int vec = 0; vec <data.Count;vec++)
            {
                TrainData TData = data[vec];
                List<double> outputs = UpdateOne(TData.setIn);
                if(outputs.Count == 0)
                {
                    Debug.LogError("训练发生了错误");
                }

                for(int op = 0;op < mOutputNum;op++)
                {
                    //计算误差
                    double err = (TData.setOut[op] - outputs[op]) * outputs[op] * (1 - outputs[op]);

                    mNeuralLayerDataList[1].mNeuralDataList[op].m_dError = err;

                    List<double> curWeight = mNeuralLayerDataList[1].mNeuralDataList[op]._Code;
                    var curNrnHid = mNeuralLayerDataList[0].mNeuralDataList;
                    int w = 0;
                    int hidIndex = 0;

                    //更新每一个权重,偏移除外
                    for(int i = 0; i < curWeight.Count-1;i++)
                    {
                        WeightUpdate = err * mLearningRate * curNrnHid[hidIndex].mActivation;
                        curWeight[i] += WeightUpdate + mNeuralLayerDataList[1].mNeuralDataList[op].m_vecPrevUpdate[w] * MOMENTUM;
                        mNeuralLayerDataList[1].mNeuralDataList[op].m_vecPrevUpdate[w] = WeightUpdate;
                        w++;
                        hidIndex++;
                    }

                    //计算偏移值
                    WeightUpdate = err * mLearningRate * BISDP;
                    curWeight[curWeight.Count -1] += WeightUpdate + mNeuralLayerDataList[1].mNeuralDataList[op].m_vecPrevUpdate[w] * MOMENTUM;
                    mNeuralLayerDataList[1].mNeuralDataList[op].m_vecPrevUpdate[w] = WeightUpdate;
                }

                double error = 0;
                for (int o = 0; o < mOutputNum; ++o)
                {

                    error += (TData.setOut[o] - outputs[o]) * (TData.setOut[o] - outputs[o]);
                }
                mErrorSum += error;

                int n = 0;
                for(int i = 0; i < mNeuralLayerDataList[0].mNeuralDataList.Count;i++)
                {
                    double err = 0;
                    var curHid = mNeuralLayerDataList[0].mNeuralDataList[i];
                    for(int j = 0; j < mNeuralLayerDataList[1].mNeuralDataList.Count;j++)
                    {
                        var curOut = mNeuralLayerDataList[1].mNeuralDataList[j];
                        err += curOut.m_dError * curOut._Code[n];
                    }

                    //now we can calculate the error
                    err *= curHid.mActivation * (1 - curHid.mActivation);
                    for(int w=0;w < mIntputNum;w++)
                    {
                        WeightUpdate = err * mLearningRate * TData.setIn[w];
                        curHid._Code[w] += WeightUpdate + curHid.m_vecPrevUpdate[w] * MOMENTUM;
                        curHid.m_vecPrevUpdate[w] = WeightUpdate;
                    }

                    WeightUpdate = err * mLearningRate * BISDP;
                    curHid._Code[mIntputNum] += WeightUpdate + curHid.m_vecPrevUpdate[mIntputNum] * MOMENTUM;
                    curHid.m_vecPrevUpdate[mIntputNum] = WeightUpdate;
                    n++;
                }
            }
        }
        public class TrainData
        {
            public List<double> setIn = new List<double>();
            public List<double> setOut = new List<double>();
        }
        public void TrainNeural(List<TrainData> data)
        {
            if (!mTrained)
            {
                ResetCode();
                mNumEpochs = 0;
                while (mErrorSum > ERROR_THRESHOLD)
                {
                    NetworkTrainingEpoch(data);
                    mNumEpochs++;
                }
                mTrained = true;
            }
        }

        private void ResetCode()
        {
            for(int i = 0; i < mNeuralLayerDataList.Count;i++)
            {
                var lay = mNeuralLayerDataList[i];
                for(int j = 0; j < lay.mNeuralDataList.Count;j++)
                {
                    var ner = lay.mNeuralDataList[j];
                    for(int k = 0; k < ner._Code.Count;k++)
                    {
                        ner._Code[k] = Random.Range(-1.0f, 1.0f); 
                    }
                }
            }
        }
    }
}
