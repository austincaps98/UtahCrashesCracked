using System;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace UtahCrashesCracked.Models
{
    public class InputData
    {
        public float pedestrian_involved { get; set; }
        public float bicyclist_involved { get; set; }
        public float motorcycle_involved { get; set; }
        public float improper_restraint { get; set; }
        public float unrestrained { get; set; }
        public float dui { get; set; }
        public float intersection_related { get; set; }
        public float overturn_rollover { get; set; }
        public float older_driver_involved { get; set; }
        public float distracted_driving { get; set; }
        public float drowsy_driving { get; set; }
        public float city_SALT_LAKE_CITY { get; set; }
        public float single_vehicle { get; set; }
        public float roadway_departure { get; set; }

        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                pedestrian_involved,
                bicyclist_involved,
                motorcycle_involved,
                improper_restraint,
                unrestrained,
                dui,
                intersection_related,
                overturn_rollover,
                older_driver_involved,
                single_vehicle,
                distracted_driving,
                drowsy_driving,
                roadway_departure,
                city_SALT_LAKE_CITY
            };
            int[] dimensions = new int[] { 1, 14 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
