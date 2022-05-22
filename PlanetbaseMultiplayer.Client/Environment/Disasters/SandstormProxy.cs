using Planetbase;
using PlanetbaseMultiplayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Environment.Disasters
{
    public class SandstormProxy : IDisasterProxy
    {
		private FieldInfo mTimeInfo;
		private FieldInfo mSandstormTimeInfo;
		private FieldInfo mSandstormInProgressInfo;
		private FieldInfo mIntensityInfo;
		private FieldInfo mParticleSystemInfo;
		private FieldInfo mOriginalEmissionRateInfo;
		private MethodInfo onStartInfo;
		private MethodInfo updatePositionInfo;

		private float mOriginalEmissionRate;

		private ParticleSystem mParticleSystem;
		private Sandstorm sandstorm;

        public float Time { get; set; }
        public float DisasterLength { get; set; }

        public SandstormProxy(float time, float disasterLength, Sandstorm sandstorm)
        {
			mTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mTime", true);
			mSandstormTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mSandstormTime", true);
			mSandstormInProgressInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mSandstormInProgressInfo", true);
			mIntensityInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mIntensity", true);
			mParticleSystemInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mParticleSystem", true);
			mOriginalEmissionRateInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mOriginalEmissionRate", true);
			onStartInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "onStart", true);
			updatePositionInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "updatePosition", true);

			Time = time;
            DisasterLength = disasterLength;
            this.sandstorm = sandstorm;
		}

        public void StartDisaster()
        {
            Reflection.SetInstanceFieldValue(sandstorm, mTimeInfo, Time);
            Reflection.SetInstanceFieldValue(sandstorm, mSandstormTimeInfo, DisasterLength);
            Reflection.SetInstanceFieldValue(sandstorm, mSandstormInProgressInfo, true);
            Reflection.InvokeInstanceMethod(sandstorm, onStartInfo, new object[] { });
			mOriginalEmissionRate = (float)Reflection.GetInstanceFieldValue(sandstorm, mOriginalEmissionRateInfo);
			mParticleSystem = (ParticleSystem)Reflection.GetInstanceFieldValue(sandstorm, mParticleSystemInfo);
		}

        public void EndDisaster()
        {
            MethodInfo destroyParticlesInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "destroyParticles", true);

            Singleton<Planetbase.EnvironmentManager>.getInstance().refreshAmbientSound();
            Reflection.InvokeInstanceMethod(sandstorm, destroyParticlesInfo, new object[] { });

			Reflection.SetInstanceFieldValue(sandstorm, mSandstormInProgressInfo, false);
        }

        public void UpdateDisaster(float timeStep)
        {
			Planet currentPlanet = PlanetManager.getCurrentPlanet();
			if (currentPlanet.getSandstormRisk() != Planet.Quantity.None)
			{
				Singleton<MusicManager>.getInstance().onTension();
				Reflection.InvokeInstanceMethod(sandstorm, updatePositionInfo, new object[] { });

				Time += timeStep;
				Reflection.SetInstanceFieldValue(sandstorm, mTimeInfo, Time);

				float num = Time / DisasterLength;
				if (num < 0.25f)
				{
					Reflection.SetInstanceFieldValue(sandstorm, mIntensityInfo, Mathf.Clamp01(4f * num));
				}
				else if (num > 0.75f)
				{
					Reflection.SetInstanceFieldValue(sandstorm, mIntensityInfo, Mathf.Clamp01(4f * (1f - num)));
				}
				else
				{
					Reflection.SetInstanceFieldValue(sandstorm, mIntensityInfo, 1f);
				}
				float mIntensity = (float)Reflection.GetInstanceFieldValue(sandstorm, mIntensityInfo);
				mParticleSystem.emissionRate = mOriginalEmissionRate * mIntensity;
			}
		}
    }
}
