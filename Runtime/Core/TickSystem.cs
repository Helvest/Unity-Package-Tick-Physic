using System;
using System.Collections.Generic;

namespace TickPhysics
{
    public abstract class TickSystem : ITickSystem
    {
        #region Fields

        public virtual bool IsPhysicUpdated { get; set; } = true;

        public virtual double TimeAtSimulation { get; protected set; }

        public virtual double NormalTime { get; protected set; }

        public virtual double FixedTime { get; protected set; }

        public virtual uint FixedFrameCount { get; protected set; }

        public virtual float ExtraDeltaTime { get; protected set; }

        protected virtual bool IsInUpdateLoop { get; set; }

        protected virtual int LoopIndex { get; set; }

        #endregion

        #region Events

        public event Action EventReadInput;
        public event Action EventUpdatePhysic;
        public event Action<uint> EventProcessInput;
        public event Action EventUpdateGraphic;

        #endregion

        #region IPhysicObject

        private readonly List<IPhysicsObject> _physicObjects = new List<IPhysicsObject>(256);

        public virtual void Add(params IPhysicsObject[] physicObjectsToAdd)
        {
            foreach (var item in physicObjectsToAdd)
            {
                if (!_physicObjects.Contains(item))
                {
                    _physicObjects.Add(item);
                }
            }
        }

        public virtual void Remove(params IPhysicsObject[] physicObjectsToRemove)
        {
            foreach (var item in physicObjectsToRemove)
            {
                int index = _physicObjects.IndexOf(item);
                _physicObjects.RemoveAt(index);

                if (IsInUpdateLoop && index < LoopIndex)
                {
                    LoopIndex--;
                }
            }
        }

        #endregion

        #region Tick

        public virtual void Tick(double time, double deltaTime, double fixedDeltaTime)
        {
            ReadInput();

            if (!IsPhysicUpdated)
            {
                return;
            }

            TimeAtSimulation = time;

            NormalTime += deltaTime;

            PhysicTick(fixedDeltaTime);

            CalculateExtraDeltaTime();

            // Here you can access the transforms state right after the simulation, if needed...	
            UpdateGraphic();

            //IsInUpdateLoop = false;
        }

        protected virtual void PhysicTick(double fixedDeltaTime)
        {
            if (fixedDeltaTime <= 0)
            {
                return;
            }

            // Catch up with the game time.
            // Advance the physics simulation by portions of fixedDeltaTime
            while (NormalTime >= FixedTime + fixedDeltaTime)
            {
                FixedFrameCount++;

                FixedTime += fixedDeltaTime;

                //Prepare inputs for actual physic frame
                ProcessInput();

                //A custom FixedUpdate
                UpdatePhysic();

                //Advance the simulation, this will also call OnTrigger and OnCollider
                SimulatePhysic(fixedDeltaTime);
            }
        }

        protected virtual void CalculateExtraDeltaTime()
        {
            ExtraDeltaTime = (float)(NormalTime - FixedTime);
        }

        #endregion

        #region Custom Updates

        protected virtual void ReadInput()
        {
            EventReadInput?.Invoke();
        }

        protected virtual void UpdatePhysic()
        {
            IsInUpdateLoop = true;

            for (LoopIndex = 0; LoopIndex < _physicObjects.Count; LoopIndex++)
            {
                _physicObjects[LoopIndex].UpdatePhysics();
            }

            IsInUpdateLoop = false;

            EventUpdatePhysic?.Invoke();
        }

        protected virtual void ProcessInput()
        {
            EventProcessInput?.Invoke(FixedFrameCount);
        }

        protected abstract void SimulatePhysic(double fixedDeltaTime);

        protected virtual void UpdateGraphic()
        {
            IsInUpdateLoop = true;

            for (LoopIndex = 0; LoopIndex < _physicObjects.Count; LoopIndex++)
            {
                _physicObjects[LoopIndex].UpdateGraphics();
            }

            IsInUpdateLoop = false;

            EventUpdateGraphic?.Invoke();
        }

        #endregion
    }
}