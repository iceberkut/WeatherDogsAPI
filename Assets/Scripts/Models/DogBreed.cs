using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class DogBreed
    {
        public string id;
        public string name;
        public string description;
        public string imageUrl;
        public string temperament;
        public WeightAndHeight lifeSpan;
        public WeightAndHeight weight;
        public WeightAndHeight height;
        public string breed_group;
        public string bred_for;
        public string reference_image_id;
    }

    [Serializable]
    public class WeightAndHeight
    {
        public string imperial;
        public string metric;
    }
} 