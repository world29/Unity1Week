﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public interface IPlatform
    {
        /// <summary>
        /// 上に載っているものを通知する。フレーム毎にリセットされるため、載っているフレームで毎回呼ぶ必要がある
        /// </summary>
        /// <param name="passenger"></param>
        void OnLandingPlatform(Transform passenger);
    }
}
