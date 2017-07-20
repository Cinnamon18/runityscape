﻿using Scripts.View.ObjectPool;
using UnityEngine;

namespace Scripts.View.Effects {

    public class ExplosionView : PooledBehaviour {

        [SerializeField]
        private ParticleSystem ps;

        public Vector2 Dimensions {
            set {
                var x = ps.shape;
                x.box = value;
                x.radius = value.x / 2;
            }
        }

        public bool IsDone {
            get {
                return !ps.IsAlive();
            }
        }

        public void Play() {
            ps.Play();
        }

        public override void Reset() {
            ps.time = 0;
        }
    }
}