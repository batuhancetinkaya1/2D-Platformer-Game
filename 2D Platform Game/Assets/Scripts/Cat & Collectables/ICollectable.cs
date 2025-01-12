using UnityEngine;

public interface ICollectable
{
   void OnCollect(PlayerCore collector);
    void OnDestroy();
}
