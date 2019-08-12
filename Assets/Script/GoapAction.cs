using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Goap
{
    public abstract class GoapAction
    {
        Dictionary<string, object> preconditions;
        Dictionary<string, object> effects;

        public float cost = 1f;

        GameObject actor;

        public GoapAction(Dictionary<string, object> _preconditions, Dictionary<string, object> _effects, GameObject _actor)
        {
            preconditions = _preconditions;
            effects = _effects;
            actor = _actor;
        }

        public void ForceReset()
        {

        }

        public abstract void Reset();

        public abstract bool IsDone();

        public abstract bool CheckPrecondition(GameObject agent);

        public abstract bool PerformAction(GameObject agent);

        public void AddPrecondition(string key, object value)
        {
            preconditions[key] = value;
        }

        public void RemovePrecondition(string key)
        {
            preconditions.Remove(key);
        }

        public void AddEffect(string key, object value)
        {
            effects[key] = value;
        }

        public void RemoveEffect(string key)
        {
            effects.Remove(key);
        }

        public Dictionary<string, object> Preconditions
        {
            get
            {
                return preconditions;
            }
        }

        public Dictionary<string, object> Effects
        {
            get
            {
                return effects;
            }
        }
    }
}


