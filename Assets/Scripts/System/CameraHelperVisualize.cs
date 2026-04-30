using UnityEngine;
using System.Collections.Generic;

public class CameraHelperVisualize : MonoBehaviour
{
    [Header("Material Visualizer")]
    public Material matHoverHijau;   // Untuk kotak pusat (JalurAir)
    public Material matSpreadBiru;   // Untuk area penyebaran (TanahKering)
    public Material matHijau, matDry;

    [Header("Pengaturan Area")]
    public float radiusPenghijauan = 2.1f; // Samakan dengan radius di WaterNode

    // Menyimpan objek yang sedang disorot
    private GameObject currentHoverObj;
    private List<GameObject> affectedTanah = new List<GameObject>();

    // Menyimpan susunan material asli agar bisa dikembalikan
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            // Jika mouse menyorot JalurAir
            if (hitObj.CompareTag("JalurAir"))
            {
                // Cegah fungsi terpanggil berkali-kali di frame yang sama
                if (hitObj != currentHoverObj)
                {
                    ClearHighlights();
                    HighlightArea(hitObj);
                }
            }
            else
            {
                // Jika menyorot objek lain (seperti pohon/batu)
                ClearHighlights();
            }
        }
        else
        {
            // Jika mouse menunjuk ke awang-awang
            ClearHighlights();
        }
    }

    void HighlightArea(GameObject pusat)
    {
        currentHoverObj = pusat;

        // 1. Tambahkan material hijau ke JalurAir yang sedang disorot
        pusat.GetComponent<MeshRenderer>().enabled = true;
        AddMaterial(pusat, matHoverHijau);

        // 2. Cari blok tanah di sekitarnya menggunakan radar
        Collider[] hits = Physics.OverlapSphere(pusat.transform.position, radiusPenghijauan);
        foreach (Collider c in hits)
        {
            // Pastikan tag-nya sesuai dengan tanah yang bisa dihijaukan
            if (c.CompareTag("TanahKering"))
            {
                GameObject tanah = c.gameObject;
                affectedTanah.Add(tanah);

                // Tambahkan material biru ke tanah tersebut
                AddMaterial(tanah, matSpreadBiru);
            }
        }
    }

    void ClearHighlights()
    {
        // 1. Tangani objek pusat (JalurAir) HANYA JIKA belum dihancurkan oleh script lain
        if (currentHoverObj != null)
        {
            RestoreMaterial(currentHoverObj);
            
            // Matikan kembali mesh-nya (biar bolong lagi) dengan aman
            MeshRenderer mr = currentHoverObj.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
        }
        
        currentHoverObj = null;

        // 2. SELALU bersihkan material biru di tanah sekitar, 
        // MESKIPUN JalurAir-nya udah hancur/berubah jadi air
        foreach (GameObject tanah in affectedTanah)
        {
            if (tanah != null) 
            {
                RestoreMaterial(tanah);
            }
        }

        // 3. Kosongkan memori list-nya
        affectedTanah.Clear();
        originalMaterials.Clear();
    }

    void AddMaterial(GameObject obj, Material highlightMat)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            // Simpan array material aslinya jika belum ada di dictionary
            if (!originalMaterials.ContainsKey(rend))
            {
                // Pakai sharedMaterials agar lebih ringan di memory
                originalMaterials[rend] = rend.sharedMaterials;
            }

            Material[] oldMats = originalMaterials[rend];
            Material[] newMats = new Material[oldMats.Length + 1];

            // Copy material lama
            for (int i = 0; i < oldMats.Length; i++)
            {
                newMats[i] = oldMats[i];
            }
            // Selipkan material visualizer di urutan paling akhir
            newMats[oldMats.Length] = highlightMat;

            rend.sharedMaterials = newMats;
        }
    }

    void RestoreMaterial(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null && originalMaterials.ContainsKey(rend))
        {
            if (obj.tag == "TanahKering")
            {
                Material[] mat = new Material[1];
                mat[0] = matDry;
                rend.sharedMaterials = mat;
            }
            else if (obj.tag == "TanahHijau")
            {
                Material[] mat = new Material[1];
                mat[0] = matHijau;
                rend.sharedMaterials = mat;
            }
            else
            {
                // Kembalikan ke array material aslinya
                rend.sharedMaterials = originalMaterials[rend];
            }
        }
    }
}
