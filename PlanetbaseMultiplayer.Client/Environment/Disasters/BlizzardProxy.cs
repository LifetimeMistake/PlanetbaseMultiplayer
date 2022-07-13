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
    public class BlizzardProxy : IDisasterProxy
    {
        private FieldInfo mTimeInfo;
        private FieldInfo mBlizzardTimeInfo;
        private FieldInfo mBlizzardInProgressInfo;
        private FieldInfo mIntensityInfo;
        private FieldInfo mParticleSystemsInfo;
        private FieldInfo mOriginalEmissionRatesInfo;
        private MethodInfo onStartInfo;
        private MethodInfo updatePositionInfo;

        private float[] mOriginalEmissionRates;

        private Blizzard blizzard;
        private ParticleSystem[] mParticleSystems;

        public float Time
        {
            get => (float)Reflection.GetInstanceFieldValue(blizzard, mTimeInfo);
            set => Reflection.SetInstanceFieldValue(blizzard, mTimeInfo, value);
        }
        public float DisasterLength
        {
            get => (float)Reflection.GetInstanceFieldValue(blizzard, mBlizzardTimeInfo);
            set => Reflection.SetInstanceFieldValue(blizzard, mBlizzardTimeInfo, value);
        }
        public float Intensity
        {
            get => (float)Reflection.GetInstanceFieldValue(blizzard, mIntensityInfo);
            set => Reflection.SetInstanceFieldValue(blizzard, mIntensityInfo, value);
        }
        public bool BlizzardInProgress
        {
            get => (bool)Reflection.GetInstanceFieldValue(blizzard, mBlizzardInProgressInfo);
            set => Reflection.SetInstanceFieldValue(blizzard, mBlizzardInProgressInfo, value);
        }

        public BlizzardProxy(float time, float disasterLength, Blizzard blizzard)
        {
            mTimeInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mTime", true);
            mBlizzardTimeInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mBlizzardTime", true);
            mBlizzardInProgressInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mBlizzardInProgress", true);
            mIntensityInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mIntensity", true);
            mParticleSystemsInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mParticleSystems", true);
            mOriginalEmissionRatesInfo = Reflection.GetPrivateFieldOrThrow(blizzard.GetType(), "mOriginalEmissionRates", true);
            onStartInfo = Reflection.GetPrivateMethodOrThrow(blizzard.GetType(), "onStart", true);
            updatePositionInfo = Reflection.GetPrivateMethodOrThrow(blizzard.GetType(), "updatePositon", true);

            this.blizzard = blizzard;
            Time = time;
            DisasterLength = disasterLength;
        }

        public void StartDisaster()
        {
            BlizzardInProgress = true;
            Reflection.InvokeInstanceMethod(blizzard, onStartInfo, new object[] { });
            mOriginalEmissionRates = (float[])Reflection.GetInstanceFieldValue(blizzard, mOriginalEmissionRatesInfo);
            mParticleSystems = (ParticleSystem[])Reflection.GetInstanceFieldValue(blizzard, mParticleSystemsInfo);
        }

        public void EndDisaster()
        {
            MethodInfo destroyParticlesInfo = Reflection.GetPrivateMethodOrThrow(blizzard.GetType(), "destroyParticles", true);
            Reflection.InvokeInstanceMethod(blizzard, destroyParticlesInfo, new object[] { });
            BlizzardInProgress = false;
            Singleton<Planetbase.EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public void UpdateDisaster(float currentTime)
        {
            Planet currentPlanet = PlanetManager.getCurrentPlanet();
            if (currentPlanet.getBlizzardRisk() != Planet.Quantity.None)
            {
                Singleton<MusicManager>.getInstance().onTension();
                Reflection.InvokeInstanceMethod(blizzard, updatePositionInfo, new object[] { });

                Time = currentTime;
                Reflection.SetInstanceFieldValue(blizzard, mTimeInfo, Time);

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
                for (int i = 0; i < mParticleSystems.Length; i++)
                {
                    mParticleSystems[i].emissionRate = mOriginalEmissionRates[i] * Intensity;
                }
            }
        }
    }
}
