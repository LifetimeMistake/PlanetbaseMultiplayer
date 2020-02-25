using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PlanetbaseMultiplayer.Patcher
{
    public class EmptyAi : BaseAi
    {
        public EmptyAi()
        {
            mRuleTimer = new PerformanceTimer(base.GetType().Name + " rules", 0);
            mIdleRules = new List<AiRule>();
            mTargetRules = new List<AiTargetRule>();
        }
    }
}
