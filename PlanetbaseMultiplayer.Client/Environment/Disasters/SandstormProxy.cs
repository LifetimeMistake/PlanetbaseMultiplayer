using Planetbase;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Utils;
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

        public float Time
        {
			get => (float)Reflection.GetInstanceFieldValue(sandstorm, mTimeInfo);
			set => Reflection.SetInstanceFieldValue(sandstorm, mTimeInfo, value);
        }
        public float DisasterLength
		{
			get => (float)Reflection.GetInstanceFieldValue(sandstorm, mSandstormTimeInfo);
			set => Reflection.SetInstanceFieldValue(sandstorm, mSandstormTimeInfo, value);
		}
		public float Intensity
        {
			get => (float)Reflection.GetInstanceFieldValue(sandstorm, mIntensityInfo);
			set => Reflection.SetInstanceFieldValue(sandstorm, mIntensityInfo, value);
        }
		public bool SandstormInProgress
		{
			get => (bool)Reflection.GetInstanceFieldValue(sandstorm, mSandstormInProgressInfo);
			set => Reflection.SetInstanceFieldValue(sandstorm, mSandstormInProgressInfo, value);
		}

		public SandstormProxy(float time, float disasterLength, Sandstorm sandstorm)
        {
			mTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mTime", true);
			mSandstormTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mSandstormTime", true);
			mSandstormInProgressInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mSandstormInProgress", true);
			mIntensityInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mIntensity", true);
			mParticleSystemInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mParticleSystem", true);
			mOriginalEmissionRateInfo = Reflection.GetPrivateFieldOrThrow(sandstorm.GetType(), "mOriginalEmissionRate", true);
			onStartInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "onStart", true);
			updatePositionInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "updatePositon", true);

			this.sandstorm = sandstorm;
			Time = time;
            DisasterLength = disasterLength;
		}

        public void StartDisaster()
        {
			SandstormInProgress = true;
            Reflection.InvokeInstanceMethod(sandstorm, onStartInfo, new object[] { });
			mOriginalEmissionRate = (float)Reflection.GetInstanceFieldValue(sandstorm, mOriginalEmissionRateInfo);
			mParticleSystem = (ParticleSystem)Reflection.GetInstanceFieldValue(sandstorm, mParticleSystemInfo);
		}

        public void EndDisaster()
        {
            MethodInfo destroyParticlesInfo = Reflection.GetPrivateMethodOrThrow(sandstorm.GetType(), "destroyParticles", true);
            Reflection.InvokeInstanceMethod(sandstorm, destroyParticlesInfo, new object[] { });
			SandstormInProgress = false;
			Singleton<Planetbase.EnvironmentManager>.getInstance().refreshAmbientSound();
		}

        public void UpdateDisaster(float currentTime)
        {
			Planet currentPlanet = PlanetManager.getCurrentPlanet();
			if (currentPlanet.getSandstormRisk() != Planet.Quantity.None)
			{
				Singleton<MusicManager>.getInstance().onTension();
				Reflection.InvokeInstanceMethod(sandstorm, updatePositionInfo, new object[] { });

				Time = currentTime;
				Reflection.SetInstanceFieldValue(sandstorm, mTimeInfo, Time);

				float num = Time / DisasterLength;
				if (num < 0.25f)
				{
					Intensity = Mathf.Clamp01(4f * num);
				}
				else if (num > 0.75f)
				{
					Intensity = Mathf.Clamp01(4f * (1f - num));
				}
				else
				{
					Intensity = 1f;
				}
				mParticleSystem.emissionRate = mOriginalEmissionRate * Intensity;
			}
		}
    }
}
