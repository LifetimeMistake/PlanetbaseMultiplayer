using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class ProduceResourceDataPackage : IDataPackage
    {
        public int ProducerId;
        public ProducerType ProducerType;
        public ResourceConstructionData[] ProducedResources;
        public ResourceDestructionData[] ConsumedResources;

        public ProduceResourceDataPackage(int producerId, ProducerType producerType, ResourceConstructionData[] producedResources, ResourceDestructionData[] consumedResources)
        {
            ProducerId = producerId;
            ProducerType = producerType;
            ProducedResources = producedResources;
            ConsumedResources = consumedResources;
        }
    }

    public enum ProducerType
    {
        Module,
        Component
    }
}
