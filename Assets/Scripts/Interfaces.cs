using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public interface IPoolable
    {
        void OnInstantiated(Transform parent);
        void OnSpawned(Entity.State state);
        void OnDespawned();
    }

    public interface ITrasnsformable
    {
        void Move();
        void Rotate();
        void Scale();
    }

    public interface IPlayable
    {
        void SetInput(Vector2 mousePosition, bool leftClick, bool rightClick);
    }

    public interface IPerishable
    {
        float Life { get; }
        void UpdateLife(float deltaTime);
    }
}