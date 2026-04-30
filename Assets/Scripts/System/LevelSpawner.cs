using UnityEngine;
using DG.Tweening;
using System.Collections;
public class LevelSpawner : MonoBehaviour
{
    [Header("Pengaturan Animasi DOTween")]
    public float durasiMuncul = 0.4f;
    public float delayAntarBlok = 0.002f;
    public float jarakDariBawah = 2f;
    public Ease tipeAnimasi = Ease.OutBack;

    void Start()
    {
        MulaiSpawnLevel();
    }

    void MulaiSpawnLevel()
    {
        // Penghitung global agar delay terus berlanjut dari Layer 1 ke Layer 2
        int hitunganDelay = 0; 

        // 1. Loop pertama: Mengecek setiap Layer (Layer 1, Layer 2, dst)
        int jumlahLayer = transform.childCount;
        for (int i = 0; i < jumlahLayer; i++)
        {
            Transform layerTarget = transform.GetChild(i);

            // 2. Loop kedua: Mengecek setiap blok (Dirt) di dalam Layer tersebut
            int jumlahBlok = layerTarget.childCount;
            for (int j = 0; j < jumlahBlok; j++)
            {
                Transform blok = layerTarget.GetChild(j);

                // Simpan ukuran dan posisi lokal aslinya
                Vector3 ukuranAsli = blok.localScale;
                Vector3 posisiAsli = blok.localPosition;

                // Setup kondisi awal (skala 0 dan turun ke bawah)
                blok.localScale = Vector3.zero;
                blok.localPosition = new Vector3(posisiAsli.x, posisiAsli.y - jarakDariBawah, posisiAsli.z);

                // Eksekusi animasi Skala
                blok.DOScale(ukuranAsli, durasiMuncul)
                    .SetDelay(hitunganDelay * delayAntarBlok)
                    .SetEase(tipeAnimasi);

                // Eksekusi animasi Posisi naik ke atas
                blok.DOLocalMoveY(posisiAsli.y, durasiMuncul)
                    .SetDelay(hitunganDelay * delayAntarBlok)
                    .SetEase(tipeAnimasi);

                // Tambahkan hitungan agar blok berikutnya muncul sedikit lebih lambat
                hitunganDelay++; 
            }
        }
    
    }
}
