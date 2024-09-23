using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace TEMARI.View
{
    public class NewMonoBehaviour : SDBase
    {
        protected override void Start()
        {
            destination *= -1;
            knockBackBody *= -1;
            knockBackWord *= -1;
            base.Start();
            wordList = new List<string>() { "足を引っ張ったら\n殺すから", "低俗だね", "いらいらするな", "馬鹿じゃないの" };
        }
    }
}