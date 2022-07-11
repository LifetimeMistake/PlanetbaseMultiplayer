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
    public class SolarFlareProxy : IDisasterProxy
    {
        private FieldInfo mTimeInfo;
        private FieldInfo mSolarFlareTimeInfo;
        private FieldInfo mSolarFlareInProgressInfo;
        private FieldInfo mIntensityInfo;
        private MethodInfo onStartInfo;

        private SolarFlare solarFlare;

        public float Time
        {
            get => (float)Reflection.GetInstanceFieldValue(solarFlare, mTimeInfo);
            set => Reflection.SetInstanceFieldValue(solarFlare, mTimeInfo, value);
        }
        public float DisasterLength
        {
            get => (float)Reflection.GetInstanceFieldValue(solarFlare, mSolarFlareTimeInfo);
            set => Reflection.SetInstanceFieldValue(solarFlare, mSolarFlareTimeInfo, value);
        }
        public float Intensity
        {
            get => (float)Reflection.GetInstanceFieldValue(solarFlare, mIntensityInfo);
            set => Reflection.SetInstanceFieldValue(solarFlare, mIntensityInfo, value);
        }
        public bool SolarFlareInProgress
        {
            get => (bool)Reflection.GetInstanceFieldValue(solarFlare, mSolarFlareInProgressInfo);
            set => Reflection.SetInstanceFieldValue(solarFlare, mSolarFlareInProgressInfo, value);
        }

        public SolarFlareProxy(float time, float disasterLength, SolarFlare solarFlare)
        {
            mTimeInfo = Reflection.GetPrivateFieldOrThrow(solarFlare.GetType(), "mTime", true);
            mSolarFlareTimeInfo = Reflection.GetPrivateFieldOrThrow(solarFlare.GetType(), "mSolarFlareTime", true);
            mSolarFlareInProgressInfo = Reflection.GetPrivateFieldOrThrow(solarFlare.GetType(), "mSolarFlareInProgress", true);
            mIntensityInfo = Reflection.GetPrivateFieldOrThrow(solarFlare.GetType(), "mIntensity", true);
            onStartInfo = Reflection.GetPrivateMethodOrThrow(solarFlare.GetType(), "onStart", true);

            this.solarFlare = solarFlare;
            Time = time;
            DisasterLength = disasterLength;
        }

        public void StartDisaster()
        {
            SolarFlareInProgress = true;
            Reflection.InvokeInstanceMethod(solarFlare, onStartInfo, new object[] { });
        }

        public void EndDisaster()
        {
            SolarFlareInProgress = false;
            Singleton<Planetbase.EnvironmentManager>.getInstance().refreshAmbientSound();
        }

        public void UpdateDisaster(float currentTime)
        {
            Planet currentPlanet = PlanetManager.getCurrentPlanet();
            if (currentPlanet.getSolarFlareRisk() != Planet.Quantity.None)
            {
                Singleton<MusicManager>.getInstance().onTension();

                Time = currentTime;
                Reflection.SetInstanceFieldValue(solarFlare, mTimeInfo, Time);

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
            }
        }
    }
}
