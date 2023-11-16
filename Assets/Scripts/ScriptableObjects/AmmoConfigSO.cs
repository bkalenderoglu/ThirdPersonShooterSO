using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 4)]
public class AmmoConfigSO : ScriptableObject
{
    public int maxAmmo = 120;
    public int clipSize = 30;

    public int currentAmmo = 120;
    public int currentClipAmmo = 30;

    /// <summary>
    /// Reloads with the ammo conserving algorithm.
    /// Meaning it will only subtract the delta between the ClipSize and CurrentClipAmmo from the CurrentAmmo.
    /// </summary>
    public void Reload() {
        int maxReloadAmount = Mathf.Min(clipSize, currentAmmo);
        int availableBulletsInCurrentClip = clipSize - currentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);
        currentClipAmmo += reloadAmount;
        currentAmmo -= reloadAmount;
    }

    /// <summary>
    /// Reloads not conserving ammo.
    /// Meaning it will always subtract the ClipSize from CurrentAmmo (if available).
    /// </summary>
    //public void Reload()
    //{
    //    int reloadAmount = Mathf.Min(clipSize, currentAmmo);
    //    currentClipAmmo = reloadAmount;
    //    currentAmmo -= reloadAmount;
    //}

    public bool CanReload() {
        return currentClipAmmo < clipSize && currentAmmo > 0;
    }
}
